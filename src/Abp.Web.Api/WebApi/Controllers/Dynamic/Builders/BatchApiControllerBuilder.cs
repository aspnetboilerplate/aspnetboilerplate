using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Filters;
using Abp.Application.Services;
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
        private Action<IApiControllerActionBuilder<T>> _forMethodsAction;
        private bool? _isApiExplorerEnabled;
        private readonly IIocResolver _iocResolver;
        private readonly IDynamicApiControllerBuilder _dynamicApiControllerBuilder;

        public BatchApiControllerBuilder(
            IIocResolver iocResolver,
            IDynamicApiControllerBuilder dynamicApiControllerBuilder, 
            Assembly assembly, 
            string servicePrefix)
        {
            _iocResolver = iocResolver;
            _dynamicApiControllerBuilder = dynamicApiControllerBuilder;
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

        public IBatchApiControllerBuilder<T> WithApiExplorer(bool isEnabled)
        {
            _isApiExplorerEnabled = isEnabled;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithServiceName(Func<Type, string> serviceNameSelector)
        {
            _serviceNameSelector = serviceNameSelector;
            return this;
        }

        public IBatchApiControllerBuilder<T> ForMethods(Action<IApiControllerActionBuilder> action)
        {
            _forMethodsAction = action;
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
                    (type.IsPublic || type.IsNestedPublic) && 
                    type.IsInterface && 
                    typeof(T).IsAssignableFrom(type) &&
                    _iocResolver.IsRegistered(type) &&
                    !RemoteServiceAttribute.IsExplicitlyDisabledFor(type)
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

                var builder = typeof(IDynamicApiControllerBuilder)
                    .GetMethod("For", BindingFlags.Public | BindingFlags.Instance)
                    .MakeGenericMethod(type)
                    .Invoke(_dynamicApiControllerBuilder, new object[] { serviceName });

                if (_filters != null)
                {
                    builder.GetType()
                        .GetMethod("WithFilters", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[] { _filters });
                }

                if (_isApiExplorerEnabled != null)
                {
                    builder.GetType()
                        .GetMethod("WithApiExplorer", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[] { _isApiExplorerEnabled });
                }

                if (_conventionalVerbs)
                {
                    builder.GetType()
                       .GetMethod("WithConventionalVerbs", BindingFlags.Public | BindingFlags.Instance)
                       .Invoke(builder, new object[0]);
                }

                if (_forMethodsAction != null)
                {
                    builder.GetType()
                        .GetMethod("ForMethods", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[] { _forMethodsAction });
                }

                builder.GetType()
                        .GetMethod("Build", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[0]);
            }
        }
        
        public static string GetConventionalServiceName(Type type)
        {
            var typeName = type.Name;

            typeName = typeName.RemovePostFix(ApplicationService.CommonPostfixes);

            if (typeName.Length > 1 && typeName.StartsWith("I") && char.IsUpper(typeName, 1))
            {
                typeName = typeName.Substring(1);
            }

            return typeName.ToCamelCase();
        }
    }
}