using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.Logging;
using Abp.Reflection.Extensions;
using Abp.Xml.Extensions;
using TimeZoneConverter;

namespace Abp.Timing.Timezone
{
    /// <summary>
    /// A helper class for timezone operations
    /// </summary>
    public static class TimezoneHelper
    {
        static readonly Dictionary<string, string> WindowsTimeZoneMappings = new Dictionary<string, string>();
        static readonly Dictionary<string, string> IanaTimeZoneMappings = new Dictionary<string, string>();
        static readonly object SyncObj = new object();

        /// <summary>
        /// Maps given windows timezone id to IANA timezone id
        /// </summary>
        /// <param name="windowsTimezoneId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string WindowsToIana(string windowsTimezoneId)
        {
            if (windowsTimezoneId.Equals("UTC", StringComparison.OrdinalIgnoreCase))
            {
                return "Etc/UTC";
            }

            GetTimezoneMappings();

            if (WindowsTimeZoneMappings.ContainsKey(windowsTimezoneId))
            {
                return WindowsTimeZoneMappings[windowsTimezoneId];
            }

            throw new Exception($"Unable to map {windowsTimezoneId} to iana timezone.");
        }

        /// <summary>
        /// Maps given IANA timezone id to windows timezone id
        /// </summary>
        /// <param name="ianaTimezoneId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string IanaToWindows(string ianaTimezoneId)
        {
            if (ianaTimezoneId.Equals("Etc/UTC", StringComparison.OrdinalIgnoreCase))
            {
                return "UTC";
            }

            GetTimezoneMappings();

            if (IanaTimeZoneMappings.ContainsKey(ianaTimezoneId))
            {
                return IanaTimeZoneMappings[ianaTimezoneId];
            }

            throw new Exception(string.Format("Unable to map {0} to windows timezone.", ianaTimezoneId));
        }

        /// <summary>
        /// Converts a date from one timezone to another
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fromTimeZoneId"></param>
        /// <param name="toTimeZoneId"></param>
        /// <returns></returns>
        public static DateTime? Convert(DateTime? date, string fromTimeZoneId, string toTimeZoneId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            var sourceTimeZone = FindTimeZoneInfo(fromTimeZoneId);
            var destinationTimeZone = FindTimeZoneInfo(toTimeZoneId);

            return TimeZoneInfo.ConvertTime(date.Value, sourceTimeZone, destinationTimeZone);
        }

        /// <summary>
        /// Converts a utc datetime to a local time based on a timezone
        /// </summary>
        /// <param name="date"></param>
        /// <param name="toTimeZoneId"></param>
        /// <returns></returns>
        public static DateTime? ConvertFromUtc(DateTime? date, string toTimeZoneId)
        {
            return Convert(date, "UTC", toTimeZoneId);
        }

        /// <summary>
        /// Converts a utc datetime in to a datetimeoffset
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTimeOffset? ConvertFromUtcToDateTimeOffset(DateTime? date, string timeZoneId)
        {
            var zonedDate = ConvertFromUtc(date, timeZoneId);

            return ConvertToDateTimeOffset(zonedDate, timeZoneId);
        }

        /// <summary>
        /// Converts a nullable date with a timezone to a nullable datetimeoffset
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTimeOffset? ConvertToDateTimeOffset(DateTime? date, string timeZoneId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return ConvertToDateTimeOffset(date.Value, timeZoneId);
        }

        /// <summary>
        /// Converts a date with a timezone to a datetimeoffset
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTimeOffset ConvertToDateTimeOffset(DateTime date, string timeZoneId)
        {
            var timeZone = FindTimeZoneInfo(timeZoneId);
            var offset = timeZone.BaseUtcOffset;
            var rule = timeZone.GetAdjustmentRules().FirstOrDefault(x => date >= x.DateStart && date <= x.DateEnd);

            if (!timeZone.SupportsDaylightSavingTime || rule == null)
            {
                return new DateTimeOffset(date, offset);
            }

            var daylightStart = GetDaylightTransition(date, rule.DaylightTransitionStart);
            var daylightEnd = GetDaylightTransition(date, rule.DaylightTransitionEnd);

            if (date >= daylightStart && date <= daylightEnd)
            {
                offset = offset.Add(rule.DaylightDelta);
            }

            return new DateTimeOffset(date, offset);
        }

        private static DateTime GetDaylightTransition(DateTime date, TimeZoneInfo.TransitionTime transitionTime)
        {
            var daylightTime = new DateTime(date.Year, transitionTime.Month, 1);

            if (transitionTime.IsFixedDateRule)
            {
                daylightTime = new DateTime(daylightTime.Year, daylightTime.Month, transitionTime.Day, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second);
            }
            else
            {
                daylightTime = daylightTime.NthOf(transitionTime.Week, transitionTime.DayOfWeek);
            }

            daylightTime = new DateTime(daylightTime.Year,
                daylightTime.Month,
                daylightTime.Day,
                transitionTime.TimeOfDay.Hour,
                transitionTime.TimeOfDay.Minute,
                transitionTime.TimeOfDay.Second);

            return daylightTime;
        }

        //from https://stackoverflow.com/questions/6140018/how-to-calculate-2nd-friday-of-month-in-c-sharp
        private static DateTime NthOf(this DateTime currentDate, int occurrence, DayOfWeek day)
        {
            var firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);

            var firstOccurrence = firstDay.DayOfWeek == day ? firstDay : firstDay.AddDays(day - firstDay.DayOfWeek);

            if (firstOccurrence.Month < currentDate.Month) occurrence = occurrence + 1;

            return firstOccurrence.AddDays(7 * (occurrence - 1));
        }

        public static DateTime? ConvertTimeByIanaTimeZoneId(DateTime? date, string fromIanaTimeZoneId, string toIanaTimeZoneId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            var sourceTimeZone = FindTimeZoneInfo(IanaToWindows(fromIanaTimeZoneId));
            var destinationTimeZone = FindTimeZoneInfo(IanaToWindows(toIanaTimeZoneId));

            return TimeZoneInfo.ConvertTime(date.Value, sourceTimeZone, destinationTimeZone);
        }

        public static DateTime? ConvertTimeFromUtcByIanaTimeZoneId(DateTime? date, string toIanaTimeZoneId)
        {
            return ConvertTimeByIanaTimeZoneId(date, "Etc/UTC", toIanaTimeZoneId);
        }

        public static DateTime? ConvertTimeToUtcByIanaTimeZoneId(DateTime? date, string fromIanaTimeZoneId)
        {
            return ConvertTimeByIanaTimeZoneId(date, fromIanaTimeZoneId, "Etc/UTC");
        }

        public static TimeZoneInfo FindTimeZoneInfo(string windowsOrIanaTimeZoneId)
        {
            return TZConvert.GetTimeZoneInfo(windowsOrIanaTimeZoneId);
        }

        public static List<string> GetWindowsTimeZoneIds(bool ignoreTimeZoneNotFoundException = true)
        {
            return TZConvert.KnownWindowsTimeZoneIds.ToList();
        }

        private static void GetTimezoneMappings()
        {
            if (WindowsTimeZoneMappings.Count > 0 && IanaTimeZoneMappings.Count > 0)
            {
                return;
            }

            lock (SyncObj)
            {
                if (WindowsTimeZoneMappings.Count > 0 && IanaTimeZoneMappings.Count > 0)
                {
                    return;
                }

                var assembly = typeof(TimezoneHelper).GetAssembly();
                var resourceNames = assembly.GetManifestResourceNames();

                var resourceName = resourceNames.First(r => r.Contains("WindowsZones.xml"));

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    var bytes = stream.GetAllBytes();
                    var xmlString = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3); //Skipping byte order mark
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xmlString);
                    var windowsMappingNodes = xmlDocument.SelectNodes("//supplementalData/windowsZones/mapTimezones/mapZone[@territory='001']");
                    var ianaMappingNodes = xmlDocument.SelectNodes("//supplementalData/windowsZones/mapTimezones/mapZone");
                    AddWindowsMappingsToDictionary(WindowsTimeZoneMappings, windowsMappingNodes);
                    AddIanaMappingsToDictionary(IanaTimeZoneMappings, ianaMappingNodes);
                }
            }
        }

        private static void AddWindowsMappingsToDictionary(Dictionary<string, string> timeZoneMappings, XmlNodeList defaultMappingNodes)
        {
            foreach (XmlNode defaultMappingNode in defaultMappingNodes)
            {
                var windowsTimezoneId = defaultMappingNode.GetAttributeValueOrNull("other");
                var ianaTimezoneId = defaultMappingNode.GetAttributeValueOrNull("type");
                if (windowsTimezoneId.IsNullOrEmpty() || ianaTimezoneId.IsNullOrEmpty())
                {
                    continue;
                }

                timeZoneMappings.Add(windowsTimezoneId, ianaTimezoneId);
            }
        }

        private static void AddIanaMappingsToDictionary(Dictionary<string, string> timeZoneMappings, XmlNodeList defaultMappingNodes)
        {
            foreach (XmlNode defaultMappingNode in defaultMappingNodes)
            {
                var ianaTimezoneId = defaultMappingNode.GetAttributeValueOrNull("type");
                var windowsTimezoneId = defaultMappingNode.GetAttributeValueOrNull("other");
                if (ianaTimezoneId.IsNullOrEmpty() || windowsTimezoneId.IsNullOrEmpty())
                {
                    continue;
                }

                ianaTimezoneId
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Where(id => !timeZoneMappings.ContainsKey(id))
                    .ToList()
                    .ForEach(ianaId => timeZoneMappings.Add(ianaId, windowsTimezoneId));
            }
        }
    }
}
