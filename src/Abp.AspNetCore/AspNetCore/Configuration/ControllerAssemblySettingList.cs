using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Reflection.Extensions;
using JetBrains.Annotations;

namespace Abp.AspNetCore.Configuration
{
    public class ControllerAssemblySettingList : List<AbpControllerAssemblySetting>
    {
        [CanBeNull]
        public AbpControllerAssemblySetting GetSettingOrNull(Type controllerType)
        {
            return this.FirstOrDefault(controllerSetting => controllerSetting.Assembly == controllerType.GetAssembly());
        }
    }
}