using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Dependency;

namespace Abp.WebHooks
{
    internal class WebHookDefinitionManager : IWebHookDefinitionManager, ISingletonDependency
    {
        private readonly IWebHooksConfiguration _webHooksConfiguration;
        private readonly IocManager _iocManager;
        private readonly ConcurrentDictionary<string, WebHookDefinition> _webHookDefinitions;

        public WebHookDefinitionManager(
            IWebHooksConfiguration webHooksConfiguration,
            IocManager iocManager
            )
        {
            _webHooksConfiguration = webHooksConfiguration;
            _iocManager = iocManager;
            _webHookDefinitions = new ConcurrentDictionary<string, WebHookDefinition>();
        }

        public void Initialize()
        {
            var context = new WebHookDefinitionContext(this);

            foreach (var providerType in _webHooksConfiguration.Providers)
            {
                using (var provider = _iocManager.ResolveAsDisposable<WebHookDefinitionProvider>(providerType))
                {
                    provider.Object.SetWebHooks(context);
                }
            }
        }

        public void Add(WebHookDefinition webHookDefinition)
        {
            if (_webHookDefinitions.ContainsKey(webHookDefinition.Name))
            {
                throw new AbpInitializationException("There is already a webhook definition with given name: " + webHookDefinition.Name + ". Webhook names must be unique!");
            }

            _webHookDefinitions.AddOrUpdate(webHookDefinition.Name, webHookDefinition, (s, d) => d);
        }

        public void AddOrReplace(WebHookDefinition webHookDefinition)
        {
            _webHookDefinitions.AddOrUpdate(webHookDefinition.Name, webHookDefinition, (s, d) => webHookDefinition);
        }

        public WebHookDefinition GetOrNull(string name)
        {
            if (!_webHookDefinitions.ContainsKey(name))
            {
                return null;
            }

            return _webHookDefinitions[name];
        }

        public WebHookDefinition Get(string name)
        {
            return _webHookDefinitions[name];
        }

        public IReadOnlyList<WebHookDefinition> GetAll()
        {
            return _webHookDefinitions.Values.ToImmutableList();
        }

        public bool TryRemove(string name, out WebHookDefinition webHookDefinition)
        {
            return _webHookDefinitions.TryRemove(name, out webHookDefinition);
        }

        public bool Contains(string name)
        {
            return _webHookDefinitions.ContainsKey(name);
        }

        public async Task<bool> IsAvailableAsync(int? tenantId, string name)
        {
            if (tenantId == null)//host allowed to subscribe all webhooks
            {
                return true;
            }

            var webHookDefinition = GetOrNull(name);
            if (webHookDefinition == null)
            {
                return true;
            }

            if (webHookDefinition.FeatureDependency != null)
            {
                using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
                {
                    featureDependencyContext.Object.TenantId = tenantId;

                    if (!await webHookDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext.Object))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsAvailable(int? tenantId, string name)
        {
            if (tenantId == null)//host allowed to subscribe all webhooks
            {
                return true;
            }

            var webHookDefinition = GetOrNull(name);
            if (webHookDefinition == null)
            {
                return true;
            }

            if (webHookDefinition.FeatureDependency != null)
            {
                using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
                {
                    featureDependencyContext.Object.TenantId = tenantId;

                    if (!webHookDefinition.FeatureDependency.IsSatisfied(featureDependencyContext.Object))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
