using System;

namespace Abp.AutoMapper
{
    public class AutoMapFromAttribute : AutoMapAttributeBase
    {
        internal override AutoMapDirection Direction => AutoMapDirection.From;

        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }
    }
}