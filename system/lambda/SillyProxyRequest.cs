using System;
using System.Dynamic;
using Newtonsoft.Json;

namespace SillyWidgets
{
    public class SillyProxyRequest
    {
        public string resource { get; set; }
        public string path { get; set; }
        public string httpMethod { get; set; }
        public dynamic queryStringParameters { get; set; }
        public dynamic headers { get; set; }
        public string body { get; set; } // this needs to be parsed for POST vars

        public SillyProxyRequest()
        {
            queryStringParameters = new ExpandoObject();
            headers = new ExpandoObject();
        }
    }
}