using Abp.Dependency;
using Abp.Localization.Sources.Resource;

namespace Taskever.Localization.Resources
{
    public class TaskeverLocalizationSource : ResourceFileLocalizationSource, ISingletonDependency
    {
        public const string SourceName = "Taskever";

        public TaskeverLocalizationSource()
            : base(SourceName, AppTexts.ResourceManager)
        {
        }
    }
}
