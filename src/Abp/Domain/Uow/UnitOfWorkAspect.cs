using Abp.Dependency;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
	[AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, typeof(Abp.Authorization.AuthorizationAspect))]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class UnitOfWorkAspect : MethodInterceptionAspect, IInstanceScopedAspect
	{
		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IUnitOfWorkDefaultOptions UnitOfWorkDefaultOptions { get; set; }

		[ImportMember("UnitOfWorkDefaultOptions", IsRequired = true)]
		public Property<IUnitOfWorkDefaultOptions> UnitOfWorkDefaultOptionsProperty;

		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IUnitOfWorkManager UnitOfWorkManager { get; set; }

		[ImportMember("UnitOfWorkManager", IsRequired = true)]
		public Property<IUnitOfWorkManager> UnitOfWorkManagerProperty;

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

			IUnitOfWorkDefaultOptions unitOfWorkOptions = UnitOfWorkDefaultOptionsProperty.Get();
			IUnitOfWorkManager unitOfWorkManager = UnitOfWorkManagerProperty.Get();

			if (unitOfWorkOptions == null || unitOfWorkManager == null)
			{
				args.Proceed();
				return;
			}

			var unitOfWorkAttr = unitOfWorkOptions.GetUnitOfWorkAttributeOrNull((MethodInfo)args.Method);

			if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
			{
				args.Proceed();
				return;
			}

			using (var uow = unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
			{
				args.Proceed();
				uow.Complete();
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

			IUnitOfWorkDefaultOptions unitOfWorkOptions = UnitOfWorkDefaultOptionsProperty.Get();
			IUnitOfWorkManager unitOfWorkManager = UnitOfWorkManagerProperty.Get();

			if (unitOfWorkOptions == null || unitOfWorkManager == null)
			{
				await args.ProceedAsync();
				return;
			}

			var unitOfWorkAttr = unitOfWorkOptions.GetUnitOfWorkAttributeOrNull((MethodInfo)args.Method);

			if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
			{
				await args.ProceedAsync();
				return;
			}

			using (var uow = unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
			{
				await args.ProceedAsync();
				await uow.CompleteAsync();
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
