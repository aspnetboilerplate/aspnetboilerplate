using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Xml.Extensions;

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
        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="cultureInfo">Culture of the dictionary</param>
        private XmlLocalizationDictionary(CultureInfo cultureInfo)
            : base(cultureInfo)
        {

        }

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
                throw new AbpException("Invalid localization file format! " + filePath, ex);
            }
        }

        /// <summary>
        /// Builds an <see cref="XmlLocalizationDictionary"/> from given xml string.
        /// </summary>
        /// <param name="xmlString">XML string</param>
        public static XmlLocalizationDictionary BuildFomXmlString(string xmlString)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);

            var localizationDictionaryNode = xmlDocument.SelectNodes("/localizationDictionary");
            if (localizationDictionaryNode == null || localizationDictionaryNode.Count <= 0)
            {
                throw new AbpException("A Localization Xml must include localizationDictionary as root node.");
            }

            var cultureName = localizationDictionaryNode[0].GetAttributeValueOrNull("culture");
            if (string.IsNullOrEmpty(cultureName))
            {
                throw new AbpException("culture is not defined in language XML file!");
            }

            var dictionary = new XmlLocalizationDictionary(CultureInfo.GetCultureInfo(cultureName));

            var dublicateNames = new List<string>();

            var textNodes = xmlDocument.SelectNodes("/localizationDictionary/texts/text");
            if (textNodes != null)
            {
                foreach (XmlNode node in textNodes)
                {
                    var name = node.GetAttributeValueOrNull("name");
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new AbpException("name attribute of a text is empty in given xml string.");
                    }

                    if (dictionary.Contains(name))
                    {
                        dublicateNames.Add(name);
                    }

                    dictionary[name] = (node.GetAttributeValueOrNull("value") ?? node.InnerText).NormalizeLineEndings();
                }
            }

            if (dublicateNames.Count > 0)
            {
                throw new AbpException("A dictionary can not contain same key twice. There are some duplicated names: " + dublicateNames.JoinAsString(", "));
            }

            return dictionary;
        }
    }
}
