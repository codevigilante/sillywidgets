using System;

namespace SillyWidgets
{
    public class SillyView : ISillyView
    {
        public SillyContentType ContentType { get; set; }
        public string Content { get; set; }

        public SillyView()
        {
            ContentType = SillyContentType.Html;
            Content = string.Empty;
        }  
    }
}