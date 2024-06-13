using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class UserAccountMap : EntityMap<UserAccount, long>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UserAccountMap() : base("AbpUserAccounts")
        {
            Map(x => x.TenantId);
            Map(x => x.UserId);
            Map(x => x.UserName);
            Map(x => x.EmailAddress);
            Map(x => x.UserLinkId);

            this.MapFullAudited();
        }
    }
}
