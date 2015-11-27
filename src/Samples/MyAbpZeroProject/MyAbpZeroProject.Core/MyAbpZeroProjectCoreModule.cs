using System.Reflection;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Abp.Localization.Sources.Xml;
using Abp.Modules;
using Abp.Zero;
using Abp.Domain.Uow;

namespace MyAbpZeroProject
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class MyAbpZeroProjectCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Remove the following line to disable multi-tenancy.
            Configuration.MultiTenancy.IsEnabled = true;

            Configuration.UnitOfWork.OverrideFilter(AbpDataFilters.MustHaveTenant, false);
            Configuration.UnitOfWork.OverrideFilter(AbpDataFilters.MayHaveTenant, false);

            

            //Add/remove localization sources here
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    MyAbpZeroProjectConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "MyAbpZeroProject.Localization.Source"
                        )
                    )
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
