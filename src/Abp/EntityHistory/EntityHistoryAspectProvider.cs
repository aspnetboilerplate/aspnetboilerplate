using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.EntityHistory
{
	[MulticastAttributeUsage(AllowExternalAssemblies = true)]
	[PSerializable]
	public class EntityHistoryAspectProvider : AssemblyLevelAspect, IAspectProvider
	{
		public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
		{
			var assembly = (Assembly)targetElement;

			return assembly.GetTypes()
				.Where(t => t.IsClass && ShouldIntercept(t))
				.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				.Where(m => !m.IsSpecialName && m.GetSemanticInfo().IsSelectable)
				.Select(m => new AspectInstance(m, new EntityHistoryAspect() { AspectPriority = 5, AttributePriority = 2 }));
		}

		private static bool ShouldIntercept(Type type)
		{
			if (type.GetTypeInfo().IsDefined(typeof(UseCaseAttribute), true))
			{
				return true;
			}

			if (type.GetMethods().Any(m => m.IsDefined(typeof(UseCaseAttribute), true)))
			{
				return true;
			}

			return false;
		}
	}
}
