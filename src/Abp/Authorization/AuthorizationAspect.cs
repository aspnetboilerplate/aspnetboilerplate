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

namespace Abp.Authorization
{
	[AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(Abp.Domain.Uow.UnitOfWorkAspect))]
	[AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, typeof(Abp.Runtime.Validation.Interception.ValidationAspect))]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class AuthorizationAspect : MethodInterceptionAspect, IInstanceScopedAspect
	{
		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IAuthorizationHelper AuthorizationHelper { get; set; }

		[ImportMember("AuthorizationHelper", IsRequired = true)]
		public Property<IAuthorizationHelper> AuthorizationHelperProperty;

		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IPostSharpOptions PostSharpOptions { get; set; }

		[ImportMember("PostSharpOptions", IsRequired = false)]
		public Property<IPostSharpOptions> PostSharpOptionsProperty;

		public override void OnInvoke(MethodInterceptionArgs args)
		{
			IPostSharpOptions postSharpOptions = PostSharpOptionsProperty?.Get();

			if (postSharpOptions == null || !postSharpOptions.EnablePostSharp)
			{
				args.Proceed();
				return;
			}

			AuthorizationHelperProperty.Get().Authorize((System.Reflection.MethodInfo)args.Method, args.Instance.GetType());
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

			await AuthorizationHelperProperty.Get().AuthorizeAsync((System.Reflection.MethodInfo)args.Method, args.Instance.GetType());
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
