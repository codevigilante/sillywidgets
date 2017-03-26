using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace silly
{
    public class BuildDirective : SillyDirective
    {
        private enum BuildOptions { Continuous, Location, Unknown }
        private string DefaultLocation = "/";
        private bool Continuous = false;
        private string SiteConfigFileName = @"/.silly/site_config.json";

        public BuildDirective(string id) : base(id, "renders the static content")
        {
            base.AddOption(new DirectiveOption("-continuous", "re-renders on change"));
            base.AddOption(new DirectiveOption("-location",  "renders the site located at the path",
                                                new List<string>() { "<path/to/site/directory>" }));
        }

        public override void Execute(string[] args)
        {
            Console.Write("Checking args...");

            ParseArgs(args);

            Console.WriteLine("done");

            string siteConfigJsonFilename = DefaultLocation + SiteConfigFileName;

            Console.Write("Reading " + siteConfigJsonFilename + "...");

            if (!Directory.Exists(DefaultLocation))
            {
                throw new Exception ("The location provided, " + DefaultLocation + ", doesn't seem to exist.");
            }

            if (!File.Exists(siteConfigJsonFilename))
            {
                throw new Exception("Can't resolve " + siteConfigJsonFilename);
            }

            SiteConfigModel siteConfig = JsonConvert.DeserializeObject<SiteConfigModel>(File.ReadAllText(siteConfigJsonFilename));

            if (siteConfig == null)
            {
                throw new Exception("Problem interpreting " + siteConfigJsonFilename);
            }
            
            Console.WriteLine("done");

            string siteJsonFilename = DefaultLocation + siteConfig.EntryPoint;

            Console.Write("Reading " + siteJsonFilename + "...");

            SillySite sillySite = JsonConvert.DeserializeObject<SillySite>(File.ReadAllText(siteJsonFilename));

            Console.WriteLine("done");

            Console.WriteLine("Compiling...");

            sillySite.Compile(DefaultLocation);

            Console.Write("Deploying...");

            sillySite.Deploy();

            Console.WriteLine("done");
        }

        private void ParseArgs(string[] args)
        {
            for(int i = 0; i < args.Length; ++i)
            {
                string arg = args[i].ToLower();

                if (String.Compare(arg, "-continuous", true) == 0)
                {
                    Continuous = true;
                    throw new Exception("-continuous option not implemented yet...sorry.");
                }
                else if (String.Compare(arg, "-location", true) == 0)
                {
                    ++i;
                    DefaultLocation = (i < args.Length) ? args[i] + "/" : throw new Exception("Expected <path/to/site.json> for -location");
                }
                else if (String.Compare(arg, "silly", true) == 0 ||
                         String.Compare(arg, "build", true) == 0)
                {
                }
                else
                {
                    throw new Exception ("Unexpected build option: " + arg);
                }
            }
        }
    }
}