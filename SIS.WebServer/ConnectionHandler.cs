namespace SIS.WebServer
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
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using SIS.WebServer.Routing;
    using SIS.WebServer.Sessions;

    internal class ConnectionHandler
    {
        private Socket client;
        private ServerRoutingTable routingTable;

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

        public async Task ProcessRequestAsync()
        {
            try
            {
                IHttpRequest request = await this.ReadRequest();

                if (request != null)                    
                {
                    Console.WriteLine($"Processing: [Method: {request.RequestMethod} | Path: {request.Path}]{Environment.NewLine}");

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
            string sessionId;

            if (httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = httpRequest
                    .Cookies
                    .GetCookie(HttpSessionStorage.SessionCookieKey);

                sessionId = cookie.Value;
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }

            else
            {
                sessionId = Guid.NewGuid().ToString();
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }

            return sessionId;
        }

        private void SetResponseSession(IHttpResponse httpResponse, string sessionId)
        {
            if (sessionId != null)
            {
                httpResponse.AddCookie(new HttpCookie(HttpSessionStorage.SessionCookieKey, sessionId));
            }
        }
    }
}