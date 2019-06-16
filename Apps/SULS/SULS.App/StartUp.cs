namespace SULS.App
{
    using Data;
    using SIS.MvcFramework;
    using SIS.MvcFramework.DependencyContainer;
    using SIS.MvcFramework.Routing;
    using SULS.Services;

    public class StartUp : IMvcApplication
    {
        public void Configure(IServerRoutingTable serverRoutingTable)
        {
            using (var db = new SULSContext())
            {
                db.Database.EnsureCreated();
            }
        }

        public void ConfigureServices(IServiceProvider serviceProvider)
        {
            // TODO: Add services
            serviceProvider.Add<IUserService, UserService>();
            serviceProvider.Add<IProblemService, ProblemService>();
            serviceProvider.Add<ISubmissionService, SubmissionService>();
        }
    }
}