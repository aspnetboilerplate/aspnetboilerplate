using System;
using System.Collections.Concurrent;
using System.Globalization;
using JetBrains.Annotations;

namespace Abp.Localization
{
    public static class CultureInfoHelper
    {
        private static readonly ConcurrentDictionary<string, CultureInfoCacheEntry> Cache = new ConcurrentDictionary<string, CultureInfoCacheEntry>();

        public static IDisposable Use([NotNull] string culture, string uiCulture = null)
        {
            Check.NotNull(culture, nameof(culture));

            return Use(Get(culture), uiCulture == null ? null : Get(uiCulture));
        }

        public static IDisposable Use([NotNull] CultureInfo culture, CultureInfo uiCulture = null)
        {
            Check.NotNull(culture, nameof(culture));

            var currentCulture = CultureInfo.CurrentCulture;
            var currentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture ?? culture;

            return new DisposeAction(() =>
            {
                CultureInfo.CurrentCulture = currentCulture;
                CultureInfo.CurrentUICulture = currentUiCulture;
            });
        }

        /// <summary>
        /// This method is a temporary solution since CultureInfo.GetCultureInfo() does not exists in netstandard yet.
        /// </summary>
        public static CultureInfo Get(string name)
        {
            if (name == null)
            {
                return null;
            }

            return Cache.GetOrAdd(name, n =>
            {
                return new CultureInfoCacheEntry(CultureInfo.ReadOnly(new CultureInfo(n)));
            }).CultureInfo;
        }

        private class CultureInfoCacheEntry
        {
            public CultureInfo CultureInfo { get; }

            public CultureInfoCacheEntry(CultureInfo cultureInfo)
            {
                this.CultureInfo = cultureInfo;
            }
        }
    }
}
