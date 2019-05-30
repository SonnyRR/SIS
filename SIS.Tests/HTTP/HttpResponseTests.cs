namespace SIS.Tests.HTTP
{
    using Xunit;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Enums;

    public class HttpResponseTests
    {
        [Fact]
        public void HttpResponseForbiddenToString()
        {
            HttpResponse response = new HttpResponse(HttpResponseStatusCode.Forbidden);

            var responseToString = response.ToString();

            Assert.True(responseToString == "HTTP/1.1 403 Forbidden\r\n\r\n\r\n", "Forbidden response is not correctly represented as a string.");
        }
    }
}
