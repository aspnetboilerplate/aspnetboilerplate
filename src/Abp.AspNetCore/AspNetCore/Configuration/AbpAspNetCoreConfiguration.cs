using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.AspNetCore.Configuration
{
    public class AbpAspNetCoreConfiguration : IAbpAspNetCoreConfiguration
    {
        public WrapResultAttribute DefaultWrapResultAttribute { get; }

        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        public List<Type> FormBodyBindingIgnoredTypes { get; }

        public ControllerAssemblySettingList ControllerAssemblySettings { get; }

        public bool IsValidationEnabledForControllers { get; set; }

        public bool IsAuditingEnabled { get; set; }

        public bool SetNoCacheForAjaxResponses { get; set; }

        public AbpAspNetCoreConfiguration()
        {
            DefaultWrapResultAttribute = new WrapResultAttribute();
            DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            ControllerAssemblySettings = new ControllerAssemblySettingList();
            FormBodyBindingIgnoredTypes = new List<Type>();
            IsValidationEnabledForControllers = true;
            SetNoCacheForAjaxResponses = true;
            IsAuditingEnabled = true;
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
}