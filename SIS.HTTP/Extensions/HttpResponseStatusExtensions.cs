namespace SIS.HTTP.Extensions
{
    using System.ComponentModel;
    using System.Reflection;
    using SIS.HTTP.Enums;

    public static class HttpResponseStatusExtensions
    {
        /// <summary>
        /// Gets the response line if the current status code.
        /// </summary>
        /// <returns>The response line.</returns>
        /// <example>400 => 400 Bad Request</example>
        /// <param name="statusCode">HTTP Status code.</param>
        public static string GetResponseLine(this HttpResponseStatusCode statusCode)
        {
            FieldInfo fieldInfo = statusCode
                .GetType()
                .GetField(statusCode.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;

            return statusCode.ToString();
        }
    }
}
