using System.Linq;
using System.Reflection;
using Abp.Domain.Entities;
using Abp.Extensions;

namespace Abp.MultiTenancy
{
    internal class MultiTenancyHelper
    {
        public static bool IsMultiTenantEntity(object entity)
        {
            return entity is IMayHaveTenant || entity is IMustHaveTenant;
        }

        /// <param name="entity">The entity to check</param>
        /// <param name="expectedTenantId">TenantId or null for host</param>
        public static bool IsTenantEntity(object entity, int? expectedTenantId)
        {
            return (entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == expectedTenantId) ||
                   (entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == expectedTenantId);
        }

        public static bool IsHostEntity(object entity)
        {
            MultiTenancySideAttribute attribute = entity.GetType().GetTypeInfo()
                .GetCustomAttributes(typeof(MultiTenancySideAttribute), true)
                .Cast<MultiTenancySideAttribute>()
                .FirstOrDefault();

            if (attribute == null)
            {
                return false;
            }

            return attribute.Side.HasFlag(MultiTenancySides.Host);
        }
    }
}
