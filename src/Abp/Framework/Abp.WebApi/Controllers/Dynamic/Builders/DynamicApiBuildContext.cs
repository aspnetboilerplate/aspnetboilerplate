using System.Collections.Generic;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// This class is used to store some informations while a dynamic api controller is being built.
    /// </summary>
    internal class DynamicApiBuildContext
    {
        /// <summary>
        /// List of methods those are customized by user.
        /// </summary>
        public IDictionary<string, DynamicApiActionInfo> CustomizedMethods { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="DynamicApiBuildContext"/>.
        /// </summary>
        public DynamicApiBuildContext()
        {
            CustomizedMethods = new Dictionary<string, DynamicApiActionInfo>();
        }
    }
}