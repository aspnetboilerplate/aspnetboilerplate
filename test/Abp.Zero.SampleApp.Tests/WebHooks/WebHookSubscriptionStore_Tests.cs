using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.WebHooks;
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

        private const string TestWebHookDefinitionName = "Test";

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
            return CreateTestSubscription(TestWebHookDefinitionName);
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

            (await _webHookSubscriptionsStore.IsSubscribedAsync(AbpSession.ToUserIdentifier(), TestWebHookDefinitionName)).ShouldBeTrue();
            (await _webHookSubscriptionsStore.IsSubscribedAsync(AbpSession.ToUserIdentifier(), TestWebHookDefinitionName + "asd")).ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Get_All_Subscriptions()
        {
            await CreateTestSubscription();
            await CreateTestSubscription("user_created");
            await CreateTestSubscription();
            await CreateTestSubscription("user_created", "admin.tenant_deleted");

            var testSubscriptions = _webHookSubscriptionsStore.GetAllSubscriptions(TestWebHookDefinitionName);
            testSubscriptions.Count.ShouldBe(2);

            foreach (var webHookSubscriptionInfo in testSubscriptions)
            {
                webHookSubscriptionInfo.WebHookDefinitions.ShouldContain(TestWebHookDefinitionName);
            }

            var userCreatedSubscriptions = _webHookSubscriptionsStore.GetAllSubscriptions("user_created");
            userCreatedSubscriptions.Count.ShouldBe(2);
            foreach (var webHookSubscriptionInfo in userCreatedSubscriptions)
            {
                webHookSubscriptionInfo.WebHookDefinitions.ShouldContain("user_created");
            }

            userCreatedSubscriptions[1].WebHookDefinitions.ShouldContain("admin.tenant_deleted");
        }

        [Fact]
        public async Task Should_Subscribe_To_Multiple_Event()
        {
            var subscription = await CreateTestSubscription(TestWebHookDefinitionName, "user_created", "admin.tenant_deleted");

            var sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);

            sub.WebHookDefinitions.ShouldContain(TestWebHookDefinitionName);
            sub.WebHookDefinitions.ShouldContain("user_created");
            sub.WebHookDefinitions.ShouldContain("admin.tenant_deleted");

            var testSubscriptions1 = _webHookSubscriptionsStore.GetAllSubscriptions(TestWebHookDefinitionName);
            testSubscriptions1.Count.ShouldBe(1);
            testSubscriptions1.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions2 = _webHookSubscriptionsStore.GetAllSubscriptions(TestWebHookDefinitionName);
            testSubscriptions2.Count.ShouldBe(1);
            testSubscriptions2.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions3 = _webHookSubscriptionsStore.GetAllSubscriptions(TestWebHookDefinitionName);
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
            await CreateTestSubscription(TestWebHookDefinitionName);
            await CreateTestSubscription(TestWebHookDefinitionName, "user_created");
            await CreateTestSubscription(TestWebHookDefinitionName, "user_created", "admin.tenant_deleted");

            var subscribedWebHooks = await _webHookSubscriptionsStore.GetAllSubscriptionsAsync(AbpSession.ToUserIdentifier());
            subscribedWebHooks.Count.ShouldBe(3);

            subscribedWebHooks[0].GetWebHookDefinitions().Count.ShouldBe(1);
            subscribedWebHooks[0].WebHookDefinitions.ShouldContain(TestWebHookDefinitionName);

            subscribedWebHooks[1].GetWebHookDefinitions().Count.ShouldBe(2);
            subscribedWebHooks[1].WebHookDefinitions.ShouldContain(TestWebHookDefinitionName);
            subscribedWebHooks[1].WebHookDefinitions.ShouldContain("user_created");

            subscribedWebHooks[2].GetWebHookDefinitions().Count.ShouldBe(3);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(TestWebHookDefinitionName);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain("user_created");
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain("admin.tenant_deleted");
        }

        [Fact]
        public async Task Should_Update_Subscription()
        {
            var subscription = await CreateTestSubscription(TestWebHookDefinitionName, "user_created", "admin.tenant_deleted");

            string headerKey = "MyHeaderKey", headerValue = "MyHeaderValue";

            subscription.AddWebHookHeader(headerKey, headerValue);
            subscription.RemoveWebHookDefinition("user_created");
            subscription.WebHookUri = "www.aspnetboilerplate.com";
            subscription.Secret = "secret2";

            await _webHookSubscriptionsStore.UpdateAsync(subscription);

            var sub = _webHookSubscriptionsStore.Get(subscription.Id);

            var webHookDefinitionAsList = sub.GetWebHookDefinitions();
            webHookDefinitionAsList.Count.ShouldBe(2);
            webHookDefinitionAsList[0].ShouldBe(TestWebHookDefinitionName);
            webHookDefinitionAsList[1].ShouldBe("admin.tenant_deleted");

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
            var subscription = await CreateTestSubscription(TestWebHookDefinitionName);

            (await _webHookSubscriptionsStore.GetAsync(subscription.Id)).ShouldNotBeNull();

            await _webHookSubscriptionsStore.DeleteAsync(subscription.Id);

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _webHookSubscriptionsStore.GetAsync(subscription.Id);
            });
        }
    }
}