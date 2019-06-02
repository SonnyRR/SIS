namespace SIS.MvcFramework
{
    using System;
    using System.Linq;
    using System.Reflection;

    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses;
    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.Attributes.Action;
    using SIS.MvcFramework.Attributes.Security;
    using SIS.MvcFramework.Result;
    using SIS.MvcFramework.Routing;

    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            AutoRegisterRoutes(application, serverRoutingTable);

            application.ConfigureServices();
            application.Configure(serverRoutingTable);

            var server = new Server(8000, serverRoutingTable);
            server.Run();
        }

        private static void AutoRegisterRoutes(
            IMvcApplication application, IServerRoutingTable serverRoutingTable)
        {
            var controllers = application
                .GetType()
                .Assembly
                .GetTypes()
                .Where(type => type.IsClass 
                    && !type.IsAbstract
                    && typeof(Controller).IsAssignableFrom(type))
                .ToList();

            foreach (var controller in controllers)
            {
                var actions = controller
                    .GetMethods(BindingFlags.DeclaredOnly
                    | BindingFlags.Public
                    | BindingFlags.Instance)
                    .Where(x => !x.IsSpecialName && x.DeclaringType == controller)
                    .Where(x => x.GetCustomAttributes()
                        .All(a => a.GetType() != typeof(NonActionAttribute)));
                
                foreach (var actionMethod in actions)
                {
                    var path = $"/{controller.Name.Replace("Controller", string.Empty)}/{actionMethod.Name}";

                    var attribute = actionMethod.GetCustomAttributes()
                        .Where(x => x
                            .GetType()
                            .IsSubclassOf(typeof(BaseHttpAttribute)))
                        .LastOrDefault() as BaseHttpAttribute;

                    var httpMethod = HttpRequestMethod.Get;

                    if (attribute != null)
                    {
                        httpMethod = attribute.Method;
                    }

                    if (attribute?.Url != null)
                    {
                        path = attribute.Url;
                    }

                    if (attribute?.ActionName != null)
                    {
                        path = $"/{controller.Name.Replace("Controller", string.Empty)}/{attribute.ActionName}";
                    }

                    serverRoutingTable.Add(httpMethod, path, request =>
                    {
                        // request => new UsersController().Login(request)
                        var controllerInstance = Activator.CreateInstance(controller);
                        ((Controller)controllerInstance).Request = request;

                        // Security Authorization - TODO: Refactor this
                        var controllerPrincipal = ((Controller)controllerInstance).User;
                        var authorizeAttribute = actionMethod
                            .GetCustomAttributes()
                            .LastOrDefault(a => a.GetType() == typeof(AuthorizeAttribute))
                            as AuthorizeAttribute;

                        if (authorizeAttribute != null && !authorizeAttribute.IsInAuthority(controllerPrincipal))
                        {
                            // TODO: Redirect to configured URL
                            return new HttpResponse(HttpResponseStatusCode.Forbidden);
                        }

                        // This calls the Invoke() on the current controller method(aciton) to return
                        // as the second param of Func<IHttpRequest, IHttpResponse>.
                        var response = actionMethod.Invoke(controllerInstance, new object[0]) as ActionResult;
                        return response;
                    });

                    Console.WriteLine($"Registered: {httpMethod.ToString().ToUpper()} {path}");
                }
            }
            Console.WriteLine();
        }
    }
}
