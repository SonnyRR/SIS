namespace SIS.HTTP.Cookies
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies.Contracts;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private Dictionary<string, HttpCookie> cookies;
        private const string HttpCookieStringSeparator = "; ";

        public HttpCookieCollection()
        {
            this.cookies = new Dictionary<string, HttpCookie>();
        }

        public void AddCookie(HttpCookie cookie)
        {
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));

            this.cookies.Add(cookie.Key, cookie);
        }

        public bool ContainsCookie(string key)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));

            return this.cookies.ContainsKey(key);
        }

        public HttpCookie GetCookie(string key)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));

            return this.cookies[key];
        }

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            foreach (var kvp in this.cookies)
            {
                yield return kvp.Value;
            }
        }

        public bool HasCookies()
        {
            return this.cookies.Count > 0 ? true : false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // FIXME
            return (IEnumerator)this.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var cookie in this.cookies.Values)
            {
                builder.Append($"Set-Cookie: {cookie}{GlobalConstants.HttpNewLine}");
            }

            return builder.ToString();
        }
    }
}
