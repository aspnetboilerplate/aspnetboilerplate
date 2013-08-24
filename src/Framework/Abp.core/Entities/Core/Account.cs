namespace Abp.Entities.Core
{
    /// <summary>
    /// Represents an account. An account is used in the cloud to identify a seperated application in the system.
    /// </summary>
    public class Account : AuditedEntity<int>
    {
        /// <summary>
        /// Company name
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        /// Owner of this account.
        /// </summary>
        public virtual User Owner { get; set; }
    }
}
