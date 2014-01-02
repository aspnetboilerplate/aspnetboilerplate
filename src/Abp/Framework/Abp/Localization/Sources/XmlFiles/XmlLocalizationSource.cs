using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Exceptions;
using Abp.Localization.Engine;

namespace Abp.Localization.Sources.XmlFiles
{
    /// <summary>
    /// XML based localization source.
    /// It uses XML files to read localized strings.
    /// </summary>
    public class XmlLocalizationSource : ILocalizationSource
    {
        /// <summary>
        /// Unique Name of the source.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets directory
        /// </summary>
        public string DirectoryPath { get; private set; }

        private LocalizationEngine _localizationEngine;

        /// <summary>
        /// Creates an Xml based localization source.
        /// </summary>
        /// <param name="name">Unique Name of the source</param>
        public XmlLocalizationSource(string name) //TODO: Add overload with directory parameter
        {
            Name = name;
            DirectoryPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "Localization\\Modules\\" + name);
            _localizationEngine = new LocalizationEngine();
            Initialize();
        }

        private void Initialize()
        {
            var files = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.TopDirectoryOnly);
            var defaultLangFile = files.FirstOrDefault(f => f.EndsWith(Name + ".xml"));
            if (defaultLangFile == null)
            {
                throw new AbpException("Can not find default localization file for source " + Name + ". A source must contain a source-name.xml file as default localization.");
            }
        }

        public string GetString(string name)
        {
            return GetString(name, Thread.CurrentThread.CurrentUICulture);
        }

        public string GetString(string name, CultureInfo culture)
        {
            return _localizationEngine.GetString(name, culture);
        }

        public IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(Thread.CurrentThread.CurrentUICulture);
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture)
        {
            return _localizationEngine.GetAllStrings(culture);
        }
    }
}
