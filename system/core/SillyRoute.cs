using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SillyWidgets
{
    public class SillyRouteDetails : Dictionary<string, object>
    {
        public string Controller { get; private set; }
        public string Method { get; private set; }

        public SillyRouteDetails(string controller, string method)
        {
            Controller = controller;
            Method = method;
        }
    }

    public class SillyRoute
    {
        public string Name { get; private set; }
        public string UrlPattern { get; private set; }
        public SillyRouteDetails Defaults { get; set; }

        public SillyRoute(string name, string urlPattern)
        {
            Name = name;
            UrlPattern = urlPattern;
        }

        public static bool IsValidUrlPattern(string urlPattern)
        {
            string splitPattern = @"(\/)";

            string[] matches = Regex.Split(urlPattern, splitPattern, RegexOptions.IgnoreCase).Where(s => s != string.Empty).ToArray();

            if (matches == null || matches.Length == 0)
            {
                return(false);
            }

            if (String.Compare("/", matches[0], true) != 0)
            {
                return(false);
            }

            string matchPattern = @"^((:controller)|(:method)|([a-zA-Z\d_\-]+)|({.+}))$";

            foreach(string match in matches)
            {
                if (String.Compare("/", match, true) == 0)
                {
                    continue;
                }

                Match result = Regex.Match(match, matchPattern, RegexOptions.IgnoreCase);

                if (!result.Success)
                {
                    return(false);
                }
            }
            
            return(true);
        }
    }
}