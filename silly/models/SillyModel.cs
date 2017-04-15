using System;
using System.IO;

namespace silly
{
    public abstract class SillyModel
    {
        public SillyModel()
        {

        }

        public abstract bool Compile(SiteConfig siteConfig = null);
    }
}