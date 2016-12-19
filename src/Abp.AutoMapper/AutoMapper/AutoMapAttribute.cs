using System;

namespace Abp.AutoMapper
{
    public class AutoMapAttribute : AutoMapAttributeBase
    {
        internal override AutoMapDirection Direction => AutoMapDirection.From | AutoMapDirection.To;

        public AutoMapAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }
    }
}