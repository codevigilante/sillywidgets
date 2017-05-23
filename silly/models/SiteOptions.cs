using System;
using System.IO;
using Newtonsoft.Json;

namespace silly
{
    public class SiteOptions
    {
        public string SillyWidgets { get; set; }
            public int LocalPort { get; set; }
            public SillySitewideVariables SitewideVariables { get; set; }
            public string DeployTo { get; set; }
            public string Widgets { get; set; }
            public string Routes { get; set; }

            public static SiteOptions CreateStockOptions()
            {
                SiteOptions options = new SiteOptions();

                options.SillyWidgets = "v0.1";
                options.LocalPort = 7575;
                options.SitewideVariables = new SillySitewideVariables();
                options.SitewideVariables.Version = "0.1";
                options.DeployTo = "deploy";
                options.Routes = "routes";
                options.Widgets = "widgets";

                return(options);
            }

            public static SiteOptions DiscoveryInDirectory(DirectoryInfo dir)
            {
                foreach(FileInfo file in dir.GetFiles())
                {
                    if (String.Compare(file.Extension, ".json", true) == 0)
                    {
                        SiteOptions existingOptions = JsonConvert.DeserializeObject<SiteOptions>(File.ReadAllText(file.FullName));

                        if (existingOptions != null &&
                            !String.IsNullOrEmpty(existingOptions.SillyWidgets))
                        {
                            return(existingOptions);
                        }
                    }
                }

                return(null);
            }
    }
}