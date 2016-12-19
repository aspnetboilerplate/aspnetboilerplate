using System;

namespace Abp.AutoMapper
{
    public class AutoMapToAttribute : AutoMapAttributeBase
    {
        internal override AutoMapDirection Direction => AutoMapDirection.To;

        public AutoMapToAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }
    }
}