using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Abp.Xml;

namespace Abp.Localization.Dictionaries.Xml
{
    /// <summary>
    /// This class is used to build a localization dictionary from XML.
    /// </summary>
    /// <remarks>
    /// Use static Build methods to create instance of this class.
    /// </remarks>
    public class XmlLocalizationDictionary : LocalizationDictionary
    {
        #region Constructor

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="cultureInfo">Culture of the dictionary</param>
        private XmlLocalizationDictionary(CultureInfo cultureInfo)
            : base(cultureInfo)
        {
            
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Builds an <see cref="XmlLocalizationDictionary"/> from given file.
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        public static XmlLocalizationDictionary BuildFomFile(string filePath)
        {
            try
            {
                return BuildFomXmlString(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                throw new AbpException("Invalid localization file format!", ex);
            }
        }

        /// <summary>
        /// Builds an <see cref="XmlLocalizationDictionary"/> from given xml string.
        /// </summary>
        /// <param name="xmlString">XML string</param>
        internal static XmlLocalizationDictionary BuildFomXmlString(string xmlString)
        {
            var settingsXmlDoc = new XmlDocument();
            settingsXmlDoc.LoadXml(xmlString);

            var localizationDictionaryNode = settingsXmlDoc.SelectNodes("/localizationDictionary");
            if (localizationDictionaryNode == null || localizationDictionaryNode.Count <= 0)
            {
                throw new AbpException("A Localization Xml must include localizationDictionary as root node.");
            }

            var dictionary = new XmlLocalizationDictionary(new CultureInfo(localizationDictionaryNode[0].GetAttributeValue("culture")));

            var textNodes = settingsXmlDoc.SelectNodes("/localizationDictionary/texts/text");
            if (textNodes != null)
            {
                foreach (XmlNode node in textNodes)
                {
                    var name = node.GetAttributeValue("name");
                    var value = node.GetAttributeValue("value");
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new AbpException("name attribute of a text is empty in given xml string.");
                    }

                    dictionary[name] = value;
                }
            }

            return dictionary;
        }

        #endregion
    }
}
