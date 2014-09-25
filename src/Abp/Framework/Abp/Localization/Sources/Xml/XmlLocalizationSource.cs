using System.IO;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Abp.Localization.Dictionaries.Xml;

namespace Abp.Localization.Sources.Xml
{
    /// <summary>
    /// XML based localization source.
    /// It uses XML files to read localized strings.
    /// </summary>
    public class XmlLocalizationSource : DictionaryBasedLocalizationSource, ISingletonDependency
    {
        internal static string RootDirectoryOfApplication { get; set; } //TODO: Find a better way of passing root directory

        /// <summary>
        /// Gets directory
        /// </summary>
        public string DirectoryPath { get; private set; }

        static XmlLocalizationSource()
        {
            RootDirectoryOfApplication = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Creates an Xml based localization source.
        /// </summary>
        /// <param name="name">Unique Name of the source</param>
        /// <param name="directoryPath">Directory path of the localization XML files</param>
        public XmlLocalizationSource(string name, string directoryPath)
            : base(name)
        {
            if (!Path.IsPathRooted(directoryPath))
            {
                directoryPath = Path.Combine(RootDirectoryOfApplication, directoryPath);
            }

            DirectoryPath = directoryPath;
        }

        public override void Initialize()
        {
            var files = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.TopDirectoryOnly);
            var defaultLangFile = files.FirstOrDefault(f => f.EndsWith(Name + ".xml"));
            if (defaultLangFile == null)
            {
                throw new AbpException("Can not find default localization file for source " + Name + ". A source must contain a source-name.xml file as default localization.");
            }

            AddDictionary(XmlLocalizationDictionary.BuildFomFile(defaultLangFile), true);
            foreach (var file in files.Where(f => f != defaultLangFile))
            {
                AddDictionary(XmlLocalizationDictionary.BuildFomFile(file));
            }
        }
    }
}
