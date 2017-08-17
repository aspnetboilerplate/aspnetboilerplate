using System;

namespace Abp.MultiTenancy
{
    /// <summary>
    /// Used to declare multi tenancy side of an object.
    /// </summary>
    public class MultiTenancySideAttribute : Attribute
    {
        /// <summary>
        /// Multitenancy side.
        /// </summary>
        public MultiTenancySides Side { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTenancySideAttribute"/> class.
        /// </summary>
        /// <param name="side">Multitenancy side.</param>
        public MultiTenancySideAttribute(MultiTenancySides side)
        {
            Side = side;
        }
    }
}