﻿using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using System.Collections.Generic;

namespace SIS.HTTP.Requests.Contracts
{
    public interface IHttpRequest
    {
        string Path { get; }

        string Url { get; }

        Dictionary<string, object> FormData { get; }

        Dictionary<string, object> QueryData { get; }

        HttpHeaderCollection Headers { get; }

        HttpRequestMethod RequestMethod { get; }
    }
}
