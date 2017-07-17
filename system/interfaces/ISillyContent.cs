using System;

namespace SillyWidgets
{
    public enum SillyContentType { Html, Json }

    public interface ISillyContent
    {
        SillyContentType ContentType { get; set; }
        string Content { get; set; }        
    }
}