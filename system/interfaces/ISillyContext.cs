using System;

namespace SillyWidgets
{
    public enum SupportedHttpMethods { Get, Post, Unsupported }

    public interface ISillyContext
    {
        bool GET(string name, out object value);
        object POST(string name);
        string Path { get; }
        SupportedHttpMethods HttpMethod { get; }
    }
}