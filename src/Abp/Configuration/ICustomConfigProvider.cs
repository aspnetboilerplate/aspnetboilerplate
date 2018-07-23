using System.Collections.Generic;

namespace Abp.Configuration.Startup
{
    public interface ICustomConfigProvider
    {
        Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext);
    }
}
