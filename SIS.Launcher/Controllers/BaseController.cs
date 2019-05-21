namespace SIS.Launcher.Controllers
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;

    public class BaseController
    {
        public IHttpResponse View([CallerMemberName] string view = null)
        {
            string controllerName = this.GetType()
                .Name
                .Replace("Controller", string.Empty);

            string viewName = view;

            string pathFromToReadContent = Path.Combine("Views", controllerName, $"{viewName}.html");
            string viewContent = File.ReadAllText(pathFromToReadContent);

            return new HtmlResult(viewContent, HttpResponseStatusCode.Ok);
        }
    }
}
