using System;

namespace Abp.AutoMapper
{
    public class AutoMapToAttribute : AutoMapAttribute
    {
        public AutoMapToAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {
        }

        internal override AutoMapDirection Direction
        {
            get { return AutoMapDirection.To; }
        }
    }
}