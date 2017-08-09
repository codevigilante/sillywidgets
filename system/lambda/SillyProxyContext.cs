using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SillyWidgets
{
    public class SillyProxyContext : ISillyContext
    {

        public string Path { get; private set; }
        public SillyProxyRequest OriginalRequest { get; private set; }
        public SupportedHttpMethods HttpMethod { get; private set; }

        public SillyProxyContext()
        {
            OriginalRequest = null;
            Path = string.Empty;
            HttpMethod = SupportedHttpMethods.Unsupported;
            _get = null;
        }

        public SillyProxyContext(SillyProxyRequest request)
            : this()
        {
            OriginalRequest = request;

            if (request == null)
            {
                return;
            }

            HttpMethod = StringToHttpMethod(request.httpMethod);

            _get = request.queryStringParameters;
            // POST is either name=value pairs or json, depending on the header's content-type value
            // x-www-form-urlencoded = name=value pairs
            // application/json = json

            Path = request.path;
        }

        private IDictionary<string, object> _get;

        public object GET(string name)
        {
            return(null);
        }

        //private IDictionary<string, object> _post;

        public object POST(string name)
        {
            return(null);
        }

        private SupportedHttpMethods StringToHttpMethod(string httpMethod)
        {
            if (String.IsNullOrEmpty(httpMethod))
            {
                return(SupportedHttpMethods.Unsupported);
            }

            if (String.Compare(httpMethod, "GET", true) == 0)
            {
                return(SupportedHttpMethods.Get);
            }
            
            if (String.Compare(httpMethod, "Post", true) == 0)
            {
                return(SupportedHttpMethods.Post);
            }

            return(SupportedHttpMethods.Unsupported);
        }
        
    }
}