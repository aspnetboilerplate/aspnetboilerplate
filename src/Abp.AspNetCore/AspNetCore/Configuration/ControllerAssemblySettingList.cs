using System;
using System.Collections.Generic;
using System.Linq;

namespace Abp.AspNetCore.Configuration
{
    public class ControllerAssemblySettingList : List<AbpControllerAssemblySetting>
    {
        public AbpControllerAssemblySetting GetSettingOrNull(Type controllerType)
        {
            return this.FirstOrDefault(controllerSetting => controllerSetting.Assembly == controllerType.Assembly);
        }
    }
}