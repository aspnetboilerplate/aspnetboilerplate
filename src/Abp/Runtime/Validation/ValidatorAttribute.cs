using System;

namespace Abp.Runtime.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidatorAttribute : Attribute
    {
        public string TypeName { get; set; }

        public ValidatorAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}