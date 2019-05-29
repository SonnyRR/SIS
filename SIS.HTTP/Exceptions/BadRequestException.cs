namespace SIS.HTTP.Exceptions
{
    using System;

    public class BadRequestException : Exception
    {
        public const string DefaultMessage = "The Request was malformed or contains unsupported elements.";

        /// <summary>
        /// This exception will be thrown when there is an error with the parsing of the HttpRequest
        /// e.g. Unsupported HTTP Protocol, Unsupported HTTP Method, Malformed Request etc. 
        /// </summary>
        public BadRequestException()
            : base(DefaultMessage)
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }
    }
}
