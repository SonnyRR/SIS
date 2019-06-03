namespace SIS.Common
{
    using System;

    public static class ValidationExtensions
    {
        public static void ThrowIfNull(this object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentException(name);
            }
        }

        public static void ThrowIfNullOrEmpty(this string text, string name)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException($"{name} cannot be null or empty.");
            }
        }
    }
}
