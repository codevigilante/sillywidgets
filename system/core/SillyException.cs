using System;

namespace SillyWidgets
{
    public class SillyException : Exception
    {
        public SillyHttpStatusCode StatusCode { get; private set; }

        public SillyException(SillyHttpStatusCode statusCode, string details)
            : base(details)
        {
            StatusCode = statusCode;
        }
        
    }
}