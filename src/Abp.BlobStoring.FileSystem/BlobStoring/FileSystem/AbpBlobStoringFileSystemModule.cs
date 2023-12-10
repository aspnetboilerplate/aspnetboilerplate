using System.Reflection;
using Abp.Modules;

namespace Abp.BlobStoring.FileSystem
{
    [DependsOn(typeof(AbpBlobStoringModule))]
    public class AbpBlobStoringFileSystemModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
