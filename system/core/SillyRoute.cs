using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SillyWidgets
{
    public class SillyRoute : List<SillySegment>
    {
        public string Name { get; private set; }
        public string UrlPattern { get; private set; }
        public bool IsValid { get; private set; }
        public string Reason { get; private set; }
        public string Controller { get; private set; }
        public string Method { get; private set; }
        public int VarCount { get; private set; }

        private static Dictionary<string, SegmentTypes> MatchingPatterns = new Dictionary<string, SegmentTypes>()
        {
            { @"^([a-zA-Z\d_\-]+)$", SegmentTypes.HardCoded },
            { @"^(:controller)$", SegmentTypes.Controller },
            { @"^(:method)$", SegmentTypes.Method },
            { @"^{.+}(=.*)?$", SegmentTypes.Variable }
        };

        public SillyRoute(string name, string urlPattern, string controller, string method)
        {
            Name = name;
            UrlPattern = urlPattern;
            IsValid = true;
            VarCount = 0;
            Controller = controller;
            Method = method;

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
                            return(new SillyControllerSegment(Controller));                        
                        case SegmentTypes.Method:
                            return(new SillyMethodSegment(Method));
                        case SegmentTypes.Variable:
                            return(MakeVariable(segment));
                        default:
                            return(null);
                    }
                }
            }

            return(null);
        }

        private SillyVariableSegment MakeVariable(string segment)
        {
            ++VarCount;

            string[] parts = segment.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts == null ||
                parts.Length == 0)
            {
                return(null);
            }

            string name = parts[0].Trim(new char[] { '{', '}' });
            string defaultVal = string.Empty;

            if (parts.Length == 2)
            {
                defaultVal = parts[1].Trim();
            }

            return(new SillyVariableSegment(name, defaultVal));
        }

        private void SetInvalid(string reason)
        {
            IsValid = false;
            Reason = reason;
        }
    }
}