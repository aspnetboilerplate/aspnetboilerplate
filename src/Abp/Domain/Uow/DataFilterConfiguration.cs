using System.Collections.Generic;

namespace Abp.Domain.Uow
{
    public class DataFilterConfiguration
    {
        public string FilterName { get; private set; }

        public bool IsEnabled { get; private set; }

        public IDictionary<string, object> FilterParameters { get; private set; }

        public DataFilterConfiguration(string filterName, bool isEnabled)
        {
            FilterName = filterName;
            IsEnabled = isEnabled;
            FilterParameters = new Dictionary<string, object>();
        }

        internal DataFilterConfiguration(DataFilterConfiguration filterToClone)
            : this(filterToClone.FilterName, filterToClone.IsEnabled)
        {
            foreach (var filterParameter in filterToClone.FilterParameters)
            {
                FilterParameters[filterParameter.Key] = filterParameter.Value;
            }
        }
    }
}