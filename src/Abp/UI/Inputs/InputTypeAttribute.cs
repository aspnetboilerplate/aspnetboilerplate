using System;

namespace Abp.UI.Inputs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InputTypeAttribute : Attribute
    {
        public InputTypeAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}