using Abp.Aspects;
using Abp.Dependency;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System.Threading.Tasks;

namespace Abp.Runtime.Validation.Interception
{
	[MulticastAttributeUsage(Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class ValidationAspect : MethodInterceptionAspect, IInstanceScopedAspect
	{
		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		public IIocManager IocManager { get; set; }

		[ImportMember("IocManager", IsRequired = true)]
		public Property<IIocManager> IocManagerProperty;

		public ValidationAspect()
		{
		}

		public override void OnInvoke(MethodInterceptionArgs args)
		{
			if (AbpCrossCuttingConcerns.IsApplied(args.Instance, AbpCrossCuttingConcerns.Validation))
			{
				args.Proceed();
				return;
			}

			var iocManager = IocManagerProperty.Get();

			using (var validator = iocManager.ResolveAsDisposable<MethodInvocationValidator>())
			{
				validator.Object.Initialize((System.Reflection.MethodInfo)args.Method, args.Arguments.ToArray());
				validator.Object.Validate();
			}

			args.Proceed();
		}

		public override async Task OnInvokeAsync(MethodInterceptionArgs args)
		{
			if (AbpCrossCuttingConcerns.IsApplied(args.Instance, AbpCrossCuttingConcerns.Validation))
			{
				await args.ProceedAsync();
				return;
			}

			var iocManager = IocManagerProperty.Get();

			using (var validator = iocManager.ResolveAsDisposable<MethodInvocationValidator>())
			{
				validator.Object.Initialize((System.Reflection.MethodInfo)args.Method, args.Arguments.ToArray());
				validator.Object.Validate();
			}

			await args.ProceedAsync();
		}

		public object CreateInstance(AdviceArgs adviceArgs)
		{
			return MemberwiseClone();
		}

		public void RuntimeInitializeInstance()
		{
		}
	}
}
