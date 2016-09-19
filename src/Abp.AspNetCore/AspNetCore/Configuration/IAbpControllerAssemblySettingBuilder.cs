using System;

namespace Abp.AspNetCore.Configuration
{
    public interface IAbpControllerAssemblySettingBuilder
    {
        AbpControllerAssemblySettingBuilder Where(Func<Type, bool> predicate);
    }
}