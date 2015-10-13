using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Dependency;
using Abp.Runtime.Session;

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
            Dictionary<string, string> currentTenantValues = null;

            if (AbpSession.TenantId.HasValue) //TODO: Get TenantId independent from the session.
            {
                currentTenantValues = new Dictionary<string, string>();
                foreach (var feature in allFeatures)
                {
                    currentTenantValues[feature.Name] = await _featureChecker.GetValueAsync(AbpSession.GetTenantId(), feature.Name);
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
                script.AppendLine("        '" + feature.Name.Replace("'", @"\'") + "': {");

                if (currentTenantValues != null)
                {
                    script.AppendLine("             value: '" + currentTenantValues[feature.Name].Replace(@"\", @"\\").Replace("'", @"\'") + "'");
                }
                else
                {
                    script.AppendLine("             value: " + "''");
                }

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