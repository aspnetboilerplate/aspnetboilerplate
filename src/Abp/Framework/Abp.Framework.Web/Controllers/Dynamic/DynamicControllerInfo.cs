using System;

namespace Abp.Web.Controllers.Dynamic
{
    /// <summary>
    /// Used to store controller type and name.
    /// </summary>
    internal class DynamicControllerInfo
    {
        /// <summary>
        /// Name of the controller.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Controller type.
        /// </summary>
        public Type Type { get; set; }
    }
}