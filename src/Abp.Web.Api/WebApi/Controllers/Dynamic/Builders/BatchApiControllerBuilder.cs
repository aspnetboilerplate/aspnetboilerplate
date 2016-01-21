using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// This interface is used to define a dynamic api controllers.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    internal class BatchApiControllerBuilder<T> : IBatchApiControllerBuilder<T>
    {
        private readonly string _servicePrefix;
        private readonly Assembly _assembly;
        private IFilter[] _filters;
        private Func<Type, string> _serviceNameSelector;
        private Func<Type, bool> _typePredicate;
        private bool _conventionalVerbs;

        public BatchApiControllerBuilder(Assembly assembly, string servicePrefix)
        {
            _assembly = assembly;
            _servicePrefix = servicePrefix;
        }

        public IBatchApiControllerBuilder<T> Where(Func<Type, bool> predicate)
        {
            _typePredicate = predicate;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithFilters(params IFilter[] filters)
        {
            _filters = filters;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithServiceName(Func<Type, string> serviceNameSelector)
        {
            _serviceNameSelector = serviceNameSelector;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithConventionalVerbs()
        {
            _conventionalVerbs = true;
            return this;
        }

        public void Build()
        {
            var types =
                from
                    type in _assembly.GetTypes()
                where
                    type.IsPublic && type.IsInterface && typeof(T).IsAssignableFrom(type) && IocManager.Instance.IsRegistered(type)
                select
                    type;

            if (_typePredicate != null)
            {
                types = types.Where(t => _typePredicate(t));
            }

            foreach (var type in types)
            {
                var serviceName = _serviceNameSelector != null
                    ? _serviceNameSelector(type)
                    : GetConventionalServiceName(type);

                if (!string.IsNullOrWhiteSpace(_servicePrefix))
                {
                    serviceName = _servicePrefix + "/" + serviceName;
                }

                var builder = typeof(DynamicApiControllerBuilder)
                    .GetMethod("For", BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(type)
                    .Invoke(null, new object[] { serviceName });

                if (_filters != null)
                {
                    builder.GetType()
                        .GetMethod("WithFilters", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[] { _filters });
                }

                if (_conventionalVerbs)
                {
                    builder.GetType()
                       .GetMethod("WithConventionalVerbs", BindingFlags.Public | BindingFlags.Instance)
                       .Invoke(builder, new object[0]);
                }

                builder.GetType()
                        .GetMethod("Build", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[0]);
            }
        }

        public static string GetConventionalServiceName(Type type)
        {
            var typeName = type.Name;

            if (typeName.EndsWith("ApplicationService"))
            {
                typeName = typeName.Substring(0, typeName.Length - "ApplicationService".Length);
            }
            else if (typeName.EndsWith("AppService"))
            {
                typeName = typeName.Substring(0, typeName.Length - "AppService".Length);
            }
            else if (typeName.EndsWith("Service"))
            {
                typeName = typeName.Substring(0, typeName.Length - "Service".Length);
            }

            if (typeName.Length > 1 && typeName.StartsWith("I") && char.IsUpper(typeName, 1))
            {
                typeName = typeName.Substring(1);
            }

            return typeName.ToCamelCase();
        }
    }
}