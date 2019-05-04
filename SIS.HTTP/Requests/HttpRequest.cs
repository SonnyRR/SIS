namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Requests.Contracts;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestAsString)
        {
            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();

            this.ParseRequest(requestAsString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public HttpHeaderCollection Headers { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        private void ParseRequest(string requestString)
        {
            string[] request = requestString
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string[] requestLine = request[0]
                 .Trim()
                 .Split();

            if (this.IsRequestLineValid(requestLine))
                throw new BadRequestException();

            this.ParseRequestMethod(method: requestLine[0]);
            this.ParseRequestUrl(url: requestLine[1]);
            this.ParseRequestPath(url: requestLine[2]);

        }

        private bool IsRequestLineValid(string[] requestLineArgs)
        {
            bool isValid = true;

            var method = CultureInfo
                .InvariantCulture
                .TextInfo
                .ToTitleCase(requestLineArgs[0]);

            var route = requestLineArgs[1];
            var protocol = requestLineArgs[2];

            if (!Enum.TryParse<HttpRequestMethod>(method, out _))
                isValid = false;

            // NOTE
            // Maybe needed to change checking method.
            else if (!Uri.IsWellFormedUriString(route, UriKind.Absolute))
                isValid = false;

            else if (protocol != "HTTP/1.1")
                isValid = false;

            return isValid;
        }

        private void ParseRequestMethod(string method)
        {
            this.RequestMethod = (HttpRequestMethod)Enum.Parse(typeof(HttpRequestMethod), method);
        }

        private void ParseRequestUrl(string url)
        {
            this.Url = url;
        }

        private void ParseRequestPath(string url)
        {
            var urlSplitted = url
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            urlSplitted.RemoveAt(0);

            this.Path = string.Join("", urlSplitted);
        }

        private void ParseHeaders(ICollection<string> splittedRequest)
        {
            foreach (var headerPair in splittedRequest.Skip(1))
            {
                if (headerPair == Environment.NewLine)
                    break;

                var kvp = headerPair.Split(": ", StringSplitOptions.RemoveEmptyEntries);

                // TODO
                // CHECK
                // Invalid data may be passed, check for invalid kvp's.
                var currentHeader = new HttpHeader(kvp[0], kvp[1]);

                this.Headers.Add(currentHeader);
            }

            if (this.Headers.ContainsHeader("Host"))
                throw new BadRequestException();
        }
    }

}
