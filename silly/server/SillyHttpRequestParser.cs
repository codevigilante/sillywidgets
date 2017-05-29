using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace silly
{
    public class SillyHttpRequestParser
    {
        public enum Methods { GET, POST }
        public enum Headers { Host, Connection, CacheControl, UpgradeInsecureRequests, UserAgent, Accept, AcceptEncoding, AcceptLanguage, Unknown }
        public Methods Method { get; private set; }
        public string URL { get; private set; }
        public string Version { get; private set; }
        public bool IsInvalid { get; private set; }
        public string InvalidReason { get; private set; }
        public bool RequestIsFile { get; private set; }
        public bool Ignore { get; private set; }

        private string[] RequestLines = null;
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
            { Headers.UserAgent, "" }
        };

        public SillyHttpRequestParser(string request)
        {
            if (String.IsNullOrEmpty(request) || String.IsNullOrWhiteSpace(request))
            {
                Ignore = true;

                return;
            }

            Ignore = false;
            IsInvalid = false;
            RequestIsFile = false;
            RequestLines = request.Split(new char[] { '\n', '\r' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (RequestLines.Length > 1)
            {
                ParseRequest();   
                ParseHeader();
            }
            else if (RequestLines.Length == 0)
            {
                Ignore = true;
            }
            else
            {
                SetInvalid("Request is incomplete");
            }
        }

        public string GetHeader(Headers param)
        {
            return(HeaderData[param]);
        }

        private void ParseRequest()
        {
            string[] requestParts = RequestLines[0].Split(' ');

            if (requestParts.Length >= 3)
            {
                string method = requestParts[0];

                if (SupportedMethods.ContainsKey(method))
                {
                    Method = SupportedMethods[method];
                }
                else
                {
                    SetInvalid("Unsupported method: " + method);
                }

                URL = requestParts[1];
                Version = requestParts[2];

                WhatIsURL();
            }
            else
            {
                SetInvalid("Not enough request parameters");
            }            
        }

        private void ParseHeader()
        {
            Regex headerName = new Regex(@"^(.*?:)");
            Regex headerValue = new Regex(@"(:\s.*)");

            foreach(string line in RequestLines)
            {
                Match match = headerName.Match(line);
                string name = match.Value.Trim(':');

                match = headerValue.Match(line);
                string value = match.Value.Trim(new char[] { ':', ' ' });
                
                HeaderData[StringToHeaderName(name)] = value;
            }
        }

        private void WhatIsURL()
        {
            int index = URL.IndexOf('.');

            if (index >= 0)
            {
                RequestIsFile = true;
            }
            else if (!URL.EndsWith("/"))
            {
                URL += "/";
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
                default:
                    return(Headers.Unknown);
            }
        }
    }
}