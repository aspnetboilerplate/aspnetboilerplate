using System;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public class AbpIdentityBuilder : IdentityBuilder
    {
        public Type TenantType { get; }

        public AbpIdentityBuilder(IdentityBuilder identityBuilder, Type tenantType)
            : base(identityBuilder.UserType, identityBuilder.RoleType, identityBuilder.Services)
        {
            TenantType = tenantType;
        }
    }
}