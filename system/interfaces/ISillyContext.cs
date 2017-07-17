using System;

namespace SillyWidgets
{
    public interface ISillyContext
    {
        object GET(string name);
        object POST(string name);
        string Path { get; }
    }
}