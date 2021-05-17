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
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.Auditing
{
	[AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(Abp.Runtime.Validation.Interception.ValidationAspect))]
	[AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, typeof(Abp.EntityHistory.EntityHistoryAspect))]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast, AllowExternalAssemblies = true)]
	[PSerializable]
	public class AuditingAspect : MethodInterceptionAspect, IInstanceScopedAspect
	{
		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IAuditingHelper AuditingHelper { get; set; }

		[ImportMember("AuditingHelper", IsRequired = true)]
		public Property<IAuditingHelper> AuditingHelperProperty;

		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IAuditingConfiguration AuditingConfiguration { get; set; }

		[ImportMember("AuditingConfiguration", IsRequired = true)]
		public Property<IAuditingConfiguration> AuditingConfigurationProperty;

		[IntroduceMember(Visibility = Visibility.Public, OverrideAction = MemberOverrideAction.Ignore)]
		[NotMapped]
		[CopyCustomAttributes(typeof(NotMappedAttribute))]
		public IAuditSerializer AuditSerializer { get; set; }

		[ImportMember("AuditSerializer", IsRequired = true)]
		public Property<IAuditSerializer> AuditSerializerProperty;

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

			IAuditingHelper auditingHelper = AuditingHelperProperty.Get();
			IAuditingConfiguration auditingConfiguration = AuditingConfigurationProperty.Get();
			IAuditSerializer auditSerializer = AuditSerializerProperty.Get();

			if (AbpCrossCuttingConcerns.IsApplied(args.Instance, AbpCrossCuttingConcerns.Auditing))
			{
				args.Proceed();
				return;
			}

			if (!auditingHelper.ShouldSaveAudit((MethodInfo)args.Method))
			{
				args.Proceed();
				return;
			}

			var auditInfo = auditingHelper.CreateAuditInfo(args.Instance.GetType(), (MethodInfo)args.Method, args.Arguments.ToArray());

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

				if (auditingConfiguration.SaveReturnValues && args.ReturnValue != null)
				{
					auditInfo.ReturnValue = auditSerializer.Serialize(args.ReturnValue);
				}

				auditingHelper.Save(auditInfo);
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

			IAuditingHelper auditingHelper = AuditingHelperProperty?.Get();
			IAuditingConfiguration auditingConfiguration = AuditingConfigurationProperty?.Get();
			IAuditSerializer auditSerializer = AuditSerializerProperty?.Get();

			if (auditingHelper == null || auditingConfiguration == null || auditSerializer == null)
			{
				await args.ProceedAsync();
				return;
			}

			if (AbpCrossCuttingConcerns.IsApplied(args.Instance, AbpCrossCuttingConcerns.Auditing))
			{
				await args.ProceedAsync();
				return;
			}

			if (!auditingHelper.ShouldSaveAudit((MethodInfo)args.Method))
			{
				await args.ProceedAsync();
				return;
			}

			var auditInfo = auditingHelper.CreateAuditInfo(args.Instance.GetType(), (MethodInfo)args.Method, args.Arguments.ToArray());

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

				if (auditingConfiguration.SaveReturnValues && args.ReturnValue != null)
				{
					auditInfo.ReturnValue = auditSerializer.Serialize(args.ReturnValue);
				}

				await auditingHelper.SaveAsync(auditInfo);
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
