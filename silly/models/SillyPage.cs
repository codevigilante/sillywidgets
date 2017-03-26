using System;
using System.Collections.Generic;

namespace silly
{
    public class SillyPage : SillyModel
    {
        public string Route { get; set; }
        public string Target { get; set; }
        public List<string> Widgets { get; set; }

        public override bool Compile(string rootDir = "")
        {
            Route = Route.ToLower();

            if (!Uri.IsWellFormedUriString(Route, UriKind.Relative))
            {
                throw new Exception ("The route '" + Route + "' is not well formed.");
            }

            return(true);
        }
    }
}