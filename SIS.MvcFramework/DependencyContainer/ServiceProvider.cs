namespace SIS.MvcFramework.DependencyContainer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ServiceProvider : IServiceProvider
    {
        private readonly IDictionary<Type, Type>
            dependencyContainer = new ConcurrentDictionary<Type, Type>();

        public void Add<TSource, TDestination>()
            where TDestination : TSource
        {
            this.dependencyContainer[typeof(TSource)] = typeof(TDestination);
        }

        public object CreateInstance(Type type)
        {
            if (this.dependencyContainer.ContainsKey(type))
            {
                type = this.dependencyContainer[type];
            }

            var constructor = type
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(x => x.GetParameters().Count())
                .FirstOrDefault();

            if (constructor == null)
            {
                return null;
            }

            var parameters = constructor.GetParameters();
            var parameterInstances = new List<object>();

            foreach (var parameter in parameters)
            {
                var parameterInstance = this.CreateInstance(parameter.ParameterType);
                parameterInstances.Add(parameterInstance);
            }

            var obj = constructor.Invoke(parameterInstances.ToArray());
            return obj;
        }
    }
}
