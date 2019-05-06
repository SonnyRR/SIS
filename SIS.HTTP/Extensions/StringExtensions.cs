namespace SIS.HTTP.Extensions
{
    using System.Globalization;

    public static class StringExtensions
    {
        /// <summary>
        /// Capitalize the specified string.
        /// </summary>
        /// <returns>The capitalized <see cref="System.String"/> value.</returns>
        /// <param name="string">String.</param>
        public static string Capitalize(this string @string)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(@string);
        }
    }
}
