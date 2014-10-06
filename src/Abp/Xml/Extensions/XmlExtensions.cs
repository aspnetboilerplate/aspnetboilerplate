using System;
using System.Xml;

namespace Abp.Xml.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="XmlNode"/> class.
    /// </summary>
    public static class XmlNodeExtensions
    {
        /// <summary>
        /// Gets an attribute's value from an Xml node.
        /// </summary>
        /// <param name="node">The Xml node</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Value of the attribute</returns>
        public static string GetAttributeValue(this XmlNode node, string attributeName)
        {
            if (node.Attributes == null || node.Attributes.Count <= 0)
            {
                throw new ApplicationException(node.Name + " node has not " + attributeName + " attribute");
            }

            return node.Attributes[attributeName].Value;
        }
    }
}
