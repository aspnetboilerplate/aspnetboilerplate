using System.Collections.Generic;
using System.Reflection;
using Abp.AutoMapper;
using Abp.EntityFramework;
using Abp.EntityFramework.GraphDiff;
using Abp.EntityFramework.GraphDiff.Configuration;
using Abp.EntityFramework.GraphDiff.Mapping;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase.SampleApplication.ContactLists;
using Abp.TestBase.SampleApplication.People;
using RefactorThis.GraphDiff;

namespace Abp.TestBase.SampleApplication
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpAutoMapperModule),
        typeof(AbpEntityFrameworkGraphDiffModule))]
    public class SampleApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Features.Providers.Add<SampleFeatureProvider>();

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    SampleApplicationConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(SampleApplicationModule).GetAssembly(), "Abp.TestBase.SampleApplication.Localization.Sources"
                    )));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpEfGraphDiff().EntityMappings = new List<EntityMapping>
            {
                MappingExpressionBuilder.For<ContactList>(
                    config => config.AssociatedCollection(entity => entity.People)),
                MappingExpressionBuilder.For<Person>(config => config.AssociatedEntity(entity => entity.ContactList))
            };
        }
    }
}
