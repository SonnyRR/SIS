namespace SIS.HTTP.Exceptions
{
    using System;

    public class BadRequestException : Exception
    {
        /// <summary>
        /// This exception will be thrown when there is an error with the parsing of the HttpRequest
        /// e.g. Unsupported HTTP Protocol, Unsupported HTTP Method, Malformed Request etc. 
        /// </summary>
        public BadRequestException()
            :base("The Request was malformed or contains unsupported elements.")
        {
        }
    }
}
