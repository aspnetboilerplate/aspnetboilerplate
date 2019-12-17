using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Abp.Zero.Configuration;

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
            RegisterUserTokenExpirationWorker();
        }

        public override void PostInitialize()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                using (var entityTypes = IocManager.ResolveAsDisposable<IAbpZeroEntityTypes>())
                {
                    var implType = typeof(UserTokenExpirationWorker<,>)
                        .MakeGenericType(entityTypes.Object.Tenant, entityTypes.Object.User);
                    var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
                    workerManager.Add(IocManager.Resolve(implType) as IBackgroundWorker);
                }
            }
        }

        private void RegisterUserTokenExpirationWorker()
        {
            using (var entityTypes = IocManager.ResolveAsDisposable<IAbpZeroEntityTypes>())
            {
                var implType = typeof(UserTokenExpirationWorker<,>)
                    .MakeGenericType(entityTypes.Object.Tenant, entityTypes.Object.User);
                IocManager.Register(implType);
            }
        }
    }
}
