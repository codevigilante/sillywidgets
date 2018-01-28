using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SillyWidgets
{
    public class SillyProxyRequest
    {
        public string resource { get; set; }
        public string path { get; set; }
        public string httpMethod { get; set; }
        public Dictionary<string, object> queryStringParameters { get; set; }
        public Dictionary<string, object> headers { get; set; }
        public string body { get; set; }

        public SillyProxyRequest()
        {
            queryStringParameters = new Dictionary<string, object>();
            headers = new Dictionary<string, object>();
        }

        public override string ToString()
        {
            string queryVars = string.Empty;

            foreach(KeyValuePair<string, object> param in queryStringParameters)
            {
                queryVars += param.Key + "=" + param.Value.ToString();
            }

            string header = string.Empty;

            foreach(KeyValuePair<string, object> param in headers)
            {
                header += param.Key + "=" + param.Value.ToString();
            }

            return("-----> M:" + httpMethod + " P:" + path + " Q:" + queryVars + " B:" + body + " END----->");
        }
    }
}