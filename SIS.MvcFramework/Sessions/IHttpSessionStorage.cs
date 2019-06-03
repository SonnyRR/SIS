namespace SIS.MvcFramework.Sessions
{
    using System;
    using SIS.HTTP.Sessions;

    public interface IHttpSessionStorage
    {
        IHttpSession GetSession(string id);

        bool ContainsSession(string id);
    }
}
