using System;

namespace Abp.AutoMapper
{
    public class AutoMapToAttribute : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        public AutoMapToAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }
    }
}