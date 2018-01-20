using System;
using System.Text;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SillyWidgets.Utilities.Server
{
    public class SillyHttpRequestParser : SillyProxyRequest
    {
        public enum Methods { GET, POST }
        public enum Headers { Host, Connection, CacheControl, UpgradeInsecureRequests, UserAgent, Accept, AcceptEncoding, AcceptLanguage, ContentType, ContentLength, Unknown }
        public Methods Method { get; private set; }
        public string URL { get; private set; }
        public string Version { get; private set; }
        public bool IsInvalid { get; private set; }
        public string InvalidReason { get; private set; }
        public bool RequestIsFile { get; private set; }
        public bool Ignore { get; private set; }

        private Dictionary<string, Methods> SupportedMethods = new Dictionary<string, Methods>()
        {
            { "GET", Methods.GET },
            { "POST", Methods.POST }
        };

        private Dictionary<Headers, string> HeaderData = new Dictionary<Headers, string>()
        {
            { Headers.Accept, "" },
            { Headers.AcceptEncoding, "" },
            { Headers.AcceptLanguage, "" },
            { Headers.CacheControl, "" },
            { Headers.Connection, "" },
            { Headers.Host, "" },
            { Headers.Unknown, "" },
            { Headers.UpgradeInsecureRequests, "" },
            { Headers.UserAgent, "" },
            { Headers.ContentType, "" },
            { Headers.ContentLength, "" }
        };

        public SillyHttpRequestParser()
            : base()
        {
        }

        public SillyHttpRequestParser(string request)
            : this()
        {
            Parse(request);
        }

        public void Parse(string request)
        {
            if (String.IsNullOrEmpty(request) || String.IsNullOrWhiteSpace(request))
            {
                Ignore = true;

                return;
            }

            Ignore = false;
            IsInvalid = false;
            RequestIsFile = false;

            string[] requestParts = Regex.Split(request, @"(\r\n\r\n)");

            if (requestParts.Length == 0)
            {
                Ignore = true;

                return;
            } 

            StringBuilder body = new StringBuilder();

            for(int i = 0; i < requestParts.Length; ++i)
            {
                string part = requestParts[i].Trim();

                if (i == 0)
                {
                    ParseHeader(part);

                    continue;
                }

                if (String.IsNullOrEmpty(part) &&
                    i == 1)
                {
                    // skip the blank line between the header and the body
                    continue;
                }

                body.Append(part);
            } 

            if (body.Length > 0)
            {
                base.body = body.ToString();
            }        
        }

        public string GetHeader(Headers param)
        {
            return(HeaderData[param]);
        }

        public string QueryToString()
        {
            IDictionary<string, object> queryVals = base.queryStringParameters;

            return(DictionaryToString(queryVals));
        }

        public string HeadersToString()
        {
            IDictionary<string, object> headerVals = base.headers;

            return(DictionaryToString(headerVals));
        }

        private string DictionaryToString(IDictionary<string, object> dic)
        {
            string str = string.Empty;

            foreach(KeyValuePair<string, object> val in dic)
            {
                str += val.Key + "=" + val.Value.ToString() + ";";
            }

            return(str);
        }

        private void ParseRequest(string requestLine)
        {
            string[] requestParts = requestLine.Split(' ');

            if (requestParts.Length >= 3)
            {
                string method = requestParts[0];

                if (SupportedMethods.ContainsKey(method))
                {
                    Method = SupportedMethods[method];
                    base.httpMethod = method;
                }
                else
                {
                    SetInvalid("Unsupported method: " + method);
                }

                URL = requestParts[1];
                Version = requestParts[2];

                WhatIsURL();
                SeparatePathFromQuery();
            }
            else
            {
                SetInvalid("Not enough request parameters");
            }            
        }

        private void ParseHeader(string header)
        {
            string[] headerLines = header.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (headerLines.Length == 0)
            {
                SetInvalid("Header appears to be empty");

                return;
            }

            Regex headerName = new Regex(@"^(.*?:)");
            Regex headerValue = new Regex(@"(:\s.*)");
            IDictionary<string, object> proxyHeaders = base.headers;

            for (int i = 0; i < headerLines.Length; ++i)
            {
                string line = headerLines[i];

                if (i == 0)
                {
                    ParseRequest(line);

                    continue;
                }

                Match match = headerName.Match(line);
                string name = match.Value.Trim(':');

                match = headerValue.Match(line);
                string value = match.Value.Trim(new char[] { ':', ' ' });

                if (!String.IsNullOrEmpty(name))
                {
                    HeaderData[StringToHeaderName(name)] = value;
                    proxyHeaders[name] = value;
                }
            }
        }

        private void WhatIsURL()
        {
            int index = URL.IndexOf('.');

            if (index >= 0)
            {
                RequestIsFile = true;
            }
        }

        private void SeparatePathFromQuery()
        {
            string[] fragments = URL.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);

            if (fragments != null && fragments.Length > 0)
            {
                base.path = fragments[0];
            }

            IDictionary<string, object> queryParams = base.queryStringParameters;

            if (fragments.Length > 1)
            {
                string[] nameValues = fragments[1].Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string nameValue in nameValues)
                {
                    string[] pair = nameValue.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    string name = pair[0];
                    string value = (pair.Length > 1) ? pair[1] : string.Empty;

                    if (String.IsNullOrEmpty(name))
                    {
                        throw new Exception("Invalid query parameter: is empty or null");
                    }

                    queryParams[name] = value;
                }
            }
        }

        private void SetInvalid(string reason)
        {
            IsInvalid = true;
            InvalidReason = reason;
        }

        private Headers StringToHeaderName(string val)
        {
            switch(val.ToLower())
            {
                case "host":
                    return(Headers.Host);
                case "connection":
                    return(Headers.Connection);
                case "cache-control":
                    return(Headers.CacheControl);
                case "upgrade-insecure-requests":
                    return(Headers.UpgradeInsecureRequests);
                case "user-agent":
                    return(Headers.UserAgent);
                case "accept":
                    return(Headers.Accept);
                case "accept-encoding":
                    return(Headers.AcceptEncoding);
                case "accept-language":
                    return(Headers.AcceptLanguage);
                case "content-type":
                    return(Headers.ContentType);
                case "content-length":
                    return(Headers.ContentLength);
                default:
                    return(Headers.Unknown);
            }
        }
    }
}