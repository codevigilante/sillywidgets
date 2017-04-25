using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace silly
{
    public class SillySite : SillyModel
    {
        public class SiteOptions
        {
            public int LocalPort { get; set; }
            public SillySitewideVariables SitewideVariables { get; set; }
            public string DeployTo { get; set; }
        };
        public SiteOptions Options { get; private set; }
        public SiteConfig Config { get; private set; }
        public Dictionary<string, SillyRoute> Routes { get; set; }
        public Dictionary<string, FileInfo> NonRouteResources { get; private set; }
        public DirectoryInfo RootDir { get; private set; }

        static public Dictionary<string, SillyWidget> WidgetTable = new Dictionary<string, SillyWidget>();

        private string SiteConfigFileName = @"/.silly/site_config.json";

        public SillySite(DirectoryInfo rootDir)
        {
            RootDir = rootDir;
            Routes = new Dictionary<string, SillyRoute>();
            NonRouteResources = new Dictionary<string, FileInfo>();
            Options = new SiteOptions();            
            Options.LocalPort = 8080;
            Options.DeployTo = "deploy";
        }

        public bool Setup(SiteConfig config = null)
        {
            if (config == null)
            {
                FileInfo siteConfigJsonFilename = new FileInfo(RootDir.FullName + SiteConfigFileName);

                Console.Write("Reading " + siteConfigJsonFilename.FullName + "...");

                if (!siteConfigJsonFilename.Exists)
                {
                    throw new Exception("Can't resolve " + siteConfigJsonFilename.FullName);
                }

                Config = JsonConvert.DeserializeObject<SiteConfig>(File.ReadAllText(siteConfigJsonFilename.FullName));

                if (Config == null)
                {
                    throw new Exception("Problem interpreting " + siteConfigJsonFilename.FullName);
                }
                
                Console.WriteLine("done");
            }
            else
            {
                Config = config;
            }

            FileInfo siteJsonFilename = new FileInfo(RootDir.FullName + "/" + Config.EntryPoint);

            Console.Write("Reading " + siteJsonFilename.FullName + "...");

            Options = JsonConvert.DeserializeObject<SiteOptions>(File.ReadAllText(siteJsonFilename.FullName));

            if (Options == null)
            {
                throw new Exception("Problem interpreting " + siteJsonFilename.FullName);
            }

            Console.WriteLine("done");

            DirectoryInfo widgetsDir = CheckDirectory(Config.Widgets);

            if (widgetsDir != null)
            {
                BuildWidgetTable(widgetsDir);
            }
            else
            {
                Console.WriteLine("No widgets found");
            } 

            return(true);
        }

        public override bool Compile(SiteConfig config = null)
        {
            Console.WriteLine("Compiling...");          

            Setup(config);

            DirectoryInfo routesDir = CheckDirectory(Config.Routes);

            if (routesDir != null)
            {
                BuildRoutes(routesDir, "/");
            }
            else
            {
                Console.WriteLine("No routes defined");
            }

            return (true);
        }

        public bool Deploy()
        {
            Console.WriteLine("Prepping deployment...");

            if (RootDir == null)
            {
                throw new Exception("Deployment target directory is unknown");
            }

            DirectoryInfo deploymentDir = new DirectoryInfo(RootDir.FullName + "/" + Options.DeployTo);

            if (deploymentDir.Exists)
            {
                EmptyDirectory(deploymentDir);
            }
            else
            {
                Console.Write("Creating deploy directory '" + deploymentDir.Name + "'...");

                deploymentDir.Create();

                Console.WriteLine("done");
            }

            foreach(KeyValuePair<string, FileInfo> resource in NonRouteResources)
            {
                Console.Write("Copying resource " + resource.Key + "...");

                FileInfo deployFile = new FileInfo(deploymentDir.FullName + resource.Key);

                if (!deployFile.Exists)
                {
                    deployFile.Directory.Create();
                    FileStream newFile = deployFile.Create();

                    newFile.Dispose();
                }

                resource.Value.CopyTo(deploymentDir.FullName + resource.Key, true);

                Console.WriteLine("done");
            }

            foreach(KeyValuePair<string, SillyRoute> route in Routes)
            {
                Console.Write("Resolving route " + route.Key + "...");

                string payload = route.Value.Resolve();

                Console.WriteLine("done");

                FileInfo deployFile = new FileInfo(deploymentDir.FullName + route.Key + ".html");

                deployFile.Directory.Create();

                using(FileStream routeFile = deployFile.Create())
                {
                    routeFile.Write(Encoding.ASCII.GetBytes(payload), 0, payload.Length);
                }
            }

            return(true);
        }

        private void EmptyDirectory(DirectoryInfo dir)
        {
            foreach(FileInfo file in dir.GetFiles())
            {
                DeleteThing(file);
            }

            foreach(DirectoryInfo subDir in dir.GetDirectories())
            {
                EmptyDirectory(subDir);

                DeleteThing(subDir);
            }
        }

        private void DeleteThing(FileSystemInfo thing)
        {
            Console.Write("Deleting " + thing.Name + "...");

            thing.Delete();

            Console.WriteLine("done");
        }

        static public FileInfo WidgetFileFromName(string widgetName)
        {
            SillyWidget widget = null;

            WidgetTable.TryGetValue(widgetName, out widget);

            if (widget != null)
            {
                return(widget.Source);
            }

            return(null);
        }

        private DirectoryInfo CheckDirectory(string targetDir)
        {
            DirectoryInfo dir = new DirectoryInfo(RootDir.FullName + "/" + targetDir);

            Console.Write("Checking directory '" + targetDir + "'...");

            if (dir.Exists)
            {
                Console.WriteLine("done");

                return (dir);
            }
        
            Console.WriteLine("Directory not found. Expected it here: " + dir.FullName);
            
            return(null);
        }

        private void BuildRoutes(DirectoryInfo routesDir, string currentPath = "")
        {
            string pathSuffix = string.Empty;

            if (!String.IsNullOrEmpty(currentPath) && String.Compare(currentPath, "/", true) != 0)
            {
                pathSuffix = "/";
            }

            string totalPath = currentPath + pathSuffix;

            foreach(FileInfo routeFile in routesDir.GetFiles())
            {
                if (String.Compare(routeFile.Extension, ".html", true) == 0)
                {
                    SillyRoute route = new SillyRoute(routeFile, totalPath);

                    Console.Write("Compiling " + route.ID + "...");

                    route.Compile(Config);

                    Routes.Add(route.ID, route);

                    Console.WriteLine("done");
                }
                else
                {
                    Console.WriteLine("Skipping non-route file " + totalPath + routeFile.Name);

                    NonRouteResources.Add(totalPath + routeFile.Name, routeFile);
                }
            }

            foreach(DirectoryInfo routeSubDir in routesDir.GetDirectories())
            {
                BuildRoutes(routeSubDir, totalPath + routeSubDir.Name);
            }
        }

        private void BuildWidgetTable(DirectoryInfo widgetsDir, string currentPath = "")
        {
            string pathSuffix = String.IsNullOrEmpty(currentPath) ? "" : "/";

            foreach(FileInfo widgetFile in widgetsDir.GetFiles())
            {
                if (String.Compare(widgetFile.Extension, ".html", true) == 0)
                {
                    SillyWidget widget = new SillyWidget(widgetFile, currentPath + pathSuffix);

                    Console.Write("Compiling " + widget.ID + "...");

                    widget.Compile(Config);

                    SillySite.WidgetTable.Add(widget.ID, widget);

                    Console.WriteLine("done");
                }
                else
                {
                    Console.WriteLine("Skipping non-widget file " + widgetFile.Name);
                }
            }

            foreach(DirectoryInfo widgetSubDir in widgetsDir.GetDirectories())
            {
                BuildWidgetTable(widgetSubDir, currentPath + pathSuffix + widgetSubDir.Name);
            }
        }
    }
}