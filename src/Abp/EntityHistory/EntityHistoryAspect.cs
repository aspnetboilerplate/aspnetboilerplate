using Abp.Dependency;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.EntityHistory
{
	[AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(Abp.Auditing.AuditingAspect))]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class EntityHistoryAspect : MethodInterceptionAspect, IInstanceScopedAspect
	{
		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IEntityChangeSetReasonProvider ReasonProvider { get; set; }

		[ImportMember("ReasonProvider", IsRequired = true)]
		public Property<IEntityChangeSetReasonProvider> ReasonProviderProperty;

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

			var reasonProvider = ReasonProviderProperty.Get();

			var methodInfo = (MethodInfo)args.Method;
			var useCaseAttribute = methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
				?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault();

			if (useCaseAttribute?.Description == null)
			{
				args.Proceed();
				return;
			}

			using (reasonProvider.Use(useCaseAttribute.Description))
			{
				args.Proceed();
			}
		}

		public override async Task OnInvokeAsync(MethodInterceptionArgs args)
		{
			IPostSharpOptions postSharpOptions = PostSharpOptionsProperty?.Get();

			if (postSharpOptions == null || !postSharpOptions.EnablePostSharp)
			{
				await args.ProceedAsync();
				return;
			}

			var reasonProvider = ReasonProviderProperty.Get();

			var methodInfo = (MethodInfo)args.Method;
			var useCaseAttribute = methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
				?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault();

			if (useCaseAttribute?.Description == null)
			{
				await args.ProceedAsync();
				return;
			}

			using (reasonProvider.Use(useCaseAttribute.Description))
			{
				await args.ProceedAsync();
			}
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
