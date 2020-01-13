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
        private readonly IWebHookSubscriptionsStore _webHookSubscriptionsStore;

        public WebHookSubscriptionStore_Tests()
        {
            _webHookSubscriptionsStore = Resolve<IWebHookSubscriptionsStore>();

            AbpSession.TenantId = null;
            AbpSession.UserId = 1;
        }


        #region Async

        private async Task<WebHookSubscriptionInfo> CreateTestSubscriptionAsync(params string[] webHookDefinition)
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
                        };

                        foreach (var definition in webHookDefinition)
                        {
                            subscription.SubscribeWebhook(definition);
                        }

                        await _webHookSubscriptionsStore.InsertAsync(subscription);

                        await uow.CompleteAsync();
                        return subscription;
                    }
                }
            }
        }

        private Task<WebHookSubscriptionInfo> CreateTestSubscriptionAsync()
        {
            return CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Test);
        }

        [Fact]
        public async Task Should_Insert_Subscription_Async()
        {
            var subscription = await CreateTestSubscriptionAsync();
            WebHookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);
                    }
                }
            }

            sub.ShouldNotBeNull();
            sub.WebHookDefinitions.ShouldContain("Test");
            sub.Secret.ShouldBe("secret");
        }

        [Fact]
        public async Task Should_Get_Is_Subscribed_To_WebHooks_Async()
        {
            await CreateTestSubscriptionAsync();

            (await _webHookSubscriptionsStore.IsSubscribedAsync(AbpSession.TenantId, AppWebHookDefinitionNames.Test)).ShouldBeTrue();
            (await _webHookSubscriptionsStore.IsSubscribedAsync(AbpSession.TenantId, AppWebHookDefinitionNames.Test + "asd")).ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Get_All_Subscriptions_Async()
        {
            await CreateTestSubscriptionAsync();//AppWebHookDefinitionNames.Test
            await CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Users.Created);
            await CreateTestSubscriptionAsync();//AppWebHookDefinitionNames.Test
            await CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            var testSubscriptions = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Test);
            testSubscriptions.Count.ShouldBe(2);

            foreach (var webHookSubscriptionInfo in testSubscriptions)
            {
                webHookSubscriptionInfo.GetSubscribedWebhooks().Single().ShouldBe(AppWebHookDefinitionNames.Test);
            }

            var userCreatedSubscriptions = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
            foreach (var webHookSubscriptionInfo in userCreatedSubscriptions)
            {
                webHookSubscriptionInfo.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            }

            userCreatedSubscriptions[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
        }

        [Fact]
        public async Task Should_Subscribe_To_Multiple_Event_Async()
        {
            var subscription = await CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            WebHookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);
                    }
                }
            }

            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            var testSubscriptions1 = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Test);
            testSubscriptions1.Count.ShouldBe(1);
            testSubscriptions1.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions2 = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Users.Created);
            testSubscriptions2.Count.ShouldBe(1);
            testSubscriptions2.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions3 = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            testSubscriptions3.Count.ShouldBe(1);
            testSubscriptions3.Single().Id.ShouldBe(sub.Id);
        }

        [Fact]
        public async Task Should_Get_Subscribed_WebHooks_Async()
        {
            await CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Test);
            await CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created);
            await CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            var subscribedWebHooks = await _webHookSubscriptionsStore.GetAllSubscriptionsAsync(AbpSession.TenantId);
            subscribedWebHooks.Count.ShouldBe(3);

            subscribedWebHooks[0].GetSubscribedWebhooks().Count.ShouldBe(1);
            subscribedWebHooks[0].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);

            subscribedWebHooks[1].GetSubscribedWebhooks().Count.ShouldBe(2);
            subscribedWebHooks[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            subscribedWebHooks[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

            subscribedWebHooks[2].GetSubscribedWebhooks().Count.ShouldBe(3);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
        }

        [Fact]
        public async Task Should_Update_Subscription_Async()
        {
            var subscription = await CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            string headerKey = "MyHeaderKey", headerValue = "MyHeaderValue";

            subscription.AddWebHookHeader(headerKey, headerValue);
            subscription.UnsubscribeWebhook(AppWebHookDefinitionNames.Users.Created);
            subscription.WebHookUri = "www.aspnetboilerplate.com";
            subscription.Secret = "secret2";

            await _webHookSubscriptionsStore.UpdateAsync(subscription);

            WebHookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = await _webHookSubscriptionsStore.GetAsync(subscription.Id);
                    }
                }
            }

            var webHookDefinitionAsList = sub.GetSubscribedWebhooks();
            webHookDefinitionAsList.Count.ShouldBe(2);
            webHookDefinitionAsList[0].ShouldBe(AppWebHookDefinitionNames.Test);
            webHookDefinitionAsList[1].ShouldBe(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            var additionalHeaderAsDictionary = sub.GetWebHookHeaders();
            additionalHeaderAsDictionary.Count.ShouldBe(1);
            additionalHeaderAsDictionary.ShouldContainKey(headerKey);
            additionalHeaderAsDictionary[headerKey].ShouldBe(headerValue);

            sub.WebHookUri.ShouldBe(subscription.WebHookUri);
            sub.Secret.ShouldBe(subscription.Secret);
        }

        [Fact]
        public async Task Should_Delete_Subscription_Async()
        {
            var subscription = await CreateTestSubscriptionAsync(AppWebHookDefinitionNames.Test);

            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        (await _webHookSubscriptionsStore.GetAsync(subscription.Id)).ShouldNotBeNull();

                        await _webHookSubscriptionsStore.DeleteAsync(subscription.Id);
                        await uow.CompleteAsync();

                        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                        {
                            await _webHookSubscriptionsStore.GetAsync(subscription.Id);
                        });
                    }
                }
            }
        }

        #endregion

        #region Sync
        private WebHookSubscriptionInfo CreateTestSubscriptionSync(params string[] webHookDefinition)
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
                            Secret = "secret"
                        };

                        foreach (var definition in webHookDefinition)
                        {
                            subscription.SubscribeWebhook(definition);
                        }

                        _webHookSubscriptionsStore.Insert(subscription);

                        uow.Complete();
                        return subscription;
                    }
                }
            }
        }

        private WebHookSubscriptionInfo CreateTestSubscriptionSync()
        {
            return CreateTestSubscriptionSync(AppWebHookDefinitionNames.Test);
        }

        [Fact]
        public void Should_Insert_Subscription_Sync()
        {
            var subscription = CreateTestSubscriptionSync();

            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        var sub = _webHookSubscriptionsStore.Get(subscription.Id);

                        sub.ShouldNotBeNull();
                        sub.WebHookDefinitions.ShouldContain("Test");
                        sub.Secret.ShouldBe("secret");
                    }
                }
            }
        }

        [Fact]
        public void Should_Get_Is_Subscribed_To_WebHooks_Sync()
        {
            CreateTestSubscriptionSync();

            _webHookSubscriptionsStore.IsSubscribed(AbpSession.TenantId, AppWebHookDefinitionNames.Test).ShouldBeTrue();
            _webHookSubscriptionsStore.IsSubscribed(AbpSession.TenantId, AppWebHookDefinitionNames.Test + "asd").ShouldBeFalse();
        }

        [Fact]
        public void Should_Get_All_Subscriptions_Sync()
        {
            CreateTestSubscriptionSync();//AppWebHookDefinitionNames.Test
            CreateTestSubscriptionSync(AppWebHookDefinitionNames.Users.Created);
            CreateTestSubscriptionSync();//AppWebHookDefinitionNames.Test
            CreateTestSubscriptionSync(AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            var testSubscriptions = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Test);
            testSubscriptions.Count.ShouldBe(2);

            foreach (var webHookSubscriptionInfo in testSubscriptions)
            {
                webHookSubscriptionInfo.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            }

            var userCreatedSubscriptions = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
            foreach (var webHookSubscriptionInfo in userCreatedSubscriptions)
            {
                webHookSubscriptionInfo.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            }

            userCreatedSubscriptions[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
        }

        [Fact]
        public void Should_Subscribe_To_Multiple_Event_Sync()
        {
            var subscription = CreateTestSubscriptionSync(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            WebHookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = _webHookSubscriptionsStore.Get(subscription.Id);
                    }
                }
            }

            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            sub.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            var testSubscriptions1 = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Test);
            testSubscriptions1.Count.ShouldBe(1);
            testSubscriptions1.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions2 = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Users.Created);
            testSubscriptions2.Count.ShouldBe(1);
            testSubscriptions2.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions3 = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            testSubscriptions3.Count.ShouldBe(1);
            testSubscriptions3.Single().Id.ShouldBe(sub.Id);
        }

        [Fact]
        public void Should_Get_Subscribed_WebHooks_Sync()
        {
            CreateTestSubscriptionSync(AppWebHookDefinitionNames.Test);
            CreateTestSubscriptionSync(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created);
            CreateTestSubscriptionSync(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            var subscribedWebHooks = _webHookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId);
            subscribedWebHooks.Count.ShouldBe(3);

            subscribedWebHooks[0].GetSubscribedWebhooks().Count.ShouldBe(1);
            subscribedWebHooks[0].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);

            subscribedWebHooks[1].GetSubscribedWebhooks().Count.ShouldBe(2);
            subscribedWebHooks[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            subscribedWebHooks[1].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

            subscribedWebHooks[2].GetSubscribedWebhooks().Count.ShouldBe(3);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscribedWebHooks[2].WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
        }

        [Fact]
        public void Should_Update_Subscription_Sync()
        {
            var subscription = CreateTestSubscriptionSync(AppWebHookDefinitionNames.Test, AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            string headerKey = "MyHeaderKey", headerValue = "MyHeaderValue";

            subscription.AddWebHookHeader(headerKey, headerValue);
            subscription.UnsubscribeWebhook(AppWebHookDefinitionNames.Users.Created);
            subscription.WebHookUri = "www.aspnetboilerplate.com";
            subscription.Secret = "secret2";

            _webHookSubscriptionsStore.Update(subscription);

            WebHookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = _webHookSubscriptionsStore.Get(subscription.Id);
                    }
                }
            }

            var webHookDefinitionAsList = sub.GetSubscribedWebhooks();
            webHookDefinitionAsList.Count.ShouldBe(2);
            webHookDefinitionAsList.ShouldNotContain(AppWebHookDefinitionNames.Users.Created);
            webHookDefinitionAsList[0].ShouldBe(AppWebHookDefinitionNames.Test);
            webHookDefinitionAsList[1].ShouldBe(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);

            var additionalHeaderAsDictionary = sub.GetWebHookHeaders();
            additionalHeaderAsDictionary.Count.ShouldBe(1);
            additionalHeaderAsDictionary.ShouldContainKey(headerKey);
            additionalHeaderAsDictionary[headerKey].ShouldBe(headerValue);

            sub.WebHookUri.ShouldBe(subscription.WebHookUri);
            sub.Secret.ShouldBe(subscription.Secret);
        }

        [Fact]
        public void Should_Delete_Subscription_Sync()
        {
            var subscription = CreateTestSubscriptionSync(AppWebHookDefinitionNames.Test);

            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        _webHookSubscriptionsStore.Get(subscription.Id).ShouldNotBeNull();

                        _webHookSubscriptionsStore.Delete(subscription.Id);
                        uow.Complete();
                        Assert.Throws<EntityNotFoundException>(() => { _webHookSubscriptionsStore.Get(subscription.Id); });
                    }
                }
            }
        }

        #endregion
    }
}