namespace SIS.HTTP.Headers.Contracts
{
    using System;

    public interface IHttpHeaderCollection
    {
        void AddHeader(HttpHeader header);

        bool ContainsHeader(string key);

        HttpHeader GetHeader(string key);

    }
}
