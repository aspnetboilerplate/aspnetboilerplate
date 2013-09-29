using System;
using System.Security.Principal;

namespace Abp.Security
{
    //TODO: Inherit from GenericIdentity and move this class out of Core!
    public class AbpIdentity : IIdentity
    {
        public int UserId { get; private set; }

        public string Name { get; private set; }

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public AbpIdentity()
        {
            AuthenticationType = "abp";
            IsAuthenticated = false;
        }

        public AbpIdentity(int userId, string name)
            : this()
        {
            UserId = userId;
            Name = name;
            IsAuthenticated = true;
        }

        public string SerializeToString()
        {
            return string.Join("|", new[] { UserId.ToString(), Name, AuthenticationType });
        }

        public void DeserializeFromString(string str)
        {
            var splitted = str.Split('|');
            UserId = Convert.ToInt32(splitted[0]);
            Name = splitted[1];
            AuthenticationType = splitted[2];
            IsAuthenticated = true;
        }
    }
}