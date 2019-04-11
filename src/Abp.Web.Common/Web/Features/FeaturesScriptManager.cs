using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.Web.Http;

namespace Abp.Web.Features
{
    public class FeaturesScriptManager : IFeaturesScriptManager, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IFeatureManager _featureManager;
        private readonly IFeatureChecker _featureChecker;

        public FeaturesScriptManager(IFeatureManager featureManager, IFeatureChecker featureChecker)
        {
            _featureManager = featureManager;
            _featureChecker = featureChecker;

            AbpSession = NullAbpSession.Instance;
        }

        public async Task<string> GetScriptAsync()
        {
            var allFeatures = _featureManager.GetAll().ToList();
            var currentValues = new Dictionary<string, string>();

            if (AbpSession.TenantId.HasValue)
            {
                var currentTenantId = AbpSession.GetTenantId();
                foreach (var feature in allFeatures)
                {
                    currentValues[feature.Name] = await _featureChecker.GetValueAsync(currentTenantId, feature.Name);
                }
            }
            else
            {
                foreach (var feature in allFeatures)
                {
                    currentValues[feature.Name] = feature.DefaultValue;
                }
            }

            var script = new StringBuilder();

            script.AppendLine("(function() {");

            script.AppendLine();

            script.AppendLine("    abp.features = abp.features || {};");

            script.AppendLine();

            script.AppendLine("    abp.features.allFeatures = {");

            for (var i = 0; i < allFeatures.Count; i++)
            {
                var feature = allFeatures[i];
                script.AppendLine("        '" + HttpEncode.JavaScriptStringEncode(feature.Name) + "': {");
                script.AppendLine("             value: '" + HttpEncode.JavaScriptStringEncode(currentValues[feature.Name]) + "'");
                script.Append("        }");

                if (i < allFeatures.Count - 1)
                {
                    script.AppendLine(",");
                }
                else
                {
                    script.AppendLine();
                }
            }

            script.AppendLine("    };");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}