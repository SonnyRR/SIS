namespace SIS.HTTP.Enums
{
    using System.ComponentModel;

    // FIXME
    // TODO
    // Missing enum member "Description" attributes will not work correctly
    // when sent as a response to the client.
    public enum HttpResponseStatusCode
    {
        [Description("OK")]
        Ok = 200,
        Created = 201,
        Found = 302,

        [Description("See Other")]
        SeeOther = 303,

        [Description("Bad Request")]
        BadRequest = 400,
        Unauthorized = 401,

        [Description("Forbidden")]
        Forbidden = 403,

        [Description("Not Found")]
        NotFound = 404,

        [Description("Internal Server Error")]
        InternalServerError = 500
    }
}
