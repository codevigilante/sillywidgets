using System;
using System.IO;

namespace silly
{
    public abstract class SillyModel
    {
        public DirectoryInfo RootDir { get; private set; }

        public SillyModel(DirectoryInfo root = null)
        {
            RootDir = root;
        }

        public abstract bool Compile();
    }
}