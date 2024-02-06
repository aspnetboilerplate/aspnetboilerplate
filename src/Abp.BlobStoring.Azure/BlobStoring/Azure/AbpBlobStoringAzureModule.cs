using System.Reflection;
using Abp.Modules;

namespace Abp.BlobStoring.Azure
{
    [DependsOn(typeof(AbpBlobStoringModule))]
    public class AbpBlobStoringAzureModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}