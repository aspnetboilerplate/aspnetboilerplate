using Abp.Dependency;
using Newtonsoft.Json;

namespace Abp.Auditing
{
    public class JsonNetAuditSerializer : IAuditSerializer, ITransientDependency
    {
        private readonly IAuditingConfiguration _configuration;

        public JsonNetAuditSerializer(IAuditingConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Serialize(object obj)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new AuditingContractResolver(_configuration.IgnoredTypes)
            };

            return JsonConvert.SerializeObject(obj, options);
        }
    }
}