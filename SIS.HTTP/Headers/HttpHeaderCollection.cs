namespace SIS.HTTP.Headers
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;

    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Common;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly IDictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, HttpHeader>();
        }

        /// <summary>
        /// Wrapper for IDictionary, it adds the header as a value and its name as a key.
        /// </summary>
        /// <param name="header">Header.</param>
        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));
            this.headers.Add(header.Key, header);
        }

        /// <summary>
        /// Wrapper for IDictonary, it checks if a header with a given name is already present.
        /// </summary>
        /// <returns><c>true</c>, if header is present, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        public bool ContainsHeader(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));

            return this.headers.ContainsKey(key);
        }

        /// <summary>
        /// Wrapper for IDictionary.
        /// </summary>
        /// <returns>The header or null if not present.</returns>
        /// <param name="key">Key.</param>
        public HttpHeader GetHeader(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));

            return this.headers
                .FirstOrDefault(kvp => kvp.Key == key)
                .Value;
        }

        /// <summary>
        /// Appends headers into a single string with correct formatting.
        /// </summary>
        /// <returns>A formated <see cref="System.String"/> of inner headers.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var kvp in this.headers)
            {
                builder.Append($"{kvp.Value.ToString()}{GlobalConstants.HttpNewLine}");
            }

            return builder.ToString().TrimEnd();
        }
    }
}
