namespace SIS.HTTP.Exceptions
{
    using System;

    public class InternalServerErrorException : Exception
    {
        /// <summary>
        /// This exception will be thrown whenever there is an error that the Server was not suppoused to encounter.
        /// </summary>
        public InternalServerErrorException()
            : base("The Server has encountered an error.")
        {
        }
    }
}
