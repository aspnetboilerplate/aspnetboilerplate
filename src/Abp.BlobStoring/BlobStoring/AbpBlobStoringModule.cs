using System.Reflection;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.BlobStoring
{
    public class AbpBlobStoringModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpBlobStoringOptions>();

            IocManager.Register(typeof(IBlobContainer<>), typeof(BlobContainer<>), DependencyLifeStyle.Transient);
            IocManager.Register<IBlobContainer, BlobContainer<DefaultContainer>>(DependencyLifeStyle.Transient);

        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
