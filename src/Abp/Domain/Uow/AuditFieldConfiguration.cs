namespace Abp.Domain.Uow
{
    public class AuditFieldConfiguration
    {
        public string FieldName { get; set; }
        
        public bool IsEnabled { get; }
        
        public AuditFieldConfiguration(string fieldName, bool isEnabled)
        {
            FieldName = fieldName;
            IsEnabled = isEnabled;
        }
    }
}