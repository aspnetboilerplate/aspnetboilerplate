namespace Abp.Entities.Core
{
    /// <summary>
    /// Represents a cloud account.
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
