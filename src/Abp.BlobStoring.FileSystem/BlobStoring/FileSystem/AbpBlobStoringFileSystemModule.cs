using System.Reflection;
using Abp.BlobStoring;
using Abp.Modules;

namespace Abp.BlobStoring.FileSystem
{
    [DependsOn(typeof(AbpBlobStoringModule))]
    public class AbpBlobStoringFileSystemModule : AbpModule
    {
        public override void PreInitialize()
        {
            //IocManager.Register<IBlobContainer, BlobContainer>(DependencyLifeStyle.Transient);

        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
