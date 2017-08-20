using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SillyWidgets
{
    public class SillyView : ISillyView
    {
        public SillyContentType ContentType { get; set; }
        public string Content { get; set; }
        public SillyResource ViewFile { get; set; }
        public List<SillyResource> WidgetFiles { get; private set; }
        //Dictionary<string, SillyWidget> Widgets { get; }

        public SillyView()
        {
            ContentType = SillyContentType.Html;
            Content = string.Empty;
        }

        public async Task<bool> Load()
        {
            return(false);
        }

        public bool BindText(string name, string value)
        {
            return(true);
        }
    }
}