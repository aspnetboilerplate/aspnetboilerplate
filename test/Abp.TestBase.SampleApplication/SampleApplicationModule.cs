using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Dependency;
using Abp.EntityFramework;
using Abp.EntityFramework.GraphDiff;
using Abp.EntityFramework.GraphDiff.Configuration;
using Abp.EntityFramework.GraphDiff.Mapping;
using Abp.Localization;
using Abp.Modules;
using Abp.TestBase.SampleApplication.ContacLists;
using Abp.TestBase.SampleApplication.People;
using Abp.TestBase.SampleApplication.Shop;
using Abp.UI.Inputs;
using AutoMapper;
using RefactorThis.GraphDiff;

namespace Abp.TestBase.SampleApplication
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpAutoMapperModule), typeof(AbpEntityFrameworkGraphDiffModule))]
    public class SampleApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Features.Providers.Add<SampleFeatureProvider>();
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpEfGraphDiff().EntityMappings = new List<EntityMapping>
            {
                MappingExpressionBuilder.For<ContactList>(config => config.AssociatedCollection(entity => entity.People)),
                MappingExpressionBuilder.For<Person>(config => config.AssociatedEntity(entity => entity.ContactList))
            };
        }
    }

    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Product, ProductListDto>().AfterMap((source, destination, context) =>
            {
                //todo@ismail => how to resolve ISettingManager here...
                //var settingManager = IocManager.Instance.Resolve<ISettingManager>();
                var currentLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                var defaultLanguage = "en"; //settingManager.GetSettingValue(LocalizationSettingNames.DefaultLanguage);

                if (source.Translations.Any(t => t.Language == currentLanguage))
                {
                    var currentTranlation = source.Translations.Single(pt => pt.Language == currentLanguage);
                    context.Mapper.Map(currentTranlation, destination);
                }
                else if (source.Translations.Any(t => t.Language == defaultLanguage))
                {
                    var defaultTranlation = source.Translations.Single(pt => pt.Language == defaultLanguage);
                    context.Mapper.Map(defaultTranlation, destination);
                }
                else if (source.Translations.Any())
                {
                    var tranlation = source.Translations.First();
                    context.Mapper.Map(tranlation, destination);
                }
            });
        }
    }

    public class MultiLingualTranslationMapAction : IMappingAction<Product, ProductListDto>
    {
        public void Process(Product source, ProductListDto destination)
        {
            if (source.Translations != null && source.Translations.Any(pt => pt.Language == "en"))
            {
                var productTranlation = source.Translations.Single(pt => pt.Language == "en");
                Mapper.Map(productTranlation, destination);
            }
        }
    }
}
