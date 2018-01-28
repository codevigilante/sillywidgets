using System;
using System.Collections.Generic;

namespace SillyWidgets
{
    public enum SupportedHttpMethods { Get, Post, Unsupported }

    public interface ISillyContext
    {
        bool GET(string name, out object value);
        bool POST(string name, out object value);
        string Path { get; }
        SupportedHttpMethods HttpMethod { get; }
        bool HEADER(string name, out string value);
    }
}