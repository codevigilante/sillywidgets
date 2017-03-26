using System;
using System.Collections.Generic;
using System.IO;

namespace silly
{
    public class SillyJs : SillyModel
    {
        public string Where { get; set; }
        public string Source { get; set; }

        public enum Placement { Body, Head }
        public Placement JsPlacement { get; private set; }

        public override bool Compile(string rootDir = "")
        {
            JsPlacement = WhereToPlacement();

            if (!File.Exists(rootDir + Source))
            {
                throw new Exception("Cannot locate javascript file '" + rootDir + Source + "'");
            }

            return (true);
        }

        private Placement WhereToPlacement()
        {
            if (Where == null)
            {
                return (Placement.Head);
            }

            switch(Where.ToLower())
            {
                case "body": return(Placement.Body);
                case "head": return(Placement.Head);
                default: throw new Exception("Javascript placement, " + Where + ", not recognized. Must be 'body' or 'head' only.");
            }
        }
    }
}