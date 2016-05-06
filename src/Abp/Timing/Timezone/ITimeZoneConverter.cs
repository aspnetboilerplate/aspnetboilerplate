using System;

namespace Abp.Timing.Timezone
{
    /// <summary>
    /// Interface for timezone converter
    /// </summary>
    public interface ITimeZoneConverter
    {
        /// <summary>
        /// Converts given date to user's time zone. 
        /// If timezone setting is not specified, returns given date.
        /// </summary>
        /// <param name="date">Base date to convert</param>
        /// <param name="tenantId">TenantId of user</param>
        /// <param name="userId">UserId to convert date for</param>
        /// <returns></returns>
        DateTime? Convert(DateTime? date, int? tenantId, long userId);

        /// <summary>
        /// Converts given date to tenant's time zone. 
        /// If timezone setting is not specified, returns given date.
        /// </summary>
        /// <param name="date">Base date to convert</param>
        /// <param name="tenantId">TenantId  to convert date for</param>
        /// <returns></returns>
        DateTime? Convert(DateTime? date, int tenantId);

        /// <summary>
        /// Converts given date to application's time zone. 
        /// If timezone setting is not specified, returns given date.
        /// </summary>
        /// <param name="date">Base date to convert</param>
        /// <returns></returns>
        DateTime? Convert(DateTime? date);
    }
}
