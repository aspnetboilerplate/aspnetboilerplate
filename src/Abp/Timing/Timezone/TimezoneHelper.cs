using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.Reflection.Extensions;
using Abp.Xml.Extensions;

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

        public static DateTime? Convert(DateTime? date, string fromTimeZoneId, string toTimeZoneId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(fromTimeZoneId);
            var destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(toTimeZoneId);
            return TimeZoneInfo.ConvertTime(date.Value, sourceTimeZone, destinationTimeZone);
        }

        public static DateTime? ConvertFromUtc(DateTime? date, string toTimeZoneId)
        {
            return Convert(date, "UTC", toTimeZoneId);
        }

        public static DateTime? ConvertTimeByIanaTimeZoneId(DateTime? date, string fromIanaTimeZoneId, string toIanaTimeZoneId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(IanaToWindows(fromIanaTimeZoneId));
            var destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(IanaToWindows(toIanaTimeZoneId));

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
