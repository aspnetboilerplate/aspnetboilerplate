using System;

namespace Abp.AutoMapper
{
    public class AutoMapFromAttribute : AutoMapAttribute
    {
        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {
        }

        internal override AutoMapDirection Direction
        {
            get { return AutoMapDirection.From; }
        }
    }
}