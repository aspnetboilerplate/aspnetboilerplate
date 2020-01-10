using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Services;
using Abp.Domain.Uow;

namespace Abp.WebHooks
{
    public class WebHookSubscriptionManager : DomainService, IWebHookSubscriptionManager
    {
        public IWebHookSubscriptionsStore WebHookSubscriptionsStore { get; set; }

        private readonly IGuidGenerator _guidGenerator;

        private readonly IWebHookDefinitionManager _webHookDefinitionManager;

        public WebHookSubscriptionManager(
            IGuidGenerator guidGenerator,
            IWebHookDefinitionManager webHookDefinitionManager)
        {
            _guidGenerator = guidGenerator;
            _webHookDefinitionManager = webHookDefinitionManager;

            WebHookSubscriptionsStore = NullWebHookSubscriptionsStore.Instance;
        }

        public async Task<WebHookSubscription> GetAsync(Guid id)
        {
            return (await WebHookSubscriptionsStore.GetAsync(id)).ToWebHookSubscription();
        }

        public WebHookSubscription Get(Guid id)
        {
            return WebHookSubscriptionsStore.Get(id).ToWebHookSubscription();
        }

        public async Task<List<WebHookSubscription>> GetAllSubscriptionsAsync(int? tenantId)
        {
            return (await WebHookSubscriptionsStore.GetAllSubscriptionsAsync(tenantId))
                .Select(x => x.ToWebHookSubscription()).ToList();
        }

        public List<WebHookSubscription> GetAllSubscriptions(int? tenantId)
        {
            return WebHookSubscriptionsStore.GetAllSubscriptions(tenantId)
            .Select(x => x.ToWebHookSubscription()).ToList();
        }

        public async Task<List<WebHookSubscription>> GetAllSubscriptionsIfFeaturesGrantedAsync(int? tenantId, string webHookName)
        {
            if (!await _webHookDefinitionManager.IsAvailableAsync(tenantId, webHookName))
            {
                return new List<WebHookSubscription>();
            }

            return (await WebHookSubscriptionsStore.GetAllSubscriptionsAsync(tenantId, webHookName))
                .Select(x => x.ToWebHookSubscription()).ToList();
        }

        public List<WebHookSubscription> GetAllSubscriptionsIfFeaturesGranted(int? tenantId, string webHookName)
        {
            if (!_webHookDefinitionManager.IsAvailable(tenantId, webHookName))
            {
                return new List<WebHookSubscription>();
            }

            return WebHookSubscriptionsStore.GetAllSubscriptions(tenantId, webHookName)
                .Select(x => x.ToWebHookSubscription()).ToList();
        }

        public async Task<bool> IsSubscribedAsync(int? tenantId, string webHookName)
        {
            if (!await _webHookDefinitionManager.IsAvailableAsync(tenantId, webHookName))
            {
                return false;
            }

            return await WebHookSubscriptionsStore.IsSubscribedAsync(tenantId, webHookName);
        }

        public bool IsSubscribed(int? tenantId, string webHookName)
        {
            if (!_webHookDefinitionManager.IsAvailable(tenantId, webHookName))
            {
                return false;
            }

            return WebHookSubscriptionsStore.IsSubscribed(tenantId, webHookName);
        }

        public async Task AddOrUpdateSubscriptionAsync(WebHookSubscription webHookSubscription)
        {
            await CheckIfPermissionsGrantedAsync(webHookSubscription);

            if (webHookSubscription.Id == default)
            {
                webHookSubscription.Id = _guidGenerator.Create();
                webHookSubscription.Secret = "whs_" + Guid.NewGuid().ToString().Replace("-", "");
                await WebHookSubscriptionsStore.InsertAsync(webHookSubscription.ToWebHookSubscriptionInfo());
            }
            else
            {
                await WebHookSubscriptionsStore.UpdateAsync(webHookSubscription.ToWebHookSubscriptionInfo());
            }
        }

        public void AddOrUpdateSubscription(WebHookSubscription webHookSubscription)
        {
            CheckIfPermissionsGranted(webHookSubscription);

            if (webHookSubscription.Id == default)
            {
                webHookSubscription.Id = _guidGenerator.Create();
                webHookSubscription.Secret = "whs_" + Guid.NewGuid().ToString().Replace("-", "");
                WebHookSubscriptionsStore.Insert(webHookSubscription.ToWebHookSubscriptionInfo());
            }
            else
            {
                WebHookSubscriptionsStore.Update(webHookSubscription.ToWebHookSubscriptionInfo());
            }
        }

        [UnitOfWork]
        public async Task AddWebHookAsync(WebHookSubscriptionInfo subscription, string webHookName)
        {
            await CheckPermissionsAsync(subscription.TenantId, webHookName);

            subscription.AddWebHookDefinition(webHookName);
        }

        [UnitOfWork]
        public void AddWebHook(WebHookSubscriptionInfo subscription, string webHookName)
        {
            CheckPermissions(subscription.TenantId, webHookName);

            subscription.AddWebHookDefinition(webHookName);
        }

        #region PermissionCheck

        private async Task CheckIfPermissionsGrantedAsync(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.WebHookDefinitions == null || webHookSubscription.WebHookDefinitions.Count == 0)
            {
                return;
            }

            foreach (var webHookDefinition in webHookSubscription.WebHookDefinitions)
            {
                await CheckPermissionsAsync(webHookSubscription.TenantId, webHookDefinition);
            }
        }

        private async Task CheckPermissionsAsync(int? tenantId, string webHookName)
        {
            if (!await _webHookDefinitionManager.IsAvailableAsync(tenantId, webHookName))
            {
                //TODO: improve exception message
                throw new AbpAuthorizationException($"Tenant \"{tenantId}\" must have necessary feature(s) to use webhook \"{webHookName}\"");
            }
        }

        private void CheckIfPermissionsGranted(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.WebHookDefinitions == null || webHookSubscription.WebHookDefinitions.Count == 0)
            {
                return;
            }

            foreach (var webHookDefinition in webHookSubscription.WebHookDefinitions)
            {
                CheckPermissions(webHookSubscription.TenantId, webHookDefinition);
            }
        }

        private void CheckPermissions(int? tenantId, string webHookName)
        {
            if (!_webHookDefinitionManager.IsAvailable(tenantId, webHookName))
            {
                //TODO: improve exception message
                throw new AbpAuthorizationException($"Tenant \"{tenantId}\" must have necessary feature(s) to use webhook \"{webHookName}\"");
            }
        }


        #endregion
    }
}
