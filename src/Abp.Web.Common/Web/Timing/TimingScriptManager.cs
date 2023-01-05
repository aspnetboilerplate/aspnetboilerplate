using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Timing;
using Abp.Timing.Timezone;

namespace Abp.Web.Timing
{
    /// <summary>
    /// This class is used to build timing script.
    /// </summary>
    public class TimingScriptManager : ITimingScriptManager, ITransientDependency
    {
        private readonly ISettingManager _settingManager;
        private readonly IClock _clock;
        
        public TimingScriptManager(ISettingManager settingManager, IClock clock)
        {
            _settingManager = settingManager;
            _clock = clock;
        }

        public async Task<string> GetScriptAsync()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");

            script.AppendLine("    abp.clock.provider = abp.timing." + _clock.Provider.GetType().Name.ToCamelCase() + " || abp.timing.localClockProvider;");
            script.AppendLine("    abp.clock.provider.supportsMultipleTimezone = " + _clock.SupportsMultipleTimezone.ToString().ToLowerInvariant() + ";");

            if (_clock.SupportsMultipleTimezone)
            {
                script.AppendLine("    abp.timing.timeZoneInfo = " + await GetUsersTimezoneScriptsAsync());
            }

            script.Append("})();");

            return script.ToString();
        }

        private async Task<string> GetUsersTimezoneScriptsAsync()
        {
            var timezoneId = await _settingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);
            var timezone = TimezoneHelper.FindTimeZoneInfo(timezoneId);

            return " {" +
                   "        windows: {" +
                   "            timeZoneId: '" + timezoneId + "'," +
                   "            baseUtcOffsetInMilliseconds: '" + timezone.BaseUtcOffset.TotalMilliseconds + "'," +
                   "            currentUtcOffsetInMilliseconds: '" + timezone.GetUtcOffset(_clock.Now).TotalMilliseconds + "'," +
                   "            isDaylightSavingTimeNow: '" + timezone.IsDaylightSavingTime(_clock.Now) + "'" +
                   "        }," +
                   "        iana: {" +
                   "            timeZoneId:'" + TimezoneHelper.WindowsToIana(timezoneId) + "'" +
                   "        }," +
                   "    }";
        }
    }
}