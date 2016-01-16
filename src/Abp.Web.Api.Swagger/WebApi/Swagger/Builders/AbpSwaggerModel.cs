using System.Collections.Generic;

namespace Abp.WebApi.Swagger.Builders
{
    internal class AbpSwaggerModel
    {
        public List<string> Modules { get; set; }

        public Dictionary<string, List<string>> Services { get; set; }

        public AbpSwaggerModel()
        {
            Modules = new List<string>();
            Services = new Dictionary<string, List<string>>();
        }
    }
}
