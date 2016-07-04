using System.Collections.Generic;
using System.Reflection;
using Abp.EntityFramework.GraphDiff;
using Abp.EntityFramework.GraphDiff.Configuration;
using Abp.EntityFramework.GraphDiff.Mapping;
using Abp.EntityFramework.GraphDIff.Tests.Entities;
using Abp.Modules;
using Abp.TestBase;
using RefactorThis.GraphDiff;

namespace Abp.EntityFramework.GraphDIff.Tests
{
    [DependsOn(typeof(AbpEntityFrameworkGraphDiffModule), typeof(AbpTestBaseModule))]
    public class AbpEntityFrameworkGraphDiffTestModule : AbpModule
    {
        public override void Initialize()
        {
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpEfGraphDiff().EntityMappings = new List<EntityMapping>
            {
                MappingExpressionBuilder.For<MyMainEntity>(config => config.AssociatedCollection(entity => entity.MyDependentEntities)),
                MappingExpressionBuilder.For<MyDependentEntity>(config => config.AssociatedEntity(entity => entity.MyMainEntity))
            };
        }
    }
}