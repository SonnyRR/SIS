namespace SIS.HTTP.Responses
{
    using System.Text;

    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Cookies.Contracts;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Extensions;
    using SIS.HTTP.Headers;

    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
            this.Headers = new HttpHeaderCollection();
            this.Content = new byte[0];
            this.Cookies = new HttpCookieCollection();
        }

        public HttpResponse(HttpResponseStatusCode statusCode)
            :this()
        {
            this.StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; private set; }

        public IHttpCookieCollection Cookies { get; private set; }

        public byte[] Content { get; set; }

        public void AddCookie(HttpCookie cookie)
        {
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));

            this.Cookies.AddCookie(cookie);
        }

        /// <summary>
        /// Adds the header to the request.
        /// </summary>
        /// <param name="header">Header.</param>
        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));

            this.Headers.AddHeader(header);
        }

        /// <summary>
        /// Forms the full response in <see cref="System.Byte"/>[] in order to be send to the client.
        /// </summary>
        /// <returns>The full response as <see cref="System.Byte"/>[].</returns>
        public byte[] GetBytes()
        {
            var responseLineAndHeadersAsString = this.ToString();

            var responseLineAsBytes = Encoding.UTF8.GetBytes(responseLineAndHeadersAsString);
            var response = new byte[responseLineAsBytes.Length + this.Content.Length];

            for (int index = 0; index < responseLineAsBytes.Length; index++)
            {
                response[index] = responseLineAsBytes[index];
            }

            for (int index = 0; index < this.Content.Length; index++)
            {
                response[index + responseLineAsBytes.Length] = this.Content[index];
            }

            return response;
        }

        /// <summary>
        /// Forms the Response line. It holds the protocol, the status code and the status
        /// and the Response Headers along with the CRLF line.
        /// These properties are concatenated in a <see cref="System.String"/> and returned.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the response line with its concatenated properties.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{GlobalConstants.HttpOneProtocolFragment} {this.StatusCode.GetResponseLine()}").Append(GlobalConstants.HttpNewLine);
            builder.Append($"{this.Headers.ToString()}").Append(GlobalConstants.HttpNewLine);

            if (this.Cookies.HasCookies())
            {
                builder.Append($"{this.Cookies}").Append(GlobalConstants.HttpNewLine);
            }

            builder.Append(GlobalConstants.HttpNewLine);

            return builder.ToString();
        }
    }
}
