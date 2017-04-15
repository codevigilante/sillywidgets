using System;
using System.IO;
using HtmlAgilityPack;

namespace silly
{
    public class SillyRoute : SillyWidget
    {
        public Object Variables { get; set; }

        public SillyRoute(FileInfo widgetFile, string prefix = "")
            : base(widgetFile, prefix)
        {
        }

        public override bool Compile(SiteConfig config = null)
        {
            base.LoadHtml();

            HtmlNode root = Html.DocumentNode;

            HtmlNodeCollection widgetNodes = root.SelectNodes("//*[@silly-widget]");

            if (widgetNodes == null)
            {
                return(true);
            }
            
            foreach(HtmlNode widgetNode in widgetNodes)
            {
                string widgetName = widgetNode.Attributes["silly-widget"].Value;

                if (!SillySite.WidgetTable.ContainsKey(widgetName))
                {
                    throw new Exception("Cannot resolve widget '" + widgetName + "'");
                }
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

                        if (!SillySite.Assets.ContainsKey(attrVal))
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

                    if (!SillySite.Assets.ContainsKey(attrVal))
                    {
                        throw new Exception("Cannot resolve javascript reference '" + attrVal + "'");
                    }
                }
            }

            return(true);
        }
    }
}