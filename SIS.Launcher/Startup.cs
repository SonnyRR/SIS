namespace SIS.Launcher
{
    using System;

    using SIS.HTTP.Enums;
    using SIS.Launcher.Controllers;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class Startup
    {
        static void Main(string[] args)
        {
            ServerRoutingTable routingTable = new ServerRoutingTable();

            routingTable.Add(HttpRequestMethod.Get, "/", httpRequest
                => new HomeController().Home(httpRequest));

            Server server = new Server(8040, routingTable);
            server.Run();
        }
    }
}
