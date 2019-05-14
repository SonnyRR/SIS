﻿namespace SIS.HTTP.Responses
{
    using System;
    using System.Linq;
    using System.Text;

    using SIS.HTTP.Common;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Extensions;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Responses.Contracts;

    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
        }

        public HttpResponse(HttpResponseStatusCode statusCode)
        {
            this.Headers = new HttpHeaderCollection();
            this.Content = null;

            this.StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; private set; }

        public byte[] Content { get; set; }

        /// <summary>
        /// Adds the header to the request.
        /// </summary>
        /// <param name="header">Header.</param>
        public void AddHeader(HttpHeader header)
        {
            this.Headers.Add(header);
        }

        /// <summary>
        /// Forms the full response in <see cref="System.Byte"/>[] in order to be send to the client.
        /// </summary>
        /// <returns>The full response as <see cref="System.Byte"/>[].</returns>
        public byte[] GetBytes()
        {
            var responseLineAsString = this.ToString();

            // NB: Use consistent encodings when converting to byte[]
            // as there is difference when encoding/decoding from bytes.
            var responseLineAsBytes = Encoding.UTF8.GetBytes(responseLineAsString);

            return responseLineAsBytes
                .Concat(this.Content ?? Enumerable.Empty<byte>())
                .ToArray();
        }

        /// <summary>
        /// Forms the Response line. It holds the protocol, the status code and the status, and the Response Headers along with the CRLF line.
        /// These properties are concatenated in a <see cref="System.String"/> and returned.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the response line with its concatenated properties.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{GlobalConstants.HttpOneProtocolFragment} {this.StatusCode.GetResponseLine()}{GlobalConstants.HttpNewLine}");
            builder.Append($"{this.Headers.ToString()}{GlobalConstants.HttpNewLine}");
            builder.Append(GlobalConstants.HttpNewLine);

            return builder.ToString();
        }
    }
}
