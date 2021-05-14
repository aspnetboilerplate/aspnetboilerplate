using Abp.Modules;
using Abp.Runtime.Validation.Interception;

[assembly: ValidationAspectProvider()]

namespace Abp
{
	[DependsOn(typeof(AbpKernelModule))]
	public class AbpPostSharpModule : AbpModule
	{
	}
}
