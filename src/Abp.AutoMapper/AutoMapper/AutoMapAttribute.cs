using System;

namespace Abp.AutoMapper
{
    public class AutoMapAttribute : Attribute
    {
        public AutoMapAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }

        public Type[] TargetTypes { get; private set; }

        internal virtual AutoMapDirection Direction
        {
            get { return AutoMapDirection.From | AutoMapDirection.To; }
        }
    }
}