using Abp.Application.Services;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Auditing
{
	[MulticastAttributeUsage(AllowExternalAssemblies = true)]
	[PSerializable]
	public class AuditingAspectProvider : AssemblyLevelAspect, IAspectProvider
	{
		public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
		{
			var assembly = (Assembly)targetElement;

			return assembly.GetTypes()
				.Where(t => t.IsClass && ShouldIntercept(t))
				.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				.Where(m => !m.IsSpecialName && m.GetSemanticInfo().IsSelectable)
				.Select(m => new AspectInstance(m, new AuditingAspect() { AspectPriority = 4, AttributePriority = 2 }));
		}

		private static bool ShouldIntercept(Type type)
		{
			if (typeof(IApplicationService).IsAssignableFrom(type))
			{
				return true;
			}

			if (type.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true))
			{
				return true;
			}

			if (type.GetMethods().Any(m => m.IsDefined(typeof(AuditedAttribute), true)))
			{
				return true;
			}

			return false;
		}
	}
}
