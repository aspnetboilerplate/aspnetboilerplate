using Abp.Dependency;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class UnitOfWorkAspect : MethodInterceptionAspect, IInstanceScopedAspect
	{
		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IIocManager IocManager { get; set; }

		[ImportMember("IocManager", IsRequired = true)]
		public Property<IIocManager> IocManagerProperty;

		public override void OnInvoke(MethodInterceptionArgs args)
		{
			var iocManager = IocManagerProperty.Get();

			if (iocManager == null)
			{
				args.Proceed();
				return;
			}

			if (!iocManager.IsRegistered<IUnitOfWorkDefaultOptions>() || !iocManager.IsRegistered<IUnitOfWorkManager>())
			{
				args.Proceed();
				return;
			}

			using (var unitOfWorkOptions = iocManager.ResolveAsDisposable<IUnitOfWorkDefaultOptions>())
			using (var unitOfWorkManager = iocManager.ResolveAsDisposable<IUnitOfWorkManager>())
			{
				if (unitOfWorkOptions?.Object == null || unitOfWorkManager?.Object == null)
				{
					args.Proceed();
					return;
				}

				var unitOfWorkAttr = unitOfWorkOptions.Object.GetUnitOfWorkAttributeOrNull((MethodInfo)args.Method);

				if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
				{
					args.Proceed();
					return;
				}

				using (var uow = unitOfWorkManager.Object.Begin(unitOfWorkAttr.CreateOptions()))
				{
					args.Proceed();
					uow.Complete();
				}
			}

		}

		public override async Task OnInvokeAsync(MethodInterceptionArgs args)
		{
			var iocManager = IocManagerProperty.Get();

			if (iocManager == null)
			{
				await args.ProceedAsync();
				return;
			}

			if (!iocManager.IsRegistered<IUnitOfWorkDefaultOptions>() || !iocManager.IsRegistered<IUnitOfWorkManager>())
			{
				await args.ProceedAsync();
				return;
			}

			using (var unitOfWorkOptions = iocManager.ResolveAsDisposable<IUnitOfWorkDefaultOptions>())
			using (var unitOfWorkManager = iocManager.ResolveAsDisposable<IUnitOfWorkManager>())
			{
				if (unitOfWorkOptions?.Object == null || unitOfWorkManager?.Object == null)
				{
					await args.ProceedAsync();
					return;
				}

				var unitOfWorkAttr = unitOfWorkOptions.Object.GetUnitOfWorkAttributeOrNull((MethodInfo)args.Method);

				if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
				{
					await args.ProceedAsync();
					return;
				}

				using (var uow = unitOfWorkManager.Object.Begin(unitOfWorkAttr.CreateOptions()))
				{
					await args.ProceedAsync();
					await uow.CompleteAsync();
				}
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
