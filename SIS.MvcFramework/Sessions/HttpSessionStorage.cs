namespace SIS.MvcFramework.Sessions
{
    using System.Collections.Concurrent;

    using SIS.HTTP.Sessions;

    public class HttpSessionStorage : IHttpSessionStorage
    {
        public const string SessionCookieKey = "SIS_ID";

        private readonly ConcurrentDictionary<string, IHttpSession> HttpSessions =
            new ConcurrentDictionary<string, IHttpSession>();

        public IHttpSession GetSession(string id)
        {
            return HttpSessions.GetOrAdd(id, _ => new HttpSession(id));
        }

        public bool ContainsSession(string id)
        {
            return HttpSessions.ContainsKey(id);
        }
    }
}
