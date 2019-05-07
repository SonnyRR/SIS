namespace SIS.Launcher.Controllers
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;

    public class HomeController
    {
        public IHttpResponse Index()
        {
            var content = "<h1>Hello, world!</h1>";

            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }
    }
}
