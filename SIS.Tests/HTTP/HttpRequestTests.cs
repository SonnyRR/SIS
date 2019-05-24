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

        [Fact]
        public void ParseRequestShouldThrowExceptionWhenRequestLineIsInvalid()
        {
            var requestAsStringWithoutProtocol = @"GET /users/profile/show/vasilkotsev?simplequery=value
Host: softuni.bg
Connection: keep-alive";

            var requestAsStringWithoutMethod = @"/users/profile/show/vasilkotsev?simplequery=value HTTP/1.1
Host: softuni.bg
Connection: keep-alive";

            Assert.Throws(typeof(BadRequestException), () => new HttpRequest(requestAsStringWithoutProtocol));
            Assert.Throws(typeof(BadRequestException), () => new HttpRequest(requestAsStringWithoutMethod));
        }
    }
}
