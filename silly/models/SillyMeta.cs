using System;
using System.Collections.Generic;
using System.IO;

namespace silly
{
    public class SillyMeta : SillyModel
    {
        public List<string> CSS { get; set; }
        public List<SillyJs> JS { get; set; }

        public override bool Compile(string rootDir = "")
        {
            if (CSS != null)
            {
                foreach(string css in CSS)
                {
                    if (!File.Exists(rootDir + css))
                    {
                        throw new Exception("Cannot find CSS file '" + rootDir + css + "'");
                    }
                }
            }

            if (JS != null)
            {
                foreach(SillyJs js in JS)
                {
                    js.Compile(rootDir);
                }
            }

            return (true);
        }
    }
}