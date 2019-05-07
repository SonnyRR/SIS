namespace SIS.WebServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using SIS.WebServer.Routing;

    public class Server
    {
        private const string LocalhostIpAddress = "127.0.0.1";

        private readonly int port;
        private readonly TcpListener listener;
        private readonly ServerRoutingTable routingTable;
        private bool isRunning;

        public Server(int port, ServerRoutingTable routingTable)
        {
            this.port = port;
            this.routingTable = routingTable;

            var ipAddress = IPAddress.Parse(LocalhostIpAddress);
            this.listener = new TcpListener(ipAddress, this.port);

        }

        public void Run()
        {
            this.listener.Start();
            this.isRunning = true;

            Console.WriteLine($"SIS Server is running on: {LocalhostIpAddress}:{this.port} | {Environment.OSVersion}");

            var task = Task.Run(this.ListenLoop);
            task.Wait();
        }

        public async Task ListenLoop()
        {
            while (this.isRunning)
            {
                var client = await this.listener.AcceptSocketAsync();
                var connectionHandler = new ConnectionHandler(client, this.routingTable);
                var responseTask = connectionHandler.ProcessRequestAsync();
                responseTask.Wait();
            }
        }

    }
}
