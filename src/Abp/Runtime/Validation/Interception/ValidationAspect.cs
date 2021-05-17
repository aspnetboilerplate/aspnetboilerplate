using Abp.Aspects;
using Abp.Dependency;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Abp.Runtime.Validation.Interception
{
	[AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(Abp.Authorization.AuthorizationAspect))]
	[AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, typeof(Abp.Auditing.AuditingAspect))]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class ValidationAspect : MethodInterceptionAspect, IInstanceScopedAspect
	{
		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IIocResolver IocResolver { get; set; }

		[ImportMember("IocResolver", IsRequired = true)]
		public Property<IIocResolver> IocResolverProperty;

		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IPostSharpOptions PostSharpOptions { get; set; }

		[ImportMember("PostSharpOptions", IsRequired = false)]
		public Property<IPostSharpOptions> PostSharpOptionsProperty;

		public ValidationAspect()
		{
		}

		public override void OnInvoke(MethodInterceptionArgs args)
		{
			IPostSharpOptions postSharpOptions = PostSharpOptionsProperty?.Get();

			if (postSharpOptions == null || !postSharpOptions.EnablePostSharp)
			{
				args.Proceed();
				return;
			}

			if (AbpCrossCuttingConcerns.IsApplied(args.Instance, AbpCrossCuttingConcerns.Validation))
			{
				args.Proceed();
				return;
			}

			IIocResolver iocResolver = IocResolverProperty.Get();

			using (var validator = iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
			{
				validator.Object.Initialize((System.Reflection.MethodInfo)args.Method, args.Arguments.ToArray());
				validator.Object.Validate();
			}

			args.Proceed();
		}

		public override async Task OnInvokeAsync(MethodInterceptionArgs args)
		{
			IPostSharpOptions postSharpOptions = PostSharpOptionsProperty?.Get();

			if (postSharpOptions == null || !postSharpOptions.EnablePostSharp)
			{
				await args.ProceedAsync();
				return;
			}

			if (AbpCrossCuttingConcerns.IsApplied(args.Instance, AbpCrossCuttingConcerns.Validation))
			{
				await args.ProceedAsync();
				return;
			}

			IIocResolver iocResolver = IocResolverProperty.Get();

			using (var validator = iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
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
