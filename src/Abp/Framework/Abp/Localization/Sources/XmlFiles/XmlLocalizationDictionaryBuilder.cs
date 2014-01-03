using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Abp.Exceptions;
using Abp.Localization.Engine;
using Abp.Utils.Extensions.Xml;

namespace Abp.Localization.Sources.XmlFiles
{
    internal static class XmlLocalizationDictionaryBuilder
    {
        public static LocalizationDictionary BuildFomFile(string filePath)
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

        public static LocalizationDictionary BuildFomXmlString(string xmlString)
        {
            var settingsXmlDoc = new XmlDocument();
            settingsXmlDoc.LoadXml(xmlString);

            var localizationDictionaryNode = settingsXmlDoc.SelectNodes("/localizationDictionary");
            if (localizationDictionaryNode == null || localizationDictionaryNode.Count <= 0)
            {
                throw new AbpException("A Localization Xml must include localizationDictionary as root node.");
            }

            var dictionary = new LocalizationDictionary(new CultureInfo(localizationDictionaryNode[0].GetAttributeValue("culture")));

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
    }
}
