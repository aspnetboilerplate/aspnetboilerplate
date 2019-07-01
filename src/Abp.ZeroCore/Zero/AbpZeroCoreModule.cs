using Abp.Authorization.Users;
using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;

namespace Abp.Zero
{
    [DependsOn(typeof(AbpZeroCommonModule))]
    public class AbpZeroCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpZeroConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AbpZeroCoreModule).GetAssembly(), "Abp.Zero.Localization.SourceExt"
                    )
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workerManager.Add(IocManager.Resolve<UserTokenExpirationWorker>());
            }
        }
    }
}
