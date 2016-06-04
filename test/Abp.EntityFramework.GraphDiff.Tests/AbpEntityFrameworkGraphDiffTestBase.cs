using Abp.Collections;
using Abp.Modules;
using Abp.TestBase;

namespace Abp.EntityFramework.GraphDIff.Tests
{
    public class AbpEntityFrameworkGraphDiffTestBase : AbpIntegratedTestBase
    {
        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<AbpEntityFrameworkGraphDiffTestModule>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();
            //LocalIocManager.Register<IEntityMappingManager, EntityMappingManager>();
            //LocalIocManager.Register<IAbpEntityFrameworkGraphDiffModuleConfiguration, AbpEntityFrameworkGraphDiffModuleConfiguration>();
            //LocalIocManager.IocContainer.Register(Component.For<IAbpStartupConfiguration>().UsingFactoryMethod(() => Substitute.For<IAbpStartupConfiguration>()));
            //LocalIocManager.IocContainer.Register(Component.For<IAbpEntityFrameworkGraphDiffModuleConfiguration>().UsingFactoryMethod(() => Substitute.For<IAbpEntityFrameworkGraphDiffModuleConfiguration>()));

            //Substitute.For<IAbpEntityFrameworkGraphDiffModuleConfiguration>().EntityMappings = new List<EntityMapping>
            //    {
            //        MappingExpressionBuilder.For<MyMainEntity>(config => config.AssociatedCollection(entity => entity.MyDependentEntities)),
            //        MappingExpressionBuilder.For<MyDependentEntity>(config => config.AssociatedEntity(entity => entity.MyMainEntity))
            //    };
        }
    }
}
