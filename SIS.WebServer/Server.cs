namespace SIS.WebServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
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

            Console.WriteLine($"SIS Server is running on: {LocalhostIpAddress}:{this.port}{Environment.NewLine}" +
                $"OS: {RuntimeInformation.OSDescription}{Environment.NewLine}");

            var task = Task.Run(this.Listen);
            task.Wait();
        }

        public async Task Listen()
        {
            while (this.isRunning)
            {
                Console.WriteLine("Waiting for client request...");

                using (var client = this.listener.AcceptSocketAsync().GetAwaiter().GetResult())
                {
                    var connectionHandler = new ConnectionHandler(client, this.routingTable);
                    await connectionHandler.ProcessRequestAsync();                    
                }
            }
        }

    }
}
