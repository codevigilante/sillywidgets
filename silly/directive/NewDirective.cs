using System;
using System.Collections.Generic;
using System.IO;
//using Newtonsoft.Json;

namespace silly
{
    public class NewDirective : SillyDirective
    {
        public string DefaultSiteName { get; set; }

        public NewDirective(string id) : base(id, "creates a new [site-name] directory (default is 'newsite') with the structure:\n\nsite-name\n\twidgets\n\tassets\n\t\tcss\n\t\tjs\n\t\timg\n\tsite.json\n")
        {
            DefaultSiteName = "newsite";

            base.AddOption(new DirectiveOption("", "name of the site to create", new List<string>() { "[site-name]" }));
        }

        // dotnet run new <app name>
        public override void Execute(string[] args)
        {
            string dirName = DefaultSiteName;

            if (args.Length > 1)
            {
                dirName = args[1];
            }

            Directory.CreateDirectory(dirName);
        }

        /*private void CreateSiteJson()
        {
            if (File.Exists(DefaultEntryPoint))
            {
                throw new Exception(DefaultEntryPoint + " already exists in current directory. Nothing to do.");
            }

            FileStream siteJsonFile = new FileStream(DefaultEntryPoint, FileMode.Create);
            Site newSite = new Site();

            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(siteJsonFile))
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(sw))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;                

                    serializer.Serialize(jsonTextWriter, newSite);
                }
            }
        }*/
    }
}