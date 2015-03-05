using Abp.Constants;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.Constants
{
    /// <summary>   Generates JavaScript for constants and enums 
    ///             marked with the PublishAttribute. </summary>
    public class ConstantScriptManager : IConstantScriptManager, ISingletonDependency
    {
        /// <summary>   Generates a JavaScript object representing the Enum.  Each Enum value is
        ///             exported as well as a values array that can be used in drop down lists.  
        ///             The values array includes an automatically named locallized string that
        ///             can be used for localization.  The localized name is in the format
        ///             Enum_[Parent_][EnumName]_[ValueName].</summary>
        ///
        /// <exception cref="ArgumentException"> Thrown if the type is not an Enum</exception>
        ///
        /// <param name="enumType">        The Enum Type to process. </param>
        /// <param name="parent">   An optional parent class name with which 
        ///                         to group the enum values. </param>
        ///
        /// <returns>   The enum script. </returns>
        private string GetEnumScript(Type enumType, string parent = null)
        {
            string resourcePrefix = "Enum_";
            var script = new StringBuilder();
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("enumType is not an Enum.", "enumType");
            }
            script.Append("    abp.constant.");
            /**
             * If a parent class is defined, add the extra level.
             * The assumption is that the parent class would have already
             * been declared first in the script.
             */
            if (!string.IsNullOrEmpty(parent))
            {
                parent = parent.Trim('.');
                resourcePrefix += parent + "_";
                script.Append(parent.ToCamelCase());
                script.Append(".");
            }
            resourcePrefix += enumType.Name + "_";
            script.Append(enumType.Name.ToCamelCase());
            script.Append(" = {");

            // Export all enum values
            int added = 0;
            var valuesArray = new StringBuilder();
            valuesArray.Append("        values: [");
            foreach (var enumValue in enumType.GetEnumValues())
            {
                if (added > 0)
                {
                    script.AppendLine(",");
                    valuesArray.AppendLine(",");
                }
                else
                {
                    script.AppendLine();
                    valuesArray.AppendLine();
                }
                string name = enumType.GetEnumName(enumValue);
                long value = Convert.ToInt64(enumValue);
                script.Append("        " + name.ToCamelCase() + ": " + value + "");
                valuesArray.Append("            { name: '" + name + "', value: " + value + ", localizedName: '" + resourcePrefix + name + "' }");
                ++added;
            }
            valuesArray.Append("]");
            if (added > 0)
            {
                script.AppendLine(",");
            }
            else
            {
                script.AppendLine();
            }
            script.AppendLine(valuesArray.ToString());
            script.AppendLine("    };");
            return script.ToString();
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    abp.constant = abp.constant || {};");

            /**
             * Use reflection to find all Enums & Classes marked as publish
             */
            TypeFinder finder = new TypeFinder();
            Type[] potentialPublishTypes = finder.Find(q => (q.IsClass || q.IsEnum));

            foreach (Type potentialPublishType in potentialPublishTypes)
            {
                // Check for publish attribute
                PublishAttribute publishAttr = TypeDescriptor.GetAttributes(potentialPublishType)
                    .OfType<PublishAttribute>()
                    .FirstOrDefault();
                if (publishAttr != null &&
                    publishAttr.Export)
                {
                    if (potentialPublishType.IsEnum)
                    {
                        script.Append(GetEnumScript(potentialPublishType));
                    }
                    else
                    {
                        script.Append("    abp.constant." + potentialPublishType.Name.ToCamelCase() + " = {");
                        // Export all public constants & readonly values
                        var added = 0;
                        foreach (var potentialConstantField in potentialPublishType.GetFields(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static))
                        {
                            if (potentialConstantField.IsLiteral)
                            {
                                object constantValue = potentialConstantField.GetRawConstantValue();
                                if (constantValue != null)
                                {
                                    if (added > 0)
                                    {
                                        script.AppendLine(",");
                                    }
                                    else
                                    {
                                        script.AppendLine();
                                    }
                                    string value = constantValue.ToString();
                                    if (potentialConstantField.FieldType == typeof(string))
                                    {
                                        // If this is a string type, enclose in quotes.
                                        value = string.Format("'{0}'", constantValue);
                                    }
                                    script.Append("        " + potentialConstantField.Name.ToCamelCase() + ": " + value);
                                    ++added;
                                }
                            }
                        }
                        script.AppendLine();
                        script.AppendLine("    };");
                    }

                    // Export any nested enums
                    foreach (var nestedType in potentialPublishType.GetNestedTypes())
                    {
                        if (nestedType.IsEnum)
                        {
                            PublishAttribute nestedPublishAttr = TypeDescriptor.GetAttributes(nestedType)
                                .OfType<PublishAttribute>()
                                .FirstOrDefault();
                            if (nestedPublishAttr == null ||
                                nestedPublishAttr.Export == true)
                            {
                                script.Append(GetEnumScript(nestedType, potentialPublishType.Name));
                            }
                        }
                    }
                }
            }

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }

}
