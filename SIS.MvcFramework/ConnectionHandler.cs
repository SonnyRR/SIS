namespace SIS.MvcFramework
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Requests;   
    using SIS.HTTP.Responses;
    using SIS.HTTP.Sessions;
    using SIS.MvcFramework.Results;
    using SIS.MvcFramework.Routing;
    using SIS.MvcFramework.Sessions;

    internal class ConnectionHandler
    {
        private Socket client;
        private ServerRoutingTable routingTable;

        public async Task ProcessRequestAsync()
        {
            try
            {
                IHttpRequest request = await this.ReadRequest();

                if (request != null)
                {
                    Console.WriteLine(
                        $@"Processing: [Method: ""{request.RequestMethod}"" | Path: ""{request.Path}""]{Environment.NewLine}");

                    var sessionId = this.SetRequestSession(request);

                    IHttpResponse response = this.HandleRequest(request);

                    this.SetResponseSession(response, sessionId);

                    await this.PrepareResponse(response);
                }
            }

            catch (BadRequestException ex)
            {
                await this.PrepareResponse(new TextResult(ex.ToString(), HttpResponseStatusCode.BadRequest));
            }

            catch (Exception ex)
            {
                await this.PrepareResponse(new TextResult(ex.ToString(), HttpResponseStatusCode.InternalServerError));
            }

            this.client.Shutdown(SocketShutdown.Both);

        }

        public ConnectionHandler(Socket client, ServerRoutingTable routingTable)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(routingTable, nameof(routingTable));

            this.client = client;
            this.routingTable = routingTable;
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            StringBuilder builder = new StringBuilder();
            var buffer = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int readBytes = await this.client
                    .ReceiveAsync(buffer.Array, SocketFlags.None);

                if (readBytes == 0)
                    break;

                string requestAsString = Encoding.UTF8.GetString(buffer.Array, 0, readBytes);
                builder.Append(requestAsString);

                if (readBytes < 1023)
                    break;
            }

            if (builder.Length == 0)
                return null;

            return new HttpRequest(builder.ToString());
        }

        private IHttpResponse HandleRequest(IHttpRequest request)
        {
            if (!this.routingTable.Contains(request.RequestMethod, request.Path))
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            return this.routingTable.Get(request.RequestMethod, request.Path).Invoke(request);
        }

        private async Task PrepareResponse(IHttpResponse response)
        {
            byte[] segments = response.GetBytes();
            await this.client.SendAsync(segments, SocketFlags.None);
        }

        private string SetRequestSession(IHttpRequest httpRequest)
        {
            if (httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = httpRequest
                    .Cookies
                    .GetCookie(HttpSessionStorage.SessionCookieKey);

                string sessionId = cookie.Value;

                if (HttpSessionStorage.ContainsSession(sessionId))
                {
                    httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
                }
            }

            return httpRequest.Session?.Id;
        }

        /// <summary>
        /// Checks if the session id is present in the HttpSessionStorage repository
        /// then creates a session and attaches a Set-Cookie header to the response.
        /// </summary>
        /// <param name="httpResponse">Response</param>
        /// <param name="sessionId">Session Id</param>
        private void SetResponseSession(IHttpResponse httpResponse, string sessionId)
        {
            
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
            }

            if (!HttpSessionStorage.ContainsSession(sessionId))
            {
                IHttpSession newSession = HttpSessionStorage.AddOrUpdateSession(sessionId);
                httpResponse.AddCookie(new HttpCookie(HttpSessionStorage.SessionCookieKey, newSession.Id));
            }
        }
    }
}