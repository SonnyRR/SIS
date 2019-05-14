namespace SIS.WebServer
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
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

                //FIXME               
                if (request == null)
                    this.client.Shutdown(SocketShutdown.Both);

                Console.WriteLine($"Processing: {request.RequestMethod} {request.Path}");

                IHttpResponse response = this.HandleRequest(request);
                await this.PrepareResponse(response);
            }

            catch (BadRequestException ex)
            {
                await this.PrepareResponse(new TextResult(ex.ToString(), HttpResponseStatusCode.BadRequest));
            }

            catch (Exception ex)
            {
                await this.PrepareResponse(new TextResult(ex.ToString(), HttpResponseStatusCode.InternalServerError));                   
            }

        }

        private IHttpResponse HandleRequest(IHttpRequest request)
        {

            if (this.routingTable.Routes.ContainsKey(request.RequestMethod) == false
                || this.routingTable.Routes[request.RequestMethod].ContainsKey(request.Path) == false)
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            return this.routingTable.Routes[request.RequestMethod][request.Path].Invoke(request);
        }

        private async Task PrepareResponse(IHttpResponse response)
        {
            byte[] segments = response.GetBytes();
            await this.client.SendAsync(segments, SocketFlags.None);
            this.client.Shutdown(SocketShutdown.Both);
        }

    }
}