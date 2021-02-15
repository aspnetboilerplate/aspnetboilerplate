namespace Abp.Domain.Uow
{
    public class UnitOfWorkAuditingConfiguration
    {
        public bool DisableCreatorUserId { get; set; }
        
        public bool DisableLastModifierUserId { get; set; }
        
        public bool DisableDeleterUserId { get; set; }
    }
    
}
