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

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();
            this.ParseHeaders(request);

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

        private void ParseRequestMethod(string[] requestLine)
        {
            var method = requestLine[0];
            this.RequestMethod = (HttpRequestMethod)Enum.Parse(typeof(HttpRequestMethod), method);
        }

        private void ParseRequestUrl(string[] requestLine)
        {
            this.Url = requestLine[1];
        }

        private void ParseRequestPath()
        {
            var urlSplitted = this.Url
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

        private bool IsRequestQueryStringValid(string queryString)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(queryString))
                isValid = false;

            var splittedQuery = queryString
                .Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

            if (splittedQuery.Length == 1)
                isValid = false;

            return isValid;
        }

        private void ParseQueryParameters()
        {
            var splittedUrlTokens = this.Url
                .Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (splittedUrlTokens.Length > 1)
            {
                var queryString = splittedUrlTokens[1];

                var isValid = IsRequestQueryStringValid(queryString);

                if (!isValid)
                    throw new BadRequestException();

                var queryPairs = queryString
                    .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in queryPairs)
                {
                    var pairTokens = pair.Split("=", StringSplitOptions.RemoveEmptyEntries);

                    var key = pairTokens[0];
                    var value = pairTokens[1];

                    if (!this.QueryData.ContainsKey(key))
                        this.QueryData[key] = value;
                }
            }
        }

        private void ParseFormDataParameters(string formData)
        {
            var dataPairs = formData
                .Split("&", StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in dataPairs)
            {
                var pairTokens = pair
                    .Split("=", StringSplitOptions.RemoveEmptyEntries);

                var key = pairTokens[0];
                var value = pairTokens[1];

                if (!this.FormData.ContainsKey(key))
                    this.FormData[key] = value;
            }
        }
    }
}
