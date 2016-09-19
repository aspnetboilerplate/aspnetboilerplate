using System;

namespace Abp.AspNetCore.Configuration
{
    public class AbpControllerAssemblySettingBuilder: IAbpControllerAssemblySettingBuilder
    {
        private readonly AbpControllerAssemblySetting _setting;

        public AbpControllerAssemblySettingBuilder(AbpControllerAssemblySetting setting)
        {
            _setting = setting;
        }

        public AbpControllerAssemblySettingBuilder Where(Func<Type, bool> predicate)
        {
            _setting.TypePredicate = predicate;
            return this;
        }
    }
}