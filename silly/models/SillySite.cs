using System;
using System.Collections.Generic;
using System.IO;

namespace silly
{
    public class SillySite : SillyModel
    {
        public SillyMeta Meta { get; set; }
        public List<SillyWidget> Widgets { get; set; }
        public List<SillyPage> Pages { get; set; }

        private Dictionary<string, SillyWidget> WidgetTable = new Dictionary<string, SillyWidget>();
        private Dictionary<string, SillyPage> PageTable = new Dictionary<string, SillyPage>();

        public override bool Compile(string rootDir = "")
        {
            if (Pages == null)
            {
                throw new Exception("No pages found, nothing to do.");
            }

            Console.Write("Assembling widgets...");

            if (Widgets != null)
            {
                foreach (SillyWidget widget in Widgets)
                {
                    if (WidgetTable.ContainsKey(widget.Name))
                    {
                        throw new Exception ("Duplicate widget definition: " + widget.Name + ". Widget names must be unique.");
                    }

                    widget.Compile(rootDir);

                    WidgetTable.Add(widget.Name, widget);
                }

                Console.WriteLine("done");
            }
            else
            {
                Console.WriteLine("Warning: no widgets found.");
            }

            Console.Write("Organizing routes...");

            foreach (SillyPage page in Pages)
            {
                page.Compile();

                if (PageTable.ContainsKey(page.Route))
                {
                    throw new Exception("Duplicate route detected: " + page.Route + ". Routes must be unique.");
                }               

                if (page.Widgets != null)
                {
                    foreach(string widgetKey in page.Widgets)
                    {
                        if (!WidgetTable.ContainsKey(widgetKey))
                        {
                            throw new Exception("Cannot resolve widget name '" + widgetKey + "'");
                        }
                    }
                }

                PageTable.Add(page.Route, page);
            }

            Console.WriteLine("done");

            Console.Write("Analyzing meta...");

            if (Meta != null)
            {
                Meta.Compile(rootDir);
            }

            Console.WriteLine("done");

            return (true);
        }

        public bool Deploy()
        {
            
            // clean the deploy directory, excluding 'assets'
            // output html

            foreach(SillyPage page in PageTable.Values)
            {
                // make the route in the 'deploy' directory
                // build the page
            }

            
            return(true);
        }
    }
}