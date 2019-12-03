using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Reflection.Extensions;
using JetBrains.Annotations;

namespace Abp.AspNetCore.Configuration
{
    public class ControllerAssemblySettingList : List<AbpControllerAssemblySetting>
    {
        public List<AbpControllerAssemblySetting> GetSettings(Type controllerType)
        {
            return this.Where(controllerSetting => controllerSetting.Assembly == controllerType.GetAssembly()).ToList();
        }
    }
}