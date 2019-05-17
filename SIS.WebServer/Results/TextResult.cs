namespace SIS.WebServer.Results
{
    using System.Text;

    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Responses;

    public class TextResult : HttpResponse
    {
        public TextResult(string content, HttpResponseStatusCode statusCode
            , string contentType = "text/plain; charset=utf8")
            : base(statusCode)
        {
            this.Headers.Add(new HttpHeader("Content-Type", contentType));
            this.Content = Encoding.UTF8.GetBytes(content);
        }

        public TextResult(byte[] content, HttpResponseStatusCode statusCode
            , string contentType = "text/plain; charset=utf8")
            : base(statusCode)
        {
            this.Headers.Add(new HttpHeader("Content-Type", contentType));
            this.Content = content;
        }
    }
}
