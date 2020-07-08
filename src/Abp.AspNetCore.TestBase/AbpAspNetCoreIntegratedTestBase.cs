using System;
using System.Linq;
using System.Net.Http;
using Abp.AspNetCore.Configuration;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Extensions;
using Abp.TestBase.Runtime.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.AspNetCore.TestBase
{
    public abstract class AbpAspNetCoreIntegratedTestBase<TStartup>
        where TStartup : class
    {
        protected TestServer Server { get; }

        public HttpClient Client { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected IIocManager IocManager { get; }

        protected TestAbpSession AbpSession { get; }

        protected string ServiceModuleName { get; }

        protected AbpAspNetCoreIntegratedTestBase()
        {
            var builder = CreateWebHostBuilder();
            Server = CreateTestServer(builder);
            Client = Server.CreateClient();

            ServiceProvider = Server.Host.Services;
            IocManager = ServiceProvider.GetRequiredService<IIocManager>();
            AbpSession = ServiceProvider.GetRequiredService<TestAbpSession>();
            ServiceModuleName = AbpControllerAssemblySetting.DefaultServiceModuleName;
        }

        protected virtual IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseStartup<TStartup>();
        }

        protected virtual TestServer CreateTestServer(IWebHostBuilder builder)
        {
            return new TestServer(builder);
        }

        #region GetUrl

        /// <summary>
        /// Gets default URL for given controller type.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        protected virtual string GetUrl<TController>()
        {
            var controllerName = typeof(TController).Name;
            string area = "/";
            if (!controllerName.EndsWith("Controller"))
            {
                area = $"/api/services/{ServiceModuleName}/";
            }
            return area + controllerName.RemovePostFix("Controller", "AppService", "ApplicationService", "Service");
        }

        /// <summary>
        /// Gets default URL for given controller type's given action.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        protected virtual string GetUrl<TController>(string actionName)
        {
            return GetUrl<TController>() + "/" + actionName;
        }

        /// <summary>
        /// Gets default URL for given controller type's given action with query string parameters (as anonymous object).
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        protected virtual string GetUrl<TController>(string actionName, object queryStringParamsAsAnonymousObject)
        {
            var url = GetUrl<TController>(actionName);

            var dictionary = new RouteValueDictionary(queryStringParamsAsAnonymousObject);
            if (dictionary.Any())
            {
                url += "?" + dictionary.Select(d => $"{d.Key}={d.Value}").JoinAsString("&");
            }

            return url;
        }

        #endregion

        #region Resolve

        /// <summary>
        /// A shortcut to resolve an object from <see cref="IocManager"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The object instance</returns>
        protected T Resolve<T>()
        {
            return IocManager.Resolve<T>();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="IocManager"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected T Resolve<T>(object argumentsAsAnonymousType)
        {
            return IocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="IocManager"/>.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type)
        {
            return IocManager.Resolve(type);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="IocManager"/>.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type, object argumentsAsAnonymousType)
        {
            return IocManager.Resolve(type, argumentsAsAnonymousType);
        }

        #endregion
    }
}
