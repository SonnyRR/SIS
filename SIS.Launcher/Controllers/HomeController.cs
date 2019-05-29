namespace SIS.Launcher.Controllers
{
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;

    public class HomeController : BaseController
    {
        public IHttpResponse Home(IHttpRequest httpRequest)
        {
            return this.View();
        }
    }
}
