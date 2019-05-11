namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

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


        /// <summary>
        /// Root call method. It calls all other methods to parse the request from plain text.
        /// Invoked by the constructor.
        /// </summary>
        /// <param name="requestString">Request string.</param>
        private void ParseRequest(string requestString)
        {
            string[] request = requestString
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string[] requestLine = request[0]
                 .Trim()
                 .Split();

            // .ToLower() fixes the all upper case behaviour caused by TextInfo.ToTitleCase():
            // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.textinfo.totitlecase?view=netframework-4.8#examples
            requestLine[0] = requestLine[0].ToLower();

            if (this.IsRequestLineValid(requestLine))
                throw new BadRequestException();

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();
            this.ParseHeaders(request);

            string formParamsAsString = request[request.Length - 1];
            this.ParseRequestParameters(formParamsAsString);

        }

        /// <summary>
        /// This method checks if the split requestLine holds exactly 3 elements, and if the 3rd element is equal to “HTTP/1.1”.
        /// </summary>
        /// <returns><c>true</c>, if request line is valid, <c>false</c> otherwise.</returns>
        /// <param name="requestLineArgs">Request line arguments.</param>
        private bool IsRequestLineValid(string[] requestLineArgs)
        {
            bool isValid = true;

            // FIXME this does not make the request method to title case.
            // Ex: GET -> Get
            var method = CultureInfo
                .InvariantCulture
                .TextInfo
                .ToTitleCase(requestLineArgs[0]);

            var route = requestLineArgs[1];
            var protocol = requestLineArgs[2];

            var canRequestEnumBeParsed = Enum.TryParse<HttpRequestMethod>(method, out _);
            if (!canRequestEnumBeParsed)
                isValid = false;

            // NOTE
            // Maybe needed to change checking method.
            else if (!Uri.IsWellFormedUriString(route, UriKind.Absolute))
                isValid = false;

            else if (protocol != "HTTP/1.1")
                isValid = false;

            return isValid;
        }

        /// <summary>
        /// Sets the Request’s Method, by parsing the 1st element from the split requestLine
        /// </summary>
        /// <param name="requestLine">Request line (splitted as an array).</param>
        private void ParseRequestMethod(string[] requestLine)
        {
            var method = CultureInfo
                .InvariantCulture
                .TextInfo
                .ToTitleCase(requestLine[0]);

            this.RequestMethod = (HttpRequestMethod)Enum.Parse(typeof(HttpRequestMethod), method);
        }

        /// <summary>
        /// Sets the Request’s Url to the 2nd element from the split requestLine.
        /// </summary>
        /// <param name="requestLine">Request line (splitted as an array).</param>
        private void ParseRequestUrl(string[] requestLine)
        {
            this.Url = HttpUtility.UrlDecode(requestLine[1]);
        }

        /// <summary>
        /// Sets the Request’s Path, by splitting the Request’s Url and taking only the path from it.
        /// </summary>
        private void ParseRequestPath()
        {
            // FIXME
            // Find a better way to parse the path from the url string.            
            var urlSplitted = this.Url
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (urlSplitted.Count > 0 && urlSplitted[0] != "/")
            {
                if (urlSplitted.Count > 1)
                    urlSplitted.RemoveAt(0);

                this.Path = $"/{string.Join("/", urlSplitted).TrimEnd('/')}";
            }

            else
            {
                this.Path = "/";
            }
        }

        /// <summary>
        /// Skipping the first line (the request line), traverses the request lines until it reaches an empty line (the CRLF line). 
        /// Each line represents a header, which must be split and parsed. 
        /// Then the string data is mapped to an HttpHeader object, and the object itself is added to the Headers property of the Request.
        /// </summary>
        /// <param name="splittedRequest">Splitted request.</param>
        /// <exception cref="BadRequestException">Throws a BadRequestException if there is no “Host” Header present after the parsing.</exception>
        private void ParseHeaders(ICollection<string> splittedRequest)
        {
            foreach (var headerPair in splittedRequest.Skip(1))
            {
                if (headerPair == Environment.NewLine)
                    break;

                var kvp = headerPair.Split(": ", StringSplitOptions.RemoveEmptyEntries);

                // NB: When it reaches a CRLF line we break the parsing loop.
                if (kvp.Length == 1)
                    break;

                // TODO
                // CHECK
                // Invalid data may be passed, check for invalid kvp's.
                var currentHeader = 
                    new HttpHeader(kvp[0], kvp[1].Replace(Environment.NewLine, string.Empty));

                this.Headers.Add(currentHeader);
            }

            if (this.Headers.ContainsHeader("Host") == false)
                throw new BadRequestException();
        }

        /// <summary>
        /// It checks if the Query string is NOT NULL or empty and if there is atleast 1 or more queryParameters.
        /// </summary>
        /// <returns><c>true</c>, if request query string is valid, <c>false</c> otherwise.</returns>
        /// <param name="queryString">Query string.</param>
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

        /// <summary>
        /// Extracts the Query string, by splitting the Request’s Url and taking only the query from it. Then splits the Query string into different parameters, and maps each of them into the Query Data Dictionary.
        /// Validates the Query string and parameters by calling the IsValidrequestQueryString() method.
        /// </summary>
        /// <remarks>Does nothing if the Request’s Url contains NO Query string.</remarks>
        /// <exception cref="BadRequestException">Throws a BadRequestException if the Query string is invalid.</exception>
        private void ParseQueryParameters()
        {
            // It's better to use HttpUtility.ParseQueryString() but the point is to write these methods by hand.

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

        /// <summary>
        /// Splits the Request’s Body into different parameters, and maps each of them into the Form Data Dictionary.
        /// </summary>
        /// <remarks>Does nothing if the Request contains NO Body.</remarks>
        /// <param name="formData">Form data.</param>
        private void ParseFormDataParameters(string formData)
        {
            if (!string.IsNullOrWhiteSpace(formData))
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

        /// <summary>
        /// Wrapper method to parse query and form data parameters.
        /// </summary>
        /// <param name="formData">Form data.</param>
        private void ParseRequestParameters(string formData)
        {
            this.ParseQueryParameters();
            this.ParseFormDataParameters(formData);
        }
    }
}
