using System;
using System.Security.Principal;

namespace Abp.Modules.Core.Security
{
    //TODO: Inherit from GenericIdentity and move this class out of Core!
    public class AbpIdentity : IIdentity
    {
        public int TenantId { get; set; }

        public int UserId { get; private set; }

        public string Name { get; private set; }

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public AbpIdentity()
        {
            AuthenticationType = "abp";
            IsAuthenticated = false;
        }

        public AbpIdentity(int tenantId, int userId, string name)
            : this()
        {
            TenantId = tenantId;
            UserId = userId;
            Name = name;
            IsAuthenticated = true;
        }

        public string SerializeToString()
        {
            return string.Join("|", new[] { TenantId.ToString(), UserId.ToString(), Name, AuthenticationType });
        }

        public void DeserializeFromString(string str)
        {
            var splitted = str.Split('|');
            TenantId = Convert.ToInt32(splitted[0]);
            UserId = Convert.ToInt32(splitted[1]);
            Name = splitted[2];
            AuthenticationType = splitted[3];
            IsAuthenticated = true;
        }
    }
}