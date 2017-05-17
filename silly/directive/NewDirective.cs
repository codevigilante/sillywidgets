using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace silly
{
    public class NewDirective : SillyDirective
    {
        public string DefaultSiteName { get; set; }

        private string RouteHtml = "<html>\n\t<head>\n\t\t<title>Silly Site</title>\n\t</head>\n\t<body>\n\t\t<div silly-widget=\"hello\"></div>\n\t</body>\n</html>";
        private string WidgetHtml = "<div style=\"text-align:center;color:#555;\">\n\t<h1>Hello!</h1>\n\t<p>If you're reading this, then silly widgets has created a new project for you. Have fun!</p>\n\t<p>PS If you need help, go to <a href=\"http://sillywidgets.com\">sillywidgets.com</a>.</p>\n</div>";

        public NewDirective(string id) : base(id, "initialize a new silly site")
        {
            ValidOptions.Add(AllowableTokens.SiteName, null);

            DefaultSiteName = "Silly Site";
            base.HelpString = "Initialize a new silly site";
        }

        protected override void Run()
        {
            SiteNameOption sitenameOption = ValidOptions[AllowableTokens.SiteName] as SiteNameOption;

            if (sitenameOption != null)
            {
                Console.Write("Setting site name...");

                DefaultSiteName = sitenameOption.GetName();

                Console.WriteLine("done");
            }

            SiteOptions Options = SiteOptions.CreateStockOptions();

            CreateJson(Options);

            DirectoryInfo routesDir = CreateDirectoryIfNotExist(Options.Routes, RouteHtml, "index.html", true);
            DirectoryInfo widgetsDir = CreateDirectoryIfNotExist(Options.Widgets, WidgetHtml, "hello.html", true);
        }

        private void CreateJson(SiteOptions siteOptions)
        {
            Console.Write("Checking for existing silly site...");

            SiteOptions existingOptions = SiteOptions.DiscoveryInDirectory(DefaultLocation);

            if (existingOptions != null)
            {
                throw new Exception("Looks like a silly site already exists here.");
            }

            Console.WriteLine("done");

            string filename = DefaultLocation.FullName + "/" + DefaultLocation.Name + ".json";
            FileInfo jsonFile = new FileInfo(filename);
            
            Console.Write("Creating " + jsonFile.Name + "...");

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using(FileStream fw = jsonFile.Create())
            using(StreamWriter sw = new StreamWriter(fw))
            using(JsonWriter jw = new JsonTextWriter(sw))
            {
                serializer.Serialize(jw, siteOptions);
            }

            Console.WriteLine("done");
        }

        private DirectoryInfo CreateDirectoryIfNotExist(string dirName, string htmlToCreate, string filename, bool abortIfAlreadyExists)
        {
            Console.Write("Creating directory " + dirName + "...");

            DirectoryInfo dir = new DirectoryInfo(DefaultLocation.FullName + "/" + dirName);

            if (dir.Exists && abortIfAlreadyExists)
            {
                throw new Exception(dirName + " already exists, so it doesn't feel right overwriting it");
            }

            if (!dir.Exists)
            {
                dir.Create();
            }

            Console.WriteLine("done");

            Console.Write("Creating " + filename + "...");

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlToCreate);

            FileInfo htmlFile = new FileInfo(dir.FullName + "/" + filename);

            using(FileStream fw = htmlFile.Create())
            using(StreamWriter sw = new StreamWriter(fw))
            {
                sw.Write(htmlToCreate);
            }

            Console.WriteLine("done");

            return(dir);
        }
    }
}