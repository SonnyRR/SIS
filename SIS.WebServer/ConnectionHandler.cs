namespace SIS.WebServer
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    using SIS.HTTP.Requests;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Routing;

    internal class ConnectionHandler
    {
        private Socket client;
        private ServerRoutingTable routingTable;

        public ConnectionHandler(Socket client, ServerRoutingTable routingTable)
        {
            this.client = client;
            this.routingTable = routingTable;
        }

        public async Task ProcessRequestAsync()
        {
            IHttpRequest request = await this.ReadRequest();

            if (request == null)
                this.client.Shutdown(SocketShutdown.Both);

            IHttpResponse response = this.HandleRequest(request);
            await this.PrepareResponse(response);
        }

        private IHttpResponse HandleRequest(IHttpRequest request)
        {

            // FIXME
            // StackOverFlow exception
            if (this.routingTable.Routes.ContainsKey(request.RequestMethod) == false
                || this.routingTable.Routes[request.RequestMethod].ContainsKey(request.Path) == false)
            {
                return new HttpResponse(HTTP.Enums.HttpResponseStatusCode.NotFound);
            }

            return this.routingTable.Routes[request.RequestMethod][request.Path].Invoke(request);
        }

        private async Task PrepareResponse(IHttpResponse response)
        {
            byte[] segments = response.GetBytes();
            await this.client.SendAsync(segments, SocketFlags.None);
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
    }
}