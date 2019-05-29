namespace SIS.MvcFramework.Result
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses;

    public abstract class ActionResult : HttpResponse
    {
        protected ActionResult(HttpResponseStatusCode httpResponseStatusCode) 
            : base(httpResponseStatusCode)
        {
        }
    }
}
