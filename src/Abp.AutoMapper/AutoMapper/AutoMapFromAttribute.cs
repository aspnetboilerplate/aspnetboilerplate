using System;

namespace Abp.AutoMapper
{
    public class AutoMapFromAttribute : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        public AutoMapFromAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }
    }
}