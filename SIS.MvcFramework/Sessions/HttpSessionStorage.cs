namespace SIS.MvcFramework.Sessions
{    
    using System.Collections.Concurrent;

    using SIS.HTTP.Sessions;

    public class HttpSessionStorage
    {
        public const string SessionCookieKey = "SIS_ID";

        private static readonly ConcurrentDictionary<string, IHttpSession> sessions
            = new ConcurrentDictionary<string, IHttpSession>();

        public static IHttpSession GetSession(string id)
        {
            return sessions.GetOrAdd(id, _ => new HttpSession(id));
        }

        public static bool ContainsSession(string id)
        {
            return sessions.ContainsKey(id);
        }

        public static IHttpSession AddOrUpdateSession(string id)
        {
            return sessions
                .AddOrUpdate(id, _ => new HttpSession(id), (key, val) => new HttpSession(id));
        }

    }
}
