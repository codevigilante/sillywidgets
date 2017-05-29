using System;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace silly
{
    public class SillyRoute : SillyWidget
    {
        public Object Variables { get; set; }

        private List<HtmlNode> Widgets = new List<HtmlNode>();


        public SillyRoute(FileInfo widgetFile, DirectoryInfo root, string prefix = "")
            : base(widgetFile, root, prefix)
        {
        }

        public override bool Compile()
        {
            base.LoadHtml();
            HtmlNode root = Html.DocumentNode;
            HtmlNodeCollection widgetNodes = root.SelectNodes("//*[@silly-widget]");

            if (widgetNodes == null)
            {
                return(true);
            }

            if (base.RootDir == null)
            {
                throw new Exception("Cannot establish root directory");
            }
            
            foreach(HtmlNode widgetNode in widgetNodes)
            {
                string widgetName = widgetNode.Attributes["silly-widget"].Value;

                if (!SillySite.WidgetTable.ContainsKey(widgetName))
                {
                    throw new Exception("Cannot resolve widget '" + widgetName + "'");
                }

                Widgets.Add(widgetNode);
            }

            HtmlNodeCollection css = root.SelectNodes("//link[@rel]");

            if (css != null)
            {
                foreach(HtmlNode cssNode in css)
                {
                    if (String.Compare(cssNode.Attributes["rel"].Value, "stylesheet", true) == 0)
                    {
                        string attrVal = cssNode.Attributes["href"].Value;

                        if (attrVal.Contains("http://"))
                        {
                            continue;
                        }

                        FileInfo cssFile = new FileInfo(base.RootDir.FullName + "/" + attrVal);

                        if (!cssFile.Exists)
                        {
                            throw new Exception("Cannot resolve CSS reference '" + attrVal + "'");
                        }
                    }
                }
            }

            HtmlNodeCollection js = root.SelectNodes("//script[@src]");

            if (js != null)
            {
                foreach(HtmlNode jsNode in js)
                {
                    string attrVal = jsNode.Attributes["src"].Value;

                    if (attrVal.Contains("http://"))
                    {
                        continue;
                    }

                    FileInfo jsFile = new FileInfo(base.RootDir.FullName + "/" + attrVal);

                    if (!jsFile.Exists)
                    {
                        throw new Exception("Cannot resolve javascript reference '" + attrVal + "'");
                    }
                }
            }

            return(true);
        }

        public string Resolve()
        {
            string payload = string.Empty;
            base.LoadHtml();

            HtmlNode root = Html.DocumentNode;

            HtmlNodeCollection widgetNodes = root.SelectNodes("//*[@silly-widget]");

            if (widgetNodes == null)
            {
                return(Html.DocumentNode.OuterHtml);
            }
            
            foreach(HtmlNode widgetNode in widgetNodes)
            {
                string widgetName = widgetNode.Attributes["silly-widget"].Value;
                FileInfo widgetFile = SillySite.WidgetFileFromName(widgetName);

                if (widgetFile != null)
                {
                    SillyWidget widget = new SillyWidget(widgetFile, base.RootDir);
                    HtmlNode widgetRoot = widget.GetRoot();

                    if (widgetRoot != null)
                    {
                        widgetNode.RemoveAllChildren();
                        widgetNode.AppendChild(widgetRoot);
                    }
                }
                else
                {
                    throw new Exception("Cannot resolve widget '" + widgetName + "'");
                }
            }

            payload = Html.DocumentNode.OuterHtml;

            return(payload);
        }
    }
}