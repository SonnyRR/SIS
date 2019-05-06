namespace SIS.WebServer
{
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using SIS.HTTP.Requests.Contracts;
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

        }
    }
}