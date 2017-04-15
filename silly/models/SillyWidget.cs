using System;
using System.IO;
using HtmlAgilityPack;

namespace silly
{
    public class SillyWidget : SillyModel
    {
        public string ID { get; set; }
        public string Source { get; set; }

        protected HtmlDocument Html = new HtmlDocument();
        
        public SillyWidget(string id, string htmlFilename)
            : base()
        {
            this.ID = id;
            this.Source = htmlFilename;

            Html.OptionCheckSyntax = true;
        }

        public SillyWidget(FileInfo widgetFile, string prefix = "")
        {
            if (!widgetFile.Exists)
            {
                throw new Exception("Widget file '" + widgetFile.FullName + "' does not exist");
            }

            ID = prefix + Path.GetFileNameWithoutExtension(widgetFile.Name);
            Source = widgetFile.FullName;
        }

        public override bool Compile(SiteConfig config = null)
        {
            LoadHtml();

            HtmlNode root = Html.DocumentNode;

            HtmlNodeCollection widgetNodes = root.SelectNodes("//*[@silly-widget]");

            if (widgetNodes == null)
            {
                return(true);
            }
            else
            {
                throw new Exception("Widget '" + Source + "' references other widgets, which is a no no");
            }
        }

        protected void LoadHtml()
        {
            using (FileStream fs = new FileStream(Source, FileMode.Open))
            {
                Html.Load(fs);

                if (Html.ParseErrors != null)
                {
                    bool wasErrors = false;

                    foreach(HtmlParseError htmlError in Html.ParseErrors)
                    {
                        Console.WriteLine(htmlError.Reason);

                        wasErrors = true;
                    }

                    if (wasErrors)
                    {
                        throw new Exception("ERROR parsing widget");
                    }
                }
            }
        }
    }
}