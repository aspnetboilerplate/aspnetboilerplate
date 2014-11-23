using System;

namespace Abp.AutoMapper
{
    public class AutoMapToAttribute : AutoMapAttribute
    {
        internal override AutoMapDirection Direction
        {
            get { return AutoMapDirection.To; }
        }

        public AutoMapToAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }
    }
}