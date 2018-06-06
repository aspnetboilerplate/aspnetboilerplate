using System;
using System.Collections.Generic;
using Abp.Authorization;
using Abp.Configuration;

namespace Abp.Web.Common.Tests
{
    public class AbpWebCommonTestModuleSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(
                    "AbpWebCommonTestModule.Test.Setting1",
                    "TestValue1",
                    scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User,
                    clientVisibilityProvider:new RequiresAuthenticationSettingClientVisibilityProvider()),

                new SettingDefinition(
                    "AbpWebCommonTestModule.Test.Setting2",
                    "TestValue2",
                    scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User,
                    clientVisibilityProvider:new RequiresPermissionSettingClientVisibilityProvider(new SimplePermissionDependency("Permission1"))),

                new SettingDefinition(
                    "AbpWebCommonTestModule.Test.Setting3",
                    "Test > Value3",
                    scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User,
                    clientVisibilityProvider:new RequiresPermissionSettingClientVisibilityProvider(new SimplePermissionDependency("Permission1")))
            };
        }
    }
}