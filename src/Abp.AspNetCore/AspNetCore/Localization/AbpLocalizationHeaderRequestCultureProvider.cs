using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Castle.Core.Logging;

namespace Abp.AspNetCore.Localization
{
    public class AbpLocalizationHeaderRequestCultureProvider : RequestCultureProvider
    {
        public ILogger Logger { get; set; }

        private static readonly char[] Separator = { '|' };

        private static readonly string _culturePrefix = "c=";
        private static readonly string _uiCulturePrefix = "uic=";

        public AbpLocalizationHeaderRequestCultureProvider()
        {
            Logger = NullLogger.Instance;
        }

        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var localizationHeader = httpContext.Request.Headers[CookieRequestCultureProvider.DefaultCookieName];

            if (localizationHeader.Count == 0)
            {
                return Task.FromResult((ProviderCultureResult) null);
            }

            var cultureResult = ParseHeaderValue(localizationHeader);
            var culture = cultureResult.Cultures.First().Value;
            var uiCulture = cultureResult.UICultures.First().Value;
            Logger.DebugFormat("{0} - Using Culture:{1} , UICulture:{2}", nameof(AbpLocalizationHeaderRequestCultureProvider), culture, uiCulture);

            return Task.FromResult(cultureResult);
        }

        /// <summary>
        /// Parses a <see cref="RequestCulture"/> from the specified cookie value.
        /// Returns <c>null</c> if parsing fails.
        /// </summary>
        /// <param name="value">The cookie value to parse.</param>
        /// <returns>The <see cref="RequestCulture"/> or <c>null</c> if parsing fails.</returns>
        public static ProviderCultureResult ParseHeaderValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (!value.Contains("|") && !value.Contains("="))
            {
                return new ProviderCultureResult(value, value);
            }

            var parts = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return null;
            }

            var potentialCultureName = parts[0];
            var potentialUiCultureName = parts[1];

            if (!potentialCultureName.StartsWith(_culturePrefix) || !potentialUiCultureName.StartsWith(_uiCulturePrefix))
            {
                return null;
            }

            var cultureName = potentialCultureName.Substring(_culturePrefix.Length);
            var uiCultureName = potentialUiCultureName.Substring(_uiCulturePrefix.Length);

            return new ProviderCultureResult(cultureName, uiCultureName);
        }
    }
}
