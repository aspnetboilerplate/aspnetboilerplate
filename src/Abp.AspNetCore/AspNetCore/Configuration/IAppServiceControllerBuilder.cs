using System;

namespace Abp.AspNetCore.Configuration
{
    //TODO: Create a fluent configuration API
    internal interface IAppServiceControllerBuilder
    {
        IAppServiceControllerBuilder WithModuleName(string moduleName);
        IAppServiceControllerBuilder WithModuleName(Func<Type, string> moduleNameSelector);
        IAppServiceControllerBuilder WithServiceName(Func<Type, string> serviceNameSelector, string serviceName);
        IAppServiceControllerBuilder WithConventionalVerbs();
        
        //TODO: IAppServiceControllerBuilder WithFilters();
    }
}