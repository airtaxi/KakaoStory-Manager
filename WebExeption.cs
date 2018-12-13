using System;
using System.Runtime.Serialization;

namespace KSP_WPF
{
    [Serializable]
    internal class WebExeption : Exception
    {
        public WebExeption()
        {
        }

        public WebExeption(string message) : base(message)
        {
        }

        public WebExeption(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebExeption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}