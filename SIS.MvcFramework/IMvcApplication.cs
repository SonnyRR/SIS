namespace SIS.MvcFramework
{
    using SIS.MvcFramework.Routing;

    public interface IMvcApplication
    {
        void Configure(IServerRoutingTable serverRoutingTable);

        void ConfigureServices(); // DI
    }
}
