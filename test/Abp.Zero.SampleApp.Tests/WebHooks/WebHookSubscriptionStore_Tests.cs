using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Webhooks;
using Abp.Zero.SampleApp.Application;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class WebhookSubscriptionStore_Tests : SampleAppTestBase
    {
        private readonly IWebhookSubscriptionsStore _webhookSubscriptionsStore;

        public WebhookSubscriptionStore_Tests()
        {
            _webhookSubscriptionsStore = Resolve<IWebhookSubscriptionsStore>();

            AbpSession.TenantId = null;
            AbpSession.UserId = 1;
        }


        #region Async

        private async Task<WebhookSubscriptionInfo> CreateTestSubscriptionAsync(params string[] webhookDefinition)
        {
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        var subscription = new WebhookSubscriptionInfo()
                        {
                            WebhookUri = "www.test.com",
                            Secret = "secret",
                        };

                        foreach (var definition in webhookDefinition)
                        {
                            subscription.SubscribeWebhook(definition);
                        }

                        await _webhookSubscriptionsStore.InsertAsync(subscription);

                        await uow.CompleteAsync();
                        return subscription;
                    }
                }
            }
        }

        private Task<WebhookSubscriptionInfo> CreateTestSubscriptionAsync()
        {
            return CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Test);
        }

        [Fact]
        public async Task Should_Insert_Subscription_Async()
        {
            var subscription = await CreateTestSubscriptionAsync();
            WebhookSubscriptionInfo sub = null;

            await WithUnitOfWorkAsync(AbpSession.TenantId,
                async () =>
                {
                    sub = await _webhookSubscriptionsStore.GetAsync(subscription.Id);
                });

            sub.ShouldNotBeNull();
            sub.Webhooks.ShouldContain("Test");
            sub.Secret.ShouldBe("secret");
        }

        [Fact]
        public async Task Should_Get_Is_Subscribed_To_Webhooks_Async()
        {
            await CreateTestSubscriptionAsync();

            await WithUnitOfWorkAsync(async () =>
            {
                (await _webhookSubscriptionsStore.IsSubscribedAsync(AbpSession.TenantId, AppWebhookDefinitionNames.Test)).ShouldBeTrue();
                (await _webhookSubscriptionsStore.IsSubscribedAsync(AbpSession.TenantId, AppWebhookDefinitionNames.Test + "asd")).ShouldBeFalse();
            });
        }

        [Fact]
        public async Task Should_Get_All_Subscriptions_Async()
        {
            await CreateTestSubscriptionAsync();//AppWebhookDefinitionNames.Test
            await CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Users.Created);
            await CreateTestSubscriptionAsync();//AppWebhookDefinitionNames.Test
            await CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Users.Created, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            var testSubscriptions = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Test);
            testSubscriptions.Count.ShouldBe(2);

            foreach (var webhookSubscriptionInfo in testSubscriptions)
            {
                webhookSubscriptionInfo.GetSubscribedWebhooks().Single().ShouldBe(AppWebhookDefinitionNames.Test);
            }

            var userCreatedSubscriptions = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
            foreach (var webhookSubscriptionInfo in userCreatedSubscriptions)
            {
                webhookSubscriptionInfo.Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);
            }

            userCreatedSubscriptions[1].Webhooks.ShouldContain(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
        }

        [Fact]
        public async Task Should_Subscribe_To_Multiple_Event_Async()
        {
            var subscription = await CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Test, AppWebhookDefinitionNames.Users.Created, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            WebhookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = await _webhookSubscriptionsStore.GetAsync(subscription.Id);
                    }
                }
            }

            sub.Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
            sub.Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);
            sub.Webhooks.ShouldContain(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            var testSubscriptions1 = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Test);
            testSubscriptions1.Count.ShouldBe(1);
            testSubscriptions1.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions2 = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Users.Created);
            testSubscriptions2.Count.ShouldBe(1);
            testSubscriptions2.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions3 = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            testSubscriptions3.Count.ShouldBe(1);
            testSubscriptions3.Single().Id.ShouldBe(sub.Id);
        }

        [Fact]
        public async Task Should_Get_Subscribed_Webhooks_Async()
        {
            await CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Test);
            await CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Test, AppWebhookDefinitionNames.Users.Created);
            await CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Test, AppWebhookDefinitionNames.Users.Created, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            var subscribedWebhooks = await _webhookSubscriptionsStore.GetAllSubscriptionsAsync(AbpSession.TenantId);
            subscribedWebhooks.Count.ShouldBe(3);

            subscribedWebhooks[0].GetSubscribedWebhooks().Count.ShouldBe(1);
            subscribedWebhooks[0].Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);

            subscribedWebhooks[1].GetSubscribedWebhooks().Count.ShouldBe(2);
            subscribedWebhooks[1].Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
            subscribedWebhooks[1].Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);

            subscribedWebhooks[2].GetSubscribedWebhooks().Count.ShouldBe(3);
            subscribedWebhooks[2].Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
            subscribedWebhooks[2].Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);
            subscribedWebhooks[2].Webhooks.ShouldContain(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
        }

        [Fact]
        public async Task Should_Update_Subscription_Async()
        {
            var subscription = await CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Test, AppWebhookDefinitionNames.Users.Created, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            string headerKey = "MyHeaderKey", headerValue = "MyHeaderValue";

            subscription.AddWebhookHeader(headerKey, headerValue);
            subscription.UnsubscribeWebhook(AppWebhookDefinitionNames.Users.Created);
            subscription.WebhookUri = "www.aspnetboilerplate.com";
            subscription.Secret = "secret2";

            await _webhookSubscriptionsStore.UpdateAsync(subscription);

            WebhookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = await _webhookSubscriptionsStore.GetAsync(subscription.Id);
                    }
                }
            }

            var webhookDefinitionAsList = sub.GetSubscribedWebhooks();
            webhookDefinitionAsList.Count.ShouldBe(2);
            webhookDefinitionAsList[0].ShouldBe(AppWebhookDefinitionNames.Test);
            webhookDefinitionAsList[1].ShouldBe(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            var additionalHeaderAsDictionary = sub.GetWebhookHeaders();
            additionalHeaderAsDictionary.Count.ShouldBe(1);
            additionalHeaderAsDictionary.ShouldContainKey(headerKey);
            additionalHeaderAsDictionary[headerKey].ShouldBe(headerValue);

            sub.WebhookUri.ShouldBe(subscription.WebhookUri);
            sub.Secret.ShouldBe(subscription.Secret);
        }

        [Fact]
        public async Task Should_Delete_Subscription_Async()
        {
            var subscription = await CreateTestSubscriptionAsync(AppWebhookDefinitionNames.Test);

            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        (await _webhookSubscriptionsStore.GetAsync(subscription.Id)).ShouldNotBeNull();

                        await _webhookSubscriptionsStore.DeleteAsync(subscription.Id);
                        await uow.CompleteAsync();

                        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                        {
                            await _webhookSubscriptionsStore.GetAsync(subscription.Id);
                        });
                    }
                }
            }
        }

        #endregion

        #region Sync
        private WebhookSubscriptionInfo CreateTestSubscriptionSync(params string[] webhookDefinition)
        {
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        var subscription = new WebhookSubscriptionInfo()
                        {
                            WebhookUri = "www.test.com",
                            Secret = "secret"
                        };

                        foreach (var definition in webhookDefinition)
                        {
                            subscription.SubscribeWebhook(definition);
                        }

                        _webhookSubscriptionsStore.Insert(subscription);

                        uow.Complete();
                        return subscription;
                    }
                }
            }
        }

        private WebhookSubscriptionInfo CreateTestSubscriptionSync()
        {
            return CreateTestSubscriptionSync(AppWebhookDefinitionNames.Test);
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
                        var sub = _webhookSubscriptionsStore.Get(subscription.Id);

                        sub.ShouldNotBeNull();
                        sub.Webhooks.ShouldContain("Test");
                        sub.Secret.ShouldBe("secret");
                    }
                }
            }
        }

        [Fact]
        public void Should_Get_Is_Subscribed_To_Webhooks_Sync()
        {
            CreateTestSubscriptionSync();

            WithUnitOfWork(() =>
            {
                _webhookSubscriptionsStore.IsSubscribed(AbpSession.TenantId, AppWebhookDefinitionNames.Test).ShouldBeTrue();
                _webhookSubscriptionsStore.IsSubscribed(AbpSession.TenantId, AppWebhookDefinitionNames.Test + "asd").ShouldBeFalse();
            });
        }

        [Fact]
        public void Should_Get_All_Subscriptions_Sync()
        {
            CreateTestSubscriptionSync();//AppWebhookDefinitionNames.Test
            CreateTestSubscriptionSync(AppWebhookDefinitionNames.Users.Created);
            CreateTestSubscriptionSync();//AppWebhookDefinitionNames.Test
            CreateTestSubscriptionSync(AppWebhookDefinitionNames.Users.Created, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            var testSubscriptions = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Test);
            testSubscriptions.Count.ShouldBe(2);

            foreach (var webhookSubscriptionInfo in testSubscriptions)
            {
                webhookSubscriptionInfo.Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
            }

            var userCreatedSubscriptions = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
            foreach (var webhookSubscriptionInfo in userCreatedSubscriptions)
            {
                webhookSubscriptionInfo.Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);
            }

            userCreatedSubscriptions[1].Webhooks.ShouldContain(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
        }

        [Fact]
        public void Should_Subscribe_To_Multiple_Event_Sync()
        {
            var subscription = CreateTestSubscriptionSync(AppWebhookDefinitionNames.Test, AppWebhookDefinitionNames.Users.Created, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            WebhookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = _webhookSubscriptionsStore.Get(subscription.Id);
                    }
                }
            }

            sub.Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
            sub.Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);
            sub.Webhooks.ShouldContain(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            var testSubscriptions1 = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Test);
            testSubscriptions1.Count.ShouldBe(1);
            testSubscriptions1.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions2 = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Users.Created);
            testSubscriptions2.Count.ShouldBe(1);
            testSubscriptions2.Single().Id.ShouldBe(sub.Id);

            var testSubscriptions3 = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            testSubscriptions3.Count.ShouldBe(1);
            testSubscriptions3.Single().Id.ShouldBe(sub.Id);
        }

        [Fact]
        public void Should_Get_Subscribed_Webhooks_Sync()
        {
            CreateTestSubscriptionSync(AppWebhookDefinitionNames.Test);
            CreateTestSubscriptionSync(AppWebhookDefinitionNames.Test, AppWebhookDefinitionNames.Users.Created);
            CreateTestSubscriptionSync(AppWebhookDefinitionNames.Test, AppWebhookDefinitionNames.Users.Created, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            var subscribedWebhooks = _webhookSubscriptionsStore.GetAllSubscriptions(AbpSession.TenantId);
            subscribedWebhooks.Count.ShouldBe(3);

            subscribedWebhooks[0].GetSubscribedWebhooks().Count.ShouldBe(1);
            subscribedWebhooks[0].Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);

            subscribedWebhooks[1].GetSubscribedWebhooks().Count.ShouldBe(2);
            subscribedWebhooks[1].Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
            subscribedWebhooks[1].Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);

            subscribedWebhooks[2].GetSubscribedWebhooks().Count.ShouldBe(3);
            subscribedWebhooks[2].Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
            subscribedWebhooks[2].Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);
            subscribedWebhooks[2].Webhooks.ShouldContain(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
        }

        [Fact]
        public void Should_Update_Subscription_Sync()
        {
            var subscription = CreateTestSubscriptionSync(AppWebhookDefinitionNames.Test, AppWebhookDefinitionNames.Users.Created, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            string headerKey = "MyHeaderKey", headerValue = "MyHeaderValue";

            subscription.AddWebhookHeader(headerKey, headerValue);
            subscription.UnsubscribeWebhook(AppWebhookDefinitionNames.Users.Created);
            subscription.WebhookUri = "www.aspnetboilerplate.com";
            subscription.Secret = "secret2";

            _webhookSubscriptionsStore.Update(subscription);

            WebhookSubscriptionInfo sub;
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        sub = _webhookSubscriptionsStore.Get(subscription.Id);
                    }
                }
            }

            var webhookDefinitionAsList = sub.GetSubscribedWebhooks();
            webhookDefinitionAsList.Count.ShouldBe(2);
            webhookDefinitionAsList.ShouldNotContain(AppWebhookDefinitionNames.Users.Created);
            webhookDefinitionAsList[0].ShouldBe(AppWebhookDefinitionNames.Test);
            webhookDefinitionAsList[1].ShouldBe(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);

            var additionalHeaderAsDictionary = sub.GetWebhookHeaders();
            additionalHeaderAsDictionary.Count.ShouldBe(1);
            additionalHeaderAsDictionary.ShouldContainKey(headerKey);
            additionalHeaderAsDictionary[headerKey].ShouldBe(headerValue);

            sub.WebhookUri.ShouldBe(subscription.WebhookUri);
            sub.Secret.ShouldBe(subscription.Secret);
        }

        [Fact]
        public void Should_Delete_Subscription_Sync()
        {
            var subscription = CreateTestSubscriptionSync(AppWebhookDefinitionNames.Test);

            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin())
                {
                    using (uowManager.Object.Current.SetTenantId(AbpSession.TenantId))
                    {
                        _webhookSubscriptionsStore.Get(subscription.Id).ShouldNotBeNull();

                        _webhookSubscriptionsStore.Delete(subscription.Id);
                        uow.Complete();
                        Assert.Throws<EntityNotFoundException>(() => { _webhookSubscriptionsStore.Get(subscription.Id); });
                    }
                }
            }
        }

        #endregion
    }
}