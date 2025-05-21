using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Domain.Uow;
using Abp.Web.Models;
using Abp.Web.Results.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Abp.AspNetCore.Configuration;

public class AbpAspNetCoreConfiguration : IAbpAspNetCoreConfiguration
{
    public WrapResultAttribute DefaultWrapResultAttribute { get; }

    public ResponseCacheAttribute DefaultResponseCacheAttributeForControllers { get; set; }

    public ResponseCacheAttribute DefaultResponseCacheAttributeForAppServices { get; set; }

    public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

    public List<Type> FormBodyBindingIgnoredTypes { get; }

    public ControllerAssemblySettingList ControllerAssemblySettings { get; }

    public bool IsValidationEnabledForControllers { get; set; }

    public bool IsAuditingEnabled { get; set; }

    public bool SetNoCacheForAjaxResponses { get; set; }

    public bool UseMvcDateTimeFormatForAppServices { get; set; }

    public List<string> InputDateTimeFormats { get; set; }

    public string OutputDateTimeFormat { get; set; }

    public List<Action<IEndpointRouteBuilder>> EndpointConfiguration { get; }

    public WrapResultFilterCollection WrapResultFilters { get; }

    public AbpAspNetCoreConfiguration()
    {
        DefaultWrapResultAttribute = new WrapResultAttribute();
        DefaultResponseCacheAttributeForControllers = null;
        DefaultResponseCacheAttributeForAppServices = null;
        DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
        ControllerAssemblySettings = new ControllerAssemblySettingList();
        FormBodyBindingIgnoredTypes = new List<Type>();
        EndpointConfiguration = new List<Action<IEndpointRouteBuilder>>();
        WrapResultFilters = new WrapResultFilterCollection();
        IsValidationEnabledForControllers = true;
        SetNoCacheForAjaxResponses = true;
        IsAuditingEnabled = true;
        UseMvcDateTimeFormatForAppServices = false;
        InputDateTimeFormats = null;
        OutputDateTimeFormat = null;
    }

    public AbpControllerAssemblySettingBuilder CreateControllersForAppServices(
        Assembly assembly,
        string moduleName = AbpControllerAssemblySetting.DefaultServiceModuleName,
        bool useConventionalHttpVerbs = true)
    {
        var setting = new AbpControllerAssemblySetting(moduleName, assembly, useConventionalHttpVerbs);
        ControllerAssemblySettings.Add(setting);
        return new AbpControllerAssemblySettingBuilder(setting);
    }
}