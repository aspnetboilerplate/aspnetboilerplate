using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookSubscriptionStore_Tests : SampleAppTestBase
    {
        private IWebHookSubscriptionsStore _webHookSubscriptionsStore;

        public WebHookSubscriptionStore_Tests()
        {
            _webHookSubscriptionsStore = Resolve<IWebHookSubscriptionsStore>();

            AbpSession.TenantId = null;
            AbpSession.UserId = 1;
        }


        private async Task<WebHookSubscriptionInfo> CreateTestSubscription(params string[] webHookDefinition)
        {
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        var subscription = new WebHookSubscriptionInfo()
                        {
                            WebHookUri = "www.test.com",
                            Secret = "secret",
                            UserId = AbpSession.UserId.Value
                        };

                        foreach (var definition in webHookDefinition)
                        {
                            subscription.AddWebHookDefinition(definition);
                        }

                        await _webHookSubscriptionsStore.InsertAsync(subscription);

                        await uow.CompleteAsync();
                        return subscription;
                    }
                }
            }
        }
        private Task<WebHookSubscriptionInfo> CreateTestSubscription()
        {
            return CreateTestSubscription(AppWebHookDefinitionNames.Test);
        }

        [Fact]
        public async Task Should_Insert_Subscription()
        {
            var subscription = await CreateTestSubscription();

            var sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);

            sub.ShouldNotBeNull();
            sub.WebHookDefinitions.ShouldContain("Test");
            sub.Secret.ShouldBe("secret");
        }

        [Fact]
        public async Task Should_Get_Is_Subscribed_To_WebHooks()
        {
            await CreateTestSubscription();

            (await _webHookSubscriptionsStore.IsSubscribedAsync(AbpSession.ToUserIdentifier(), AppWebHookDefinitionNames.Test)).ShouldBeTrue();
            (await _webHookSubscriptionsStore.IsSubscribedAsync(AbpSession.ToUserIdentifier(), AppWebHookDefinitionNames.Test + "asd")).ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Get_All_Subscriptions()
        {
            await CreateTestSubscription();
            await CreateTestSubscription(AppWebHookDefinitionNames.Users.Created);
            await CreateTestSubscription();
            await CreateTestSubscription(AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Tenant.Deleted);

            var testSubscriptions = _webHookSubscriptionsStore.GetAllSubscriptions(AppWebHookDefinitionNames.Test);
            testSubscriptions.Count.ShouldBe(2);

            foreach (var webHookSubscriptionInfo in testSubscriptions)
            {
                webHookSubscriptionInfo.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            }

            var userCreatedSubscriptions = _webHookSubscriptionsStore.GetAllSubscriptions(AppWebHookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
            foreach (var webHookSubscriptionInfo in userCreatedSubscriptions)
            {
                webHookSubscriptionInfo.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            }

            userCreatedSubscriptions[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Tenant.Deleted);
        }

        [Fact]
        public async Task Should_Subscribe_To_Multiple_Event()
        {
            var subscription = await CreateTestSubscription(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Tenant.Deleted);

            var sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);

            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Tenant.Deleted);

            var testSubscriptions1 = _webHookSubscriptionsStore.GetAllSubscriptions(AppWebHookDefinitionNames.Test);
            testSubscriptions1.Count.ShouldBe(1);
            testSubscriptions1.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions2 = _webHookSubscriptionsStore.GetAllSubscriptions(AppWebHookDefinitionNames.Test);
            testSubscriptions2.Count.ShouldBe(1);
            testSubscriptions2.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions3 = _webHookSubscriptionsStore.GetAllSubscriptions(AppWebHookDefinitionNames.Test);
            testSubscriptions3.Count.ShouldBe(1);
            testSubscriptions3.Single().Id.ShouldBe(sub.Id);
        }

        [Fact]
        public async Task Set_Active_Tests()
        {
            var subscription = await CreateTestSubscription();

            var sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);
            sub.IsActive.ShouldBeTrue();

            await _webHookSubscriptionsStore.SetActiveAsync(subscription.Id, false);
            sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);
            sub.IsActive.ShouldBeFalse();

            await _webHookSubscriptionsStore.SetActiveAsync(subscription.Id, true);
            sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);
            sub.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Get_Subscribed_WebHooks()
        {
            await CreateTestSubscription(AppWebHookDefinitionNames.Test);
            await CreateTestSubscription(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created);
            await CreateTestSubscription(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Tenant.Deleted);

            var subscribedWebHooks = await _webHookSubscriptionsStore.GetAllSubscriptionsAsync(AbpSession.ToUserIdentifier());
            subscribedWebHooks.Count.ShouldBe(3);

            subscribedWebHooks[0].GetWebHookDefinitions().Count.ShouldBe(1);
            subscribedWebHooks[0].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);

            subscribedWebHooks[1].GetWebHookDefinitions().Count.ShouldBe(2);
            subscribedWebHooks[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            subscribedWebHooks[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

            subscribedWebHooks[2].GetWebHookDefinitions().Count.ShouldBe(3);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Tenant.Deleted);
        }

        [Fact]
        public async Task Should_Update_Subscription()
        {
            var subscription = await CreateTestSubscription(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Tenant.Deleted);

            string headerKey = "MyHeaderKey", headerValue = "MyHeaderValue";

            subscription.AddWebHookHeader(headerKey, headerValue);
            subscription.RemoveWebHookDefinition(AppWebHookDefinitionNames.Users.Created);
            subscription.WebHookUri = "www.aspnetboilerplate.com";
            subscription.Secret = "secret2";

            await _webHookSubscriptionsStore.UpdateAsync(subscription);

            var sub = _webHookSubscriptionsStore.Get(subscription.Id);

            var webHookDefinitionAsList = sub.GetWebHookDefinitions();
            webHookDefinitionAsList.Count.ShouldBe(2);
            webHookDefinitionAsList[0].ShouldBe(AppWebHookDefinitionNames.Test);
            webHookDefinitionAsList[1].ShouldBe(AppWebHookDefinitionNames.Tenant.Deleted);

            var additionalHeaderAsDictionary = sub.GetWebHookHeaders();
            additionalHeaderAsDictionary.Count.ShouldBe(1);
            additionalHeaderAsDictionary.ShouldContainKey(headerKey);
            additionalHeaderAsDictionary[headerKey].ShouldBe(headerValue);

            sub.WebHookUri.ShouldBe(subscription.WebHookUri);
            sub.Secret.ShouldBe(subscription.Secret);
        }

        [Fact]
        public async Task Should_Delete_Subscription()
        {
            var subscription = await CreateTestSubscription(AppWebHookDefinitionNames.Test);

            (await _webHookSubscriptionsStore.GetAsync(subscription.Id)).ShouldNotBeNull();

            await _webHookSubscriptionsStore.DeleteAsync(subscription.Id);

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _webHookSubscriptionsStore.GetAsync(subscription.Id);
            });
        }
    }
}