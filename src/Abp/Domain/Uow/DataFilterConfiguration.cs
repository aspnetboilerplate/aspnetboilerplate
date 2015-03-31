using System.Collections.Generic;

namespace Abp.Domain.Uow
{
    public class DataFilterConfiguration
    {
        public string FilterName { get; private set; }
        
        public bool IsEnabled { get; private set; }
        
        public IDictionary<string, object> FilterParameters { get; set; }

        public DataFilterConfiguration(string filterName, bool isEnabled)
        {
            FilterName = filterName;
            IsEnabled = isEnabled;
            FilterParameters = new Dictionary<string, object>();
        }
    }
}