using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources.Resource;

namespace Taskever.Localization.Resources
{
    public class TaskeverLocalizationSource : ResourceFileLocalizationSource, ISingletonDependency
    {
        public TaskeverLocalizationSource()
            : base("Taskever", AppTexts.ResourceManager)
        {
        }
    }
}
