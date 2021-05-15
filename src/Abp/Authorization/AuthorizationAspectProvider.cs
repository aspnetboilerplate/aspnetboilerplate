using Abp.Application.Features;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Authorization
{
	[MulticastAttributeUsage(AllowExternalAssemblies = true)]
	[PSerializable]
	public class AuthorizationAspectProvider : AssemblyLevelAspect, IAspectProvider
	{
		public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
		{
			var assembly = (Assembly)targetElement;

			return assembly.GetTypes()
				.Where(t => t.IsClass && ShouldIntercept(t))
				.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				.Where(m => !m.IsSpecialName && m.GetSemanticInfo().IsSelectable)
				.Select(m => new AspectInstance(m, new AuthorizationAspect() { AspectPriority = 2, AttributePriority = 2 }));
		}

		private static bool ShouldIntercept(Type type)
		{
			if (SelfOrMethodsDefinesAttribute<AbpAuthorizeAttribute>(type))
			{
				return true;
			}

			if (SelfOrMethodsDefinesAttribute<RequiresFeatureAttribute>(type))
			{
				return true;
			}

			return false;
		}

		private static bool SelfOrMethodsDefinesAttribute<TAttr>(Type type)
		{
			if (type.GetTypeInfo().IsDefined(typeof(TAttr), true))
			{
				return true;
			}

			return type
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Any(m => m.IsDefined(typeof(TAttr), true));
		}
	}
}
