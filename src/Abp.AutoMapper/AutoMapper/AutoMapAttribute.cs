using System;

namespace Abp.AutoMapper
{
    public class AutoMapAttribute : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        public AutoMapAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }
    }
}