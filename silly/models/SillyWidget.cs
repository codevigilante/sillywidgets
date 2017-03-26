using System;
using System.IO;

namespace silly
{
    public class SillyWidget : SillyModel
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public Object Data { get; set; }
        public Object Resources { get; set; }

        public override bool Compile(string rootDir = "")
        {
            Source = rootDir + Source;

            if (!File.Exists(Source))
            {
                throw new Exception("Source for widget '" + Name + "' can't be found: " + Source);
            }

            return(true);
        }
    }
}