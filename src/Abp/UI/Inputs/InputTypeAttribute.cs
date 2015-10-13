using System;

namespace Abp.UI.Inputs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InputTypeAttribute : Attribute
    {
        public string TypeName { get; set; }

        public InputTypeAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}