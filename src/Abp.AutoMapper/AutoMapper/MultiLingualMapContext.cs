using Abp.Configuration;

namespace Abp.AutoMapper
{
    public class MultiLingualMapContext
    {
        public ISettingManager SettingManager { get; set; }

        public MultiLingualMapContext(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }
    }
}