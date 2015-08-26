using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Logging;

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
        public string Name { get; private set; }

        /// <summary>
        /// Reference to the <see cref="ResourceManager"/> object related to this localization source.
        /// </summary>
        public ResourceManager ResourceManager { get; private set; }

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
        }

        /// <summary>
        /// Gets localized string for given name in current language.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Localized string</returns>
        public virtual string GetString(string name)
        {
            var value = ResourceManager.GetString(name);
            if (value == null)
            {
                return ReturnGivenNameOrThrowException(name);
            }

            return value;
        }

        /// <summary>
        /// Gets localized string for given name and specified culture.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        public virtual string GetString(string name, CultureInfo culture)
        {
            var value = ResourceManager.GetString(name, culture);
            if (value == null)
            {
                return ReturnGivenNameOrThrowException(name);
            }

            return value;
        }

        /// <summary>
        /// Gets all strings in current language.
        /// </summary>
        public virtual IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        /// Gets all strings in specified culture.
        /// </summary>
        public virtual IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture)
        {
            return ResourceManager
                .GetResourceSet(culture, true, true) //TODO: true or false for createIfNotExists? Test it's effect.
                .Cast<DictionaryEntry>()
                .Select(entry => new LocalizedString(entry.Key.ToString(), entry.Value.ToString(), culture))
                .ToImmutableList();
        }

        private string ReturnGivenNameOrThrowException(string name)
        {
            var exceptionMessage = string.Format(
                "Can not find '{0}' in localization source '{1}'!",
                name, Name
                );

            if (_configuration.ReturnGivenTextIfNotFound)
            {
                LogHelper.Logger.Warn(exceptionMessage);
                return _configuration.WrapGivenTextIfNotFound
                    ? string.Format("[{0}]", name)
                    : name;
            }

            throw new AbpException(exceptionMessage);
        }
    }
}
