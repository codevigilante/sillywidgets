using System;

namespace SillyWidgets
{
    public enum SillyContentType { Html, Json }

    public interface ISillyView
    {
        SillyContentType ContentType { get; set; }
        string Content { get; set; }        
    }
}