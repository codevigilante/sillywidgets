using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SillyWidgets
{
    public enum SillyContentType { Html, Json }

    public interface ISillyView
    {
        SillyContentType ContentType { get; set; }
        string Content { get; set; }    
        string Name { get; }
        string UrlPrefix { get; }
        bool AcceptsUrlParameters { get; }

        string Render(ISillyContext context, string[] urlParams);    
    }

    /*public class SillyUrlParam : Tuple<string, bool>
    {
        public string Name 
        { 
            get { return(base.Item1); }
        }
        public string DefaultValue { get; private set; }
        public bool Optional 
        { 
            get { return(base.Item2); }
        }

        public SillyUrlParam(string name, bool optional, string defaultValue = "")
            : base(name, optional)
        {
            DefaultValue = defaultValue;
        }
    }*/
}