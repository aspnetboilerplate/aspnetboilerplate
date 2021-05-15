using Abp.Aspects;
using Abp.Dependency;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Auditing
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class AuditingAspect : MethodInterceptionAspect, IInstanceScopedAspect
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

			if (!iocManager.IsRegistered<IAuditingHelper>() 
				|| !iocManager.IsRegistered<IAuditingConfiguration>()
				|| !iocManager.IsRegistered<IAuditSerializer>())
			{
				args.Proceed();
				return;
			}

			using (var auditingHelper = iocManager.ResolveAsDisposable<IAuditingHelper>())
			using (var auditingConfiguration = iocManager.ResolveAsDisposable<IAuditingConfiguration>())
			using (var auditSerializer = iocManager.ResolveAsDisposable<IAuditSerializer>())
			{
				if (auditingHelper?.Object == null || auditingConfiguration?.Object == null || auditSerializer?.Object == null)
				{
					args.Proceed();
					return;
				}

				if (AbpCrossCuttingConcerns.IsApplied(args.Instance, AbpCrossCuttingConcerns.Auditing))
				{
					args.Proceed();
					return;
				}

				if (!auditingHelper.Object.ShouldSaveAudit((MethodInfo)args.Method))
				{
					args.Proceed();
					return;
				}

				var auditInfo = auditingHelper.Object.CreateAuditInfo(args.Instance.GetType(), (MethodInfo)args.Method, args.Arguments.ToArray());

				var stopwatch = Stopwatch.StartNew();

				try
				{
					args.Proceed();
				}
				catch (Exception ex)
				{
					auditInfo.Exception = ex;
					throw;
				}
				finally
				{
					stopwatch.Stop();
					auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

					if (auditingConfiguration.Object.SaveReturnValues && args.ReturnValue != null)
					{
						auditInfo.ReturnValue = auditSerializer.Object.Serialize(args.ReturnValue);
					}

					auditingHelper.Object.Save(auditInfo);
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

			if (!iocManager.IsRegistered<IAuditingHelper>()
				|| !iocManager.IsRegistered<IAuditingConfiguration>()
				|| !iocManager.IsRegistered<IAuditSerializer>())
			{
				await args.ProceedAsync();
				return;
			}

			using (var auditingHelper = iocManager.ResolveAsDisposable<IAuditingHelper>())
			using (var auditingConfiguration = iocManager.ResolveAsDisposable<IAuditingConfiguration>())
			using (var auditSerializer = iocManager.ResolveAsDisposable<IAuditSerializer>())
			{
				if (auditingHelper?.Object == null || auditingConfiguration?.Object == null || auditSerializer?.Object == null)
				{
					await args.ProceedAsync();
					return;
				}

				if (AbpCrossCuttingConcerns.IsApplied(args.Instance, AbpCrossCuttingConcerns.Auditing))
				{
					await args.ProceedAsync();
					return;
				}

				if (!auditingHelper.Object.ShouldSaveAudit((MethodInfo)args.Method))
				{
					await args.ProceedAsync();
					return;
				}

				var auditInfo = auditingHelper.Object.CreateAuditInfo(args.Instance.GetType(), (MethodInfo)args.Method, args.Arguments.ToArray());

				var stopwatch = Stopwatch.StartNew();

				try
				{
					await args.ProceedAsync();
				}
				catch (Exception ex)
				{
					auditInfo.Exception = ex;
					throw;
				}
				finally
				{
					stopwatch.Stop();
					auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

					if (auditingConfiguration.Object.SaveReturnValues && args.ReturnValue != null)
					{
						auditInfo.ReturnValue = auditSerializer.Object.Serialize(args.ReturnValue);
					}

					await auditingHelper.Object.SaveAsync(auditInfo);
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
