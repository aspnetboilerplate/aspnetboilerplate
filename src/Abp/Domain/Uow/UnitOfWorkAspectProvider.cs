using Abp.Application.Services;
using Abp.Domain.Repositories;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Domain.Uow
{
	[MulticastAttributeUsage(AllowExternalAssemblies = true)]
	[PSerializable]
	public class UnitOfWorkAspectProvider : AssemblyLevelAspect, IAspectProvider
	{
		public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
		{
			var assembly = (Assembly)targetElement;

			return assembly.GetTypes()
				.Where(t => t.IsClass && (
					typeof(IApplicationService).IsAssignableFrom(t) 
					|| typeof(IRepository).IsAssignableFrom(t) 
					|| IsUnitOfWorkType(t.GetTypeInfo()) 
					|| AnyMethodHasUnitOfWork(t.GetTypeInfo())))
				.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				.Where(m => !m.IsSpecialName && m.GetSemanticInfo().IsSelectable)
				.Select(m => new AspectInstance(m, new UnitOfWorkAspect() { AspectPriority = 1, AttributePriority = 2 }));
		}

		private static bool IsUnitOfWorkType(TypeInfo implementationType)
		{
			return UnitOfWorkHelper.HasUnitOfWorkAttribute(implementationType);
		}

		private static bool AnyMethodHasUnitOfWork(TypeInfo implementationType)
		{
			return implementationType
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Any(UnitOfWorkHelper.HasUnitOfWorkAttribute);
		}
	}
}
