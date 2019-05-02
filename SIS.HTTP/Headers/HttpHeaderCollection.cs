namespace SIS.HTTP.Headers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using SIS.HTTP.Headers.Contracts;
    using System.Text;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly IDictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, HttpHeader>();
        }

        public void Add(HttpHeader header)
        {
            // FIXME
            // Probably should check for duplicate keys.
            this.headers.Add(header.Key, header);
        }

        public bool ContainsHeader(string key)
        {
            return this.headers.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            return this.headers
                .FirstOrDefault(kvp => kvp.Key == key)
                .Value;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var kvp in this.headers)
            {
                builder.AppendLine(kvp.Value.ToString());
            }

            return builder.ToString().TrimEnd();
        }
    }
}
