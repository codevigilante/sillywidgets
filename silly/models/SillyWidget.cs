using System;
using System.IO;
using HtmlAgilityPack;

namespace silly
{
    public class SillyWidget : SillyModel
    {
        public string ID { get; set; }
        public FileInfo Source { get; set; }

        protected HtmlDocument Html = new HtmlDocument();

        public SillyWidget(FileInfo widgetFile, DirectoryInfo root, string prefix = "")
            : base(root)
        {
            if (!widgetFile.Exists)
            {
                throw new Exception("Widget file '" + widgetFile.FullName + "' does not exist");
            }

            ID = prefix + Path.GetFileNameWithoutExtension(widgetFile.Name);
            Source = widgetFile;

            LoadHtml();
        }

        public override bool Compile()
        {
            HtmlNode root = Html.DocumentNode;

            HtmlNodeCollection widgetNodes = root.SelectNodes("//*[@silly-widget]");

            if (widgetNodes == null)
            {
                return(true);
            }
            else
            {
                throw new Exception("Widget '" + Source.FullName + "' references other widgets, which is a no no");
            }
        }

        public HtmlNode GetRoot()
        {
            return(Html.DocumentNode);
        }

        protected void LoadHtml()
        {
            using (FileStream fs = new FileStream(Source.FullName, FileMode.Open))
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