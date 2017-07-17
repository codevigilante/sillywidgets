using System;

namespace SillyWidgets
{
    public abstract class SillyHandler
    {
        public SillyOptions Options { get; private set; }

        public SillyHandler()
        {
            Options = new SillyOptions();    
        }
    }
}