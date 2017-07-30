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

        public SillyRouteDetails(string controller = "", string method = "")
        {
            Controller = controller;
            Method = method;
        }

        public bool HasVariable(string name)
        {
            return(ContainsKey(name.ToLower()));
        }
    }

    public class SillyRoute : List<SillySegment>
    {
        public string Name { get; private set; }
        public string UrlPattern { get; private set; }
        public bool IsValid { get; private set; }
        public string Reason { get; private set; }
        public SillyRouteDetails Settings { get; private set; }
        public int VarCount { get; private set; }

        private static Dictionary<string, SegmentTypes> MatchingPatterns = new Dictionary<string, SegmentTypes>()
        {
            { @"^([a-zA-Z\d_\-]+)$", SegmentTypes.HardCoded },
            { @"^(:controller)$", SegmentTypes.Controller },
            { @"^(:method)$", SegmentTypes.Method },
            { @"^({.+})$", SegmentTypes.Variable }
        };

        public SillyRoute(string name, string urlPattern, SillyRouteDetails details = null)
        {
            Name = name;
            UrlPattern = urlPattern;
            IsValid = true;
            VarCount = 0;

            Settings = (details == null) ? new SillyRouteDetails() : details;

            ParseUrlPattern();
        }

        private void ParseUrlPattern()
        {
            if (String.IsNullOrEmpty(UrlPattern) || String.IsNullOrWhiteSpace(UrlPattern))
            {
                SetInvalid("Url pattern is null, empty, or nothing but whitespace");

                return;
            }

            if (UrlPattern[0] != '/')
            {
                SetInvalid("Url pattern must open with '/'");

                return;
            }

            string[] matches = UrlPattern.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (matches == null || matches.Length == 0)
            {
                SillySegment rootSegment = new SillyHardCodedSegment();

                Add(rootSegment);

                return;
            }

            bool controllerSet = false, methodSet = false;

            foreach(string match in matches)
            {
                if (String.Compare("/", match, true) == 0)
                {
                    continue;
                }

                SillySegment segment = ProduceFromSegment(match);

                if (segment == null)
                {
                    SetInvalid("Cannot find a suitable match for Url segment '" + match + "'");

                    return;
                }

                if (segment.Type == SegmentTypes.Controller)
                {
                    if (controllerSet)
                    {
                        SetInvalid("Cannot define multiple :controller segments in a Url pattern");

                        return;
                    }

                    controllerSet = true;

                    if (methodSet)
                    {
                        SetInvalid("Cannot define :method before :controller in a Url pattern");

                        return;
                    }

                    if (VarCount > 0)
                    {
                        SetInvalid("Cannot define {vars} before :controller in a Url pattern");

                        return;
                    }
                }

                if (segment.Type == SegmentTypes.Method)
                {
                    if (methodSet)
                    {
                        SetInvalid("Cannot define multiple :method segments in a Url pattern");

                        return;
                    }

                    methodSet = true;

                    if (VarCount > 0)
                    {
                        SetInvalid("Cannot define {vars} before :method in a Url pattern");

                        return;
                    }
                }

                Add(segment);
            }
        }

        private SillySegment ProduceFromSegment(string segment)
        {
            Dictionary<string, SegmentTypes>.KeyCollection patterns = MatchingPatterns.Keys;

            foreach(string pattern in patterns)
            {
                Match match = Regex.Match(segment, pattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    switch(MatchingPatterns[pattern])
                    {
                        case SegmentTypes.HardCoded:
                            return(new SillyHardCodedSegment(segment));
                        case SegmentTypes.Controller:
                            return(new SillyControllerSegment(Settings.Controller));                        
                        case SegmentTypes.Method:
                            return(new SillyMethodSegment(Settings.Method));
                        case SegmentTypes.Variable:
                            ++VarCount;
                            return(new SillyVariableSegment(segment.Trim(new char[] { '{', '}' })));
                        default:
                            return(null);
                    }
                }
            }

            return(null);
        }

        private void SetInvalid(string reason)
        {
            IsValid = false;
            Reason = reason;
        }
    }
}