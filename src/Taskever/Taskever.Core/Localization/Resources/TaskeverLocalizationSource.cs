using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Abp.Localization;

namespace Taskever.Localization.Resources
{
    public class TaskeverLocalizationSource : ResourceFileLocalizationSource, ITaskeverLocalizationSource
    {
        public TaskeverLocalizationSource()
            : base("Taskever", AppTexts.ResourceManager)
        {
        }
    }

    public interface ITaskeverLocalizationSource : ILocalizationSource
    {
    }
}
