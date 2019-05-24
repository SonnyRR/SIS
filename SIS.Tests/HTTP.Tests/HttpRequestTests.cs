namespace SIS.Tests.SIS.HTTP.Tests
{
    using System;
    using Xunit;

    using SIS.HTTP;
    using SIS.HTTP.Requests;

    public class HttpRequestTests
    {
        [Fact]
        public void ParseRequestTestShouldBeSuccessfull()
        {
            string requestAsString = @"GET /home/schedule HTTP/1.1
Host: softuni.bg
Connection: keep-alive
Cache-Control: max-age=0
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36 OPR/60.0.3255.109
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Accept-Encoding: gzip, deflate, br
Accept-Language: en-GB,en-US;q=0.9,en;q=0.8";

            HttpRequest request = new HttpRequest(requestAsString);

            Assert.Equal("/home/schedule", request.Path);
            Assert.Equal("softuni.bg", request.Url);


        }
    }
}
