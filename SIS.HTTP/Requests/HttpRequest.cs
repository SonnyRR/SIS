namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using SIS.Common;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Cookies.Contracts;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Sessions;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestAsString)
        {
            requestAsString.ThrowIfNullOrEmpty(nameof(requestAsString));

            this.FormData = new Dictionary<string, ISet<string>>();
            this.QueryData = new Dictionary<string, ISet<string>>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.ParseRequest(requestAsString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, ISet<string>> FormData { get; }

        public Dictionary<string, ISet<string>> QueryData { get; }

        public HttpHeaderCollection Headers { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IHttpCookieCollection Cookies { get; }

        public IHttpSession Session { get; set; }

        /// <summary>
        /// Root call method. It calls all other methods to parse the request from plain text.
        /// Invoked by the constructor.
        /// </summary>
        /// <param name="requestString">Request string.</param>
        private void ParseRequest(string requestString)
        {

            string[] wholeRequest = requestString
                .Split(GlobalConstants.HttpNewLine, StringSplitOptions.None);

            string[] requestLine = wholeRequest[0]
                 .Trim()
                 .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsRequestLineValid(requestLine))
                throw new BadRequestException();

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();

            this.ParseRequestHeaders(wholeRequest);
            this.ParseRequestCookies();

            string formParamsAsString = wholeRequest[wholeRequest.Length - 1];
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

            if (requestLineArgs.Length != 3
                || requestLineArgs[2] != GlobalConstants.HttpOneProtocolFragment)
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Sets the Request’s Method, by parsing the 1st element from the split requestLine
        /// </summary>
        /// <param name="requestLineArgs">Request line (splitted as an array).</param>
        private void ParseRequestMethod(string[] requestLineArgs)
        {
            var methodAsString = requestLineArgs[0];

            HttpRequestMethod method;

            bool isMethodParsedSuccessfuly = Enum.TryParse(methodAsString, ignoreCase: true, out method);

            if (!isMethodParsedSuccessfuly)
            {
                throw new BadRequestException(string.Format(GlobalConstants.UnsupportedHttpMethodExceptionMessage, methodAsString));
            }

            this.RequestMethod = method;
        }

        /// <summary>
        /// Sets the Request’s Url to the 2nd element from the split requestLine.
        /// </summary>
        /// <param name="requestLine">Request line (splitted as an array).</param>
        private void ParseRequestUrl(string[] requestLine)
        {
            var urlAsString = requestLine[1];
            this.Url = HttpUtility.UrlDecode(urlAsString);
        }

        /// <summary>
        /// Sets the Request’s Path, by splitting the Request’s Url and taking only the path from it.
        /// </summary>
        private void ParseRequestPath()
        {
            var localUrlQueryIndex = this.Url.IndexOf('?');

            var path = localUrlQueryIndex != -1
                ? this.Url.Remove(localUrlQueryIndex, this.Url.Length - localUrlQueryIndex)
                : this.Url;

            this.Path = path;
        }

        /// <summary>
        /// Skipping the first line (the request line), traverses the request lines until it reaches an empty line (the CRLF line). 
        /// Each line represents a header, which must be split and parsed. 
        /// Then the string data is mapped to an HttpHeader object, and the object itself is added to the Headers property of the Request.
        /// </summary>
        /// <param name="splittedRequest">Splitted request.</param>
        /// <exception cref="BadRequestException">Throws a BadRequestException if there is no “Host” Header present after the parsing.</exception>
        private void ParseRequestHeaders(ICollection<string> splittedRequest)
        {
            foreach (var headerPair in splittedRequest.Skip(1))
            {
                // NB: When it reaches a CRLF line we break the parsing loop.
                if (headerPair == string.Empty)
                    break;

                var kvp = headerPair.Split(": ", StringSplitOptions.RemoveEmptyEntries);

                var currentHeader = new HttpHeader(
                    kvp[0], kvp[1].Replace(GlobalConstants.HttpNewLine, string.Empty));

                this.Headers.AddHeader(currentHeader);
            }

            if (this.Headers.ContainsHeader("Host") == false)
                throw new BadRequestException();
        }

        /// <summary>
        /// Checks the HttpHeadersCollection for a Header with name “Cookie”.
        /// If there is a match, extracts its string value, formats it, parses it and adds it to the HttpCookieCollection.
        /// </summary>
        private void ParseRequestCookies()
        {
            if (this.Headers.ContainsHeader(HttpHeader.Cookie))
            {
                var cookiesValues = this.Headers
                    .GetHeader(HttpHeader.Cookie)
                    .Value
                    .Split("; ", StringSplitOptions.RemoveEmptyEntries);

                foreach (var cookieAsAString in cookiesValues)
                {
                    var cookieSplitted = cookieAsAString
                        .Split('=', 2, StringSplitOptions.RemoveEmptyEntries);

                    var cookieKey = cookieSplitted[0];
                    var cookieVal = cookieSplitted[1];

                    HttpCookie cookie = new HttpCookie(cookieKey, cookieVal, isNew: false);
                    this.Cookies.AddCookie(cookie);
                }
            }
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
                .Split('=', StringSplitOptions.RemoveEmptyEntries);

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
            //FIXME
            var splittedUrlTokens = this.Url
                .Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries);

            if (splittedUrlTokens.Length > 1)
            {
                var queryData = splittedUrlTokens[1];

                var isValid = this.IsRequestQueryStringValid(queryData);

                if (!isValid)
                    throw new BadRequestException();

                var queryPairs = queryData
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(kvp => kvp.Split('=', 2, StringSplitOptions.RemoveEmptyEntries))
                .ToList();

                foreach (var kvp in queryPairs)
                {
                    var key = kvp[0];
                    var value = kvp[1];

                    if (!this.QueryData.ContainsKey(key))
                    {
                        this.QueryData.Add(key, new HashSet<string>());
                    }

                    this.QueryData[key].Add(value);
                }
            }
        }

        /// <summary>
        /// Splits the Request’s Body into different parameters, and maps each of them into the Form Data Dictionary.
        /// </summary>
        /// <remarks>Does nothing if the Request contains NO Body.</remarks>
        /// <param name="formData">Form data.</param>
        private void ParseRequestFormDataParameters(string formData)
        {
            formData.ThrowIfNullOrEmpty(nameof(formData));

            var dataPairs = formData
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(kvp => kvp.Split('=', 2, StringSplitOptions.RemoveEmptyEntries))
                .ToList();

            foreach (var kvp in dataPairs)
            {
                var key = kvp[0];
                var value = kvp[1];

                if (!this.FormData.ContainsKey(key))
                {
                    this.FormData.Add(key, new HashSet<string>());
                }

                this.FormData[key].Add(value);
            }
        }

        /// <summary>
        /// Wrapper method to parse query and form data parameters.
        /// </summary>
        /// <param name="formData">Form data.</param>
        private void ParseRequestParameters(string formData)
        {
            this.ParseQueryParameters();

            if (formData != string.Empty)
                this.ParseRequestFormDataParameters(formData);
        }
    }
}
