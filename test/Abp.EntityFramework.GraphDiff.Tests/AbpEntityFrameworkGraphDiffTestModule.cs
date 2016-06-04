using System.Collections.Generic;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.EntityFramework.GraphDIff.Tests.Mapping;
using Abp.GraphDiff;
using Abp.GraphDiff.Configuration;
using Abp.GraphDiff.Mapping;
using Abp.Modules;
using RefactorThis.GraphDiff;

namespace Abp.EntityFramework.GraphDIff.Tests
{
    [DependsOn(typeof(AbpEntityFrameworkGraphDiffModule))]
    public class AbpEntityFrameworkGraphDiffTestModule : AbpModule
    {
        public override void Initialize()
        {
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpEfGraphDiff().EntityMappings = new List<EntityMapping>
            {
                MappingExpressionBuilder.For<EntityMappingManager_Tests.MyMainEntity>(config => config.AssociatedCollection(entity => entity.MyDependentEntities)),
                MappingExpressionBuilder.For<EntityMappingManager_Tests.MyDependentEntity>(config => config.AssociatedEntity(entity => entity.MyMainEntity))
            };
        }
    }
}