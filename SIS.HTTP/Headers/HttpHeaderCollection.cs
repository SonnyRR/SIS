namespace SIS.HTTP.Headers
{
    using System;
    using System.Collections.Generic;

    using SIS.HTTP.Headers.Contracts;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly IDictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, HttpHeader>();
        }

        public void Add(HttpHeader header)
        {
            throw new NotImplementedException();
        }

        public bool ContainsHeader(string key)
        {
            throw new NotImplementedException();
        }

        public HttpHeader GetHeader(string key)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
