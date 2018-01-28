using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SillyWidgets.Gizmos
{
    public static class WebUtilityGizmo
    {
        private static Regex EncodedSymbols = new Regex(@"(%[\d|a-fA-F]{2})", RegexOptions.Compiled);

        private static Dictionary<string, string> SymbolToText = new Dictionary<string, string>()
        {
            { "%20", " " },{ "%21", "!" },{ "%22", "\"" },{ "%23", "#" },{ "%24", "$" },{ "%25", "%" },{ "%26", "&" },{ "%27", "'" },
            { "%28", "(" },{ "%29", ")" },{ "%2A", "*" },{ "%2B", "+" },{ "%2C", "," },{ "%2D", "-" },{ "%2E", "." },{ "%2F", "/" },
            { "%3A", ":" },{ "%3B", ";" },{ "%3C", "<" },{ "%3D", "=" },{ "%3E", ">" },{ "%3F", "?" },{ "%40", "@" },{ "%5B", "[" },
            { "%5C", "\\" },{ "%5D", "]" },{ "%5E", "^" },{ "%5F", "_" },{ "%60", "`" },{ "%7B", "{" },{ "%7C", "|" },{ "%7D", "}" },
            { "%7E", "~" },{ "%7F", "" }
        };

        public static string UrlDecode(string encodedString)
        {
            StringBuilder decodedStr = new StringBuilder(encodedString);

            decodedStr.Replace("+", " ");

            Match symbolMatch = EncodedSymbols.Match(decodedStr.ToString());

            while (symbolMatch.Success)
            {
                string text = string.Empty;

                if (SymbolToText.TryGetValue(symbolMatch.Value.ToUpper(), out text))
                {
                    decodedStr.Replace(symbolMatch.Value, text);
                }
                
                symbolMatch = EncodedSymbols.Match(decodedStr.ToString());
            }

            return(decodedStr.ToString());
        }
    }
}