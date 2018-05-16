using System.Collections.Generic;

namespace Abp.Web.Configuration
{
    public abstract class CustomDataConfigProvider
    {
        public abstract Dictionary<string, object> GetConfig();
    }
}
