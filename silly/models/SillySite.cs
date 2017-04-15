using System;
using System.Collections.Generic;
using System.IO;
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
        public List<SillyRoute> Routes { get; set; }
        public DirectoryInfo RootDir { get; private set; }

        static public Dictionary<string, SillyWidget> WidgetTable = new Dictionary<string, SillyWidget>();
        static public Dictionary<string, FileInfo> Assets = new Dictionary<string, FileInfo>();

        private string SiteConfigFileName = @"/.silly/site_config.json";

        public SillySite(DirectoryInfo rootDir)
        {
            RootDir = rootDir;

            Options = new SiteOptions();
            
            Options.LocalPort = 8080;
        }

        public override bool Compile(SiteConfig config = null)
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

            Console.WriteLine("Compiling...");

            DirectoryInfo assetsDir = CheckDirectory(Config.Assets);

            if (assetsDir != null)
            {
                BuildAssets(assetsDir, Config.Assets);
            }
            else
            {
                Console.WriteLine("No assets found");
            }

            DirectoryInfo widgetsDir = CheckDirectory(Config.Widgets);

            if (widgetsDir != null)
            {
                BuildWidgetTable(widgetsDir);
            }
            else
            {
                Console.WriteLine("No widgets to compile");
            }            

            DirectoryInfo routesDir = CheckDirectory(Config.Routes);

            if (routesDir != null)
            {
                BuildRoutes(routesDir);
            }
            else
            {
                Console.WriteLine("No routes defined");
            }

            return (true);
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

        private void BuildAssets(DirectoryInfo dir, string currentPath = "")
        {
            string pathSuffix = String.IsNullOrEmpty(currentPath) ? "" : "/";

            foreach(FileInfo file in dir.GetFiles())
            {
                string id = currentPath + pathSuffix + file.Name;

                Console.Write("Assimilating " + id + "...");

                Assets.Add(id, file);

                Console.WriteLine("done");
            }

            foreach(DirectoryInfo subDir in dir.GetDirectories())
            {
                BuildAssets(subDir, currentPath + pathSuffix + subDir.Name);
            }
        }

        private void BuildRoutes(DirectoryInfo routesDir, string currentPath = "")
        {
            Routes = new List<SillyRoute>();

            string pathSuffix = String.IsNullOrEmpty(currentPath) ? "" : "/";

            foreach(FileInfo routeFile in routesDir.GetFiles())
            {
                if (String.Compare(routeFile.Extension, ".html", true) == 0)
                {
                    SillyRoute route = new SillyRoute(routeFile, currentPath + pathSuffix);

                    Console.Write("Compiling " + route.ID + "...");

                    route.Compile(Config);

                    Routes.Add(route);

                    Console.WriteLine("done");
                }
                else
                {
                    Console.WriteLine("Skipping non-route file " + routeFile.Name);
                }
            }

            foreach(DirectoryInfo routeSubDir in routesDir.GetDirectories())
            {
                BuildRoutes(routeSubDir, currentPath + pathSuffix + routeSubDir.Name);
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

        public bool Deploy(string targetDir)
        {            
            return(true);
        }
    }
}