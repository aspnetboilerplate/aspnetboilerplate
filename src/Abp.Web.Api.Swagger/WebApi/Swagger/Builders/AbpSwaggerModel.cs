using System.Collections.Generic;

namespace Abp.WebApi.Swagger.Builders
{
    /// <summary>
    /// Module and Application Service information
    /// </summary>
    internal class AbpSwaggerModel
    {
        /// <summary>
        /// Module names
        /// </summary>
        public List<string> Modules { get; set; }

        /// <summary>
        /// The key is module name, These values are your application service.
        /// </summary>
        public Dictionary<string, List<string>> Services { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AbpSwaggerModel()
        {
            Modules = new List<string>();
            Services = new Dictionary<string, List<string>>();
        }
    }
}
