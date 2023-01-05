using System;
using Abp.Configuration;
using Abp.Dependency;

namespace Abp.Timing.Timezone
{
    /// <summary>
    /// Time zone converter class
    /// </summary>
    public class TimeZoneConverter : ITimeZoneConverter, ITransientDependency
    {
        private readonly ISettingManager _settingManager;
        private readonly IClock _clock;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingManager"></param>
        /// <param name="clock"></param>
        public TimeZoneConverter(
            ISettingManager settingManager, 
            IClock clock)
        {
            _settingManager = settingManager;
            _clock = clock;
        }

        /// <inheritdoc/>
        public DateTime? Convert(DateTime? date, int? tenantId, long userId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!_clock.SupportsMultipleTimezone)
            {
                return date;
            }

            var usersTimezone = _settingManager.GetSettingValueForUser(TimingSettingNames.TimeZone, tenantId, userId);
            if (string.IsNullOrEmpty(usersTimezone))
            {
                return date;
            }

            return TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), usersTimezone);
        }

        /// <inheritdoc/>
        public DateTime? Convert(DateTime? date, int tenantId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!_clock.SupportsMultipleTimezone)
            {
                return date;
            }

            var tenantsTimezone = _settingManager.GetSettingValueForTenant(TimingSettingNames.TimeZone, tenantId);
            if (string.IsNullOrEmpty(tenantsTimezone))
            {
                return date;
            }

            return TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), tenantsTimezone);
        }

        /// <inheritdoc/>
        public DateTime? Convert(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!_clock.SupportsMultipleTimezone)
            {
                return date;
            }

            var applicationsTimezone = _settingManager.GetSettingValueForApplication(TimingSettingNames.TimeZone);
            if (string.IsNullOrEmpty(applicationsTimezone))
            {
                return date;
            }

            return TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), applicationsTimezone);
        }
    }
}