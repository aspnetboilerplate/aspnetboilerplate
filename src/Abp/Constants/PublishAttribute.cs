using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Constants
{
    /// <summary>   Attribute for enums and classes to indicate whether or not
    ///             to publish values and constants publically. </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class)]
    public class PublishAttribute : Attribute
    {
        /// <summary>   Define whether or not to publish an enum or a 
        ///             class's public constants </summary>
        ///
        /// <param name="export">   true to export. </param>
        public PublishAttribute(bool export = true)
        {
            Export = export;
        }

        /// <summary>   Gets or sets a value indicating whether the export. </summary>
        ///
        /// <value> true if export, false if not. </value>
        public bool Export { get; private set; }
    }
}
