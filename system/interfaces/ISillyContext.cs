using System;

namespace SillyWidgets
{
    public enum SupportedHttpMethods { All, Get, Post, Unsupported }

    public interface ISillyContext
    {
        object GET(string name);
        object POST(string name);
        string Path { get; }
        SupportedHttpMethods HttpMethod { get; }
    }
}