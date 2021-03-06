﻿namespace SIS.HTTP.Cookies
{
    using System;
    using System.Text;

    using SIS.Common;
    using SIS.HTTP.Common;
 
    public class HttpCookie
    {
        private const int HttpCookiesDefaultExpirationDays = 3;
        private const string HttpCookieDefaultPath = "/";

        public HttpCookie(string key, string value,
            int expires = HttpCookiesDefaultExpirationDays, string path = HttpCookieDefaultPath)
            : this(key, value, isNew: true, expires, path)
        {            
        }

        public HttpCookie(string key, string value, bool isNew,
            int expires = HttpCookiesDefaultExpirationDays, string path = HttpCookieDefaultPath)
        {
            key.ThrowIfNullOrEmpty(nameof(key));
            value.ThrowIfNullOrEmpty(nameof(value));
            path.ThrowIfNullOrEmpty(nameof(path));


            this.Key = key;
            this.Value = value;
            this.Expires = DateTime.UtcNow.AddDays(expires);
            this.Path = path;
            this.HttpOnly = isNew;
            this.IsNew = isNew;
        }

        public string Key { get; }

        public string Value { get; }

        public DateTime Expires { get; private set; }

        public string Path { get; set; }

        public bool IsNew { get; }

        public bool HttpOnly { get; set; }

        public void Delete()
        {
            this.Expires = DateTime.UtcNow.AddDays(-1);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{this.Key}={this.Value}; Expires={this.Expires:R}");

            if (this.HttpOnly)
            {
                builder.Append($"; HttpOnly");
            }

            builder.Append($"; Path={this.Path}");

            return builder.ToString();
        }
    }
}
