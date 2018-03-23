using System.Collections.Generic;
using System.Reflection;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Dependency;
using Abp.EntityFramework;
using Abp.EntityFramework.GraphDiff;
using Abp.EntityFramework.GraphDiff.Configuration;
using Abp.EntityFramework.GraphDiff.Mapping;
using Abp.Modules;
using Abp.TestBase.SampleApplication.ContacLists;
using Abp.TestBase.SampleApplication.People;
using Abp.TestBase.SampleApplication.Shop;
using AutoMapper;
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

            Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
            {
                CustomDtoMapper.CreateMappings(configuration, new MultiLingualMapContext
                {
                    SettingManager = IocManager.Resolve<ISettingManager>()
                });
            });

        }

        internal static class CustomDtoMapper
        {
            public static void CreateMappings(IMapperConfigurationExpression configuration, MultiLingualMapContext context)
            {
                configuration.CreateMultiLingualMap<Product, ProductTranslation, ProductListDto>(context);
            }
        }
    }
}
