namespace Abp.Domain.Uow
{
    public class DataFilterConfiguration
    {
        public string FilterName { get; private set; }
        
        public bool IsEnabled { get; private set; }

        public DataFilterConfiguration(string filterName, bool isEnabled)
        {
            FilterName = filterName;
            IsEnabled = isEnabled;
        }
    }
}