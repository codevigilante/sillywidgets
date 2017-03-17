using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace silly
{
    public class BuildDirective : SillyDirective
    {
        private enum BuildOptions { Continuous, Location, Unknown }
        private string DefaultLocation = Directory.GetCurrentDirectory();
        private bool Continuous = false;

        public BuildDirective(string id) : base(id, "renders the static content")
        {
            base.AddOption(new DirectiveOption("-continuous", "re-renders on change"));
            base.AddOption(new DirectiveOption("-location",  "renders the site located at the path",
                                                new List<string>() { "<path/to/site/directory>" }));
        }

        public override void Execute(string[] args)
        {
            ParseArgs(args);

            if (!Directory.Exists(DefaultLocation))
            {
                throw new Exception ("The location provided, " + DefaultLocation + ", doesn't seem to exist.");
            }

            string siteConfigFilename = DefaultLocation + @"/.silly/site_config.json";

            if (!File.Exists(siteConfigFilename))
            {
                throw new Exception("Can't resolve " + siteConfigFilename);
            }

            SiteConfigModel siteConfig = JsonConvert.DeserializeObject<SiteConfigModel>(File.ReadAllText(siteConfigFilename));
        }

        private void ParseArgs(string[] args)
        {
            for(int i = 0; i < args.Length; ++i)
            {
                string arg = args[i].ToLower();

                if (String.Compare(arg, "-continuous", true) == 0)
                {
                    Continuous = true;
                }
                else if (String.Compare(arg, "-location", true) == 0)
                {
                    ++i;
                    DefaultLocation = (i < args.Length) ? args[i] : throw new Exception("Expected <path/to/site.json> for -location");
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