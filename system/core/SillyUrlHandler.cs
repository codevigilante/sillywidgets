using System;

namespace SillyWidgets
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class SillyUrlHandlerAttribute : Attribute
    {
        public string Name { get; set; }
        public string UrlPrefix { get; set; }
        public bool IsIndex { get; set; }
        public Type HandlerType { get; set; }
        public SupportedHttpMethods HttpMethods { get; set; }

        public SillyUrlHandlerAttribute()
        {
            HttpMethods = SupportedHttpMethods.All;
            UrlPrefix = string.Empty;
            IsIndex = false;
            Name = string.Empty;
        }

        public SillyUrlHandlerAttribute(string prefix)
            : this()
        {
            UrlPrefix = prefix;
        }

        public SillyUrlHandlerAttribute(bool isIndex)
            : this()
        {
            IsIndex = isIndex;
        }

        public SillyUrlHandlerAttribute(Type type, string prefix = "")
            : this(prefix)
        {
            HandlerType = type;
        }
        
    }
}