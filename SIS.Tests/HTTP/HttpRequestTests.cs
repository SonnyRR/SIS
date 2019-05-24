namespace SIS.Tests.HTTP
{
    using System;
    using Xunit;

    using SIS.HTTP;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;

    public class HttpRequestTests
    {
        [Fact]
        public void ParseRequestShouldBeSuccessfull()
        {
            string requestAsString = @"GET /users/profile/show/vasilkotsev?simplequery=value&anotherone=true HTTP/1.1
Host: softuni.bg
Connection: keep-alive
Cache-Control: max-age=0
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36 OPR/60.0.3255.109
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Accept-Encoding: gzip, deflate, br
Accept-Language: en-GB,en-US;q=0.9,en;q=0.8
Cookie: simpleCookie=test; lang=bg";

            string expectedHeadersAsString = @"Host: softuni.bg
Connection: keep-alive
Cache-Control: max-age=0
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36 OPR/60.0.3255.109
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Accept-Encoding: gzip, deflate, br
Accept-Language: en-GB,en-US;q=0.9,en;q=0.8
Cookie: simpleCookie=test; lang=bg";

            HttpRequest request = new HttpRequest(requestAsString);

            var requestHeaders = request.Headers.ToString();

            Assert.Equal(request.RequestMethod, HttpRequestMethod.Get);
            Assert.Equal("/users/profile/show/vasilkotsev", request.Path);
            Assert.Equal("/users/profile/show/vasilkotsev?simplequery=value&anotherone=true", request.Url);
            Assert.Equal(2, request.QueryData.Count);
            Assert.Equal(request.Cookies.HasCookies(), true);
            Assert.Equal(expectedHeadersAsString, requestHeaders);
        }

        [Theory]
        [InlineData(@"GET /users/profile/show/vasilkotsev?simplequery=value\r\nHost: softuni.bg\r\nConnection: keep-alive\r\n")]
        [InlineData(@"/users/profile/show/vasilkotsev?simplequery=value HTTP/1.1\r\nHost: softuni.bg\r\nConnection: keep-alive\r\n")]
        public void ParseRequestShouldThrowExceptionWhenRequestLineIsInvalid(string requestAsString)
        {

            Assert.Throws(typeof(BadRequestException), () => new HttpRequest(requestAsString));
        }


        //FIXME
        // Broken test
        [Theory]
        [InlineData("POST /test HTTP/1.1\r\nHost: foo.example\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 27\r\n\r\nfield1=value1&field2=value2", 2)]
        [InlineData("POST /test HTTP/1.1\r\nHost: foo.example\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 27\r\n\r\nfield1=value1&field1=value2", 1)]
        public void RequestShouldParseFormDataCorrectly(string requestAsString, int expectedCount)
        {
            HttpRequest request = new HttpRequest(requestAsString);

            Assert.Equal(request.FormData.Count, expectedCount);
        }
    }
}
