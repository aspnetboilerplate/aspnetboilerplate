using System;
using System.Collections.Generic;
using Abp.Configuration;
using Abp.Configuration.Startup;

namespace Abp.ZeroCore.SampleApp.Application
{
    public class TestCustomConfigProvider : ICustomConfigProvider
    {
        public Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext)
        {
            var config = new Dictionary<string, object>
            {
                {
                    "test_config_int", 1
                },
                {
                    "test_config_date", DateTime.Now
                }
            };

            return config;
        }
    }

    public class TestCustomConfigProvider2 : ICustomConfigProvider
    {
        public Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext)
        {
            var config = new Dictionary<string, object>
            {
                {
                    "test_config_int", 2
                }
            };

            return config;
        }
    }
}
