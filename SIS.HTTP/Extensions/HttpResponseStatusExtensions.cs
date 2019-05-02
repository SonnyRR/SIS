namespace SIS.HTTP.Extensions
{
    using System.ComponentModel;
    using System.Reflection;
    using SIS.HTTP.Enums;

    public static class HttpResponseStatusExtensions
    {
        public static string GetResponseLine(this HttpResponseStatusCode value)
        {
            FieldInfo fieldInfo = value
                .GetType()
                .GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;

            return value.ToString();
        }
    }
}
