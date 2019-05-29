namespace SIS.WebServer.Routing
{
    using System;

    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;

    public interface IServerRoutingTable
    {
        void Add(HttpRequestMethod method, string path, Func<IHttpRequest, IHttpResponse> @delegate);

        bool Contains(HttpRequestMethod method, string path);

        Func<IHttpRequest, IHttpResponse> Get(HttpRequestMethod method, string path);
    }
}
