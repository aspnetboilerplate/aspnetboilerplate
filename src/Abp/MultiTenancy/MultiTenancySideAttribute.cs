using System;

namespace Abp.MultiTenancy
{
    public class MultiTenancySideAttribute : Attribute
    {
        public MultiTenancySides Side { get; set; }

        public MultiTenancySideAttribute(MultiTenancySides side)
        {
            Side = side;
        }
    }
}