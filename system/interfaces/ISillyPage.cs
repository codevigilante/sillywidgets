using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SillyWidgets
{
    public enum SillyContentType { Html, Json }

    public interface ISillyPage
    {
        SillyContentType ContentType { get; set; }
        string Content { get; set; }    
        string Name { get; }
        string UrlPrefix { get; }
        bool AcceptsUrlParameters { get; }

        string Render(); 
        bool Accept(ISillyContext context, string[] urlParams);   
    }
}