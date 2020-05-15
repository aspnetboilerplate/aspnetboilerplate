using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Castle.Core.Logging;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;

namespace Abp.Localization.Sources.Resource
{
    /// <summary>
    /// This class is used to simplify to create a localization source that
    /// uses resource a file.
    /// </summary>
    public class ResourceFileLocalizationSource : ILocalizationSource, ISingletonDependency
    {
        /// <summary>
        /// Unique Name of the source.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Reference to the <see cref="ResourceManager"/> object related to this localization source.
        /// </summary>
        public ResourceManager ResourceManager { get; }

        private ILogger _logger;
        private ILocalizationConfiguration _configuration;

        /// <param name="name">Unique Name of the source</param>
        /// <param name="resourceManager">Reference to the <see cref="ResourceManager"/> object related to this localization source</param>
        public ResourceFileLocalizationSource(string name, ResourceManager resourceManager)
        {
            Name = name;
            ResourceManager = resourceManager;
        }

        /// <summary>
        /// This method is called by ABP before first usage.
        /// </summary>
        public virtual void Initialize(ILocalizationConfiguration configuration, IIocResolver iocResolver)
        {
            _configuration = configuration;

            _logger = iocResolver.IsRegistered(typeof(ILoggerFactory))
                ? iocResolver.Resolve<ILoggerFactory>().Create(typeof(ResourceFileLocalizationSource))
                : NullLogger.Instance;
        }

        public virtual string GetString(string name)
        {
            var value = GetStringOrNull(name);
            if (value == null)
            {
                return ReturnGivenNameOrThrowException(name, CultureInfo.CurrentUICulture);
            }

            return value;
        }

        public virtual string GetString(string name, CultureInfo culture)
        {
            var value = GetStringOrNull(name, culture);
            if (value == null)
            {
                return ReturnGivenNameOrThrowException(name, culture);
            }

            return value;
        }

        public string GetStringOrNull(string name, bool tryDefaults = true)
        {
            //WARN: tryDefaults is not implemented!
            return ResourceManager.GetString(name);
        }

        public string GetStringOrNull(string name, CultureInfo culture, bool tryDefaults = true)
        {
            //WARN: tryDefaults is not implemented!
            return ResourceManager.GetString(name, culture);
        }

        public List<string> GetStrings(List<string> names)
        {
            var values = GetStringsInternal(names, CultureInfo.CurrentUICulture);
            var nullValues = values.Where(x => x.Value == null).ToList();
            if (nullValues.Any())
            {
                return ReturnGivenNamesOrThrowException(nullValues.Select(x => x.Name).ToList(), CultureInfo.CurrentUICulture);
            }

            return values.Select(x => x.Value).ToList();
        }

        public List<string> GetStrings(List<string> names, CultureInfo culture)
        {
            var values = GetStringsInternal(names, culture);
            var nullValues = values.Where(x => x.Value == null).ToList();
            if (nullValues.Any())
            {
                return ReturnGivenNamesOrThrowException(nullValues.Select(x => x.Name).ToList(), culture);
            }

            return values.Select(x => x.Value).ToList();
        }

        public List<string> GetStringsOrNull(List<string> names, bool tryDefaults = true)
        {
            return GetStringsInternal(names, CultureInfo.CurrentUICulture, tryDefaults).Select(x => x.Value).ToList();
        }

        public List<string> GetStringsOrNull(List<string> names, CultureInfo culture, bool tryDefaults = true)
        {
            return GetStringsInternal(names, culture, tryDefaults).Select(x => x.Value).ToList();
        }

        private List<NameValue> GetStringsInternal(List<string> names, CultureInfo culture, bool tryDefaults = true)
        {
            //WARN: tryDefaults is not implemented!
            return names.Select(name => new NameValue(name, ResourceManager.GetString(name, culture))).ToList();
        }

        /// <summary>
        /// Gets all strings in current language.
        /// </summary>
        public virtual IReadOnlyList<LocalizedString> GetAllStrings(bool includeDefaults = true)
        {
            return GetAllStrings(CultureInfo.CurrentUICulture, includeDefaults);
        }

        /// <summary>
        /// Gets all strings in specified culture.
        /// </summary>
        public virtual IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture, bool includeDefaults = true)
        {
            return ResourceManager
                .GetResourceSet(culture, true, includeDefaults)
                .Cast<DictionaryEntry>()
                .Select(entry => new LocalizedString(entry.Key.ToString(), entry.Value.ToString(), culture))
                .ToImmutableList();
        }

        protected virtual string ReturnGivenNameOrThrowException(string name, CultureInfo culture)
        {
            return LocalizationSourceHelper.ReturnGivenNameOrThrowException(
                _configuration,
                Name,
                name,
                culture,
                _logger
            );
        }

        protected virtual List<string> ReturnGivenNamesOrThrowException(List<string> names, CultureInfo culture)
        {
            return LocalizationSourceHelper.ReturnGivenNamesOrThrowException(
                _configuration,
                Name,
                names,
                culture,
                _logger
            );
        }
    }
}
