using Abp.Application.Services;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Runtime.Validation.Interception
{
	[MulticastAttributeUsage(AllowExternalAssemblies = true)]
	[PSerializable]
	public class ValidationAspectProvider : AssemblyLevelAspect, IAspectProvider
	{
		public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
		{
			var assembly = (Assembly)targetElement;

			return assembly.GetTypes()
				.Where(t => typeof(IApplicationService).IsAssignableFrom(t) && t.IsClass)
				.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				.Where(m => !m.IsSpecialName && m.GetSemanticInfo().IsSelectable)
				.Select(m => new AspectInstance(m, new ValidationAspect() { AspectPriority = 3, AttributePriority = 2 }));
		}
	}
}
