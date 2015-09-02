namespace Abp.Auditing
{
    /// <summary>
    /// Null implementation of <see cref="IAuditInfoProvider"/>.
    /// </summary>
    internal class NullAuditInfoProvider : IAuditInfoProvider
    {
        public void Fill(AuditInfo auditInfo)
        {
            
        }
    }
}