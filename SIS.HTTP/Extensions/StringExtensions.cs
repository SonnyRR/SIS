namespace SIS.HTTP.Extensions
{
    using System.Globalization;

    public static class StringExtensions
    {
        public static string Capitalize(this string @string)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(@string);
        }
    }
}
