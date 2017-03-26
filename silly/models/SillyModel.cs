using System;

namespace silly
{
    public abstract class SillyModel
    {
        public SillyModel()
        {

        }

        public abstract bool Compile(string rootDir = "");
    }
}