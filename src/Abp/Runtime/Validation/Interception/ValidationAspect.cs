using Abp.Aspects;
using Abp.Dependency;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Abp.Runtime.Validation.Interception
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class ValidationAspect : MethodInterceptionAspect, IInstanceScopedAspect
	{
		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
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

			if (iocManager == null)
			{
				args.Proceed();
				return;
			}

			if (!iocManager.IsRegistered<MethodInvocationValidator>())
			{
				args.Proceed();
				return;
			}

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

			if (iocManager == null)
			{
				await args.ProceedAsync();
				return;
			}

			if (!iocManager.IsRegistered<MethodInvocationValidator>())
			{
				await args.ProceedAsync();
				return;
			}

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
