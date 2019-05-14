namespace SIS.HTTP.Common
{
    using System;
    public class CoreValidator
    {
        public static void ThrowIfNull(object obj, string name)
        {
            if (obj == null)
                throw new ArgumentException(name);
        }

        public static void ThrowIfNullOrEmpty(string text, string name)
        {
            // FIXME
            // Maybe change string.IsNullOrWhiteSpeace() to IsNullOrEmpty()
            // depending on the desired behaviour.
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException($"{name} cannot be null or empty.");
        }
    }
}
