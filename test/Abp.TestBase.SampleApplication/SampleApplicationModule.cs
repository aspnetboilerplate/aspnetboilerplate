using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Entities;
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
using Newtonsoft.Json;
using RefactorThis.GraphDiff;

namespace Abp.TestBase.SampleApplication
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpAutoMapperModule), typeof(AbpEntityFrameworkGraphDiffModule))]
    public class SampleApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Features.Providers.Add<SampleFeatureProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpEfGraphDiff().EntityMappings = new List<EntityMapping>
            {
                MappingExpressionBuilder.For<ContactList>(config => config.AssociatedCollection(entity => entity.People)),
                MappingExpressionBuilder.For<Person>(config => config.AssociatedEntity(entity => entity.ContactList))
            };

            Configuration.Modules.AbpAutoMapper().Configurators.Add((configuration) => CustomDtoMapper.CreateMappings(configuration, IocManager.Resolve<ISettingManager>()));
        }
    }

    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration, ISettingManager settingManager)
        {
            configuration.CreateMultiLingualMap<Product, ProductTranslation, ProductListDto>(settingManager);
        }
    }

    public static class AutoMapperConfigurationExtensions
    {
        public static void CreateMultiLingualMap<TMultiLingualEntity, TTranslation, TDestination>(this IMapperConfigurationExpression configuration, ISettingManager settingManager)
            where TTranslation : class, IEntity, IEntityTranslation
            where TMultiLingualEntity : IMultiLingualEntity<TTranslation>
        {
            configuration.CreateMap<TMultiLingualEntity, TDestination>().AfterMap((source, destination, context) =>
            {
                var currentLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                var defaultLanguage = settingManager.GetSettingValue(LocalizationSettingNames.DefaultLanguage);

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
}
