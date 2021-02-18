using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Json;
using Abp.Threading;
using Abp.Webhooks;
using Abp.Zero.SampleApp.Application;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class WebhookSubscriptionManager_Tests : WebhookTestBase
    {
        private WebhookSubscription NewWebhookSubscription(int? tenantId, params string[] webhookDefinitions)
        {
            return new WebhookSubscription
            {
                TenantId = tenantId,
                Webhooks = webhookDefinitions.ToList(),
                WebhookUri = "www.mywebhook.com",
                Headers = new Dictionary<string, string>
                {
                    {"Key", "Value"}
                }
            };
        }

        private WebhookSubscription NewWebhookSubscription(string seed, int? tenantId, params string[] webhookDefinitions)
        {
            return new WebhookSubscription
            {
                TenantId = tenantId,
                Webhooks = webhookDefinitions.ToList(),
                WebhookUri = "www.mywebhook.com/" + seed,
                Headers = new Dictionary<string, string>
                {
                    {"Key", "Value_" + seed}
                }
            };
        }

        private void CompareSubscriptions(WebhookSubscription subscription1, WebhookSubscription subscription2)
        {
            subscription1.Webhooks.ShouldBe(subscription2.Webhooks);
            subscription1.TenantId.ShouldBe(subscription2.TenantId);
            subscription1.Secret.ShouldNotBeNullOrEmpty();
            subscription1.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
            subscription1.WebhookUri.ShouldBe(subscription2.WebhookUri);
            subscription1.Webhooks.ShouldBe(subscription2.Webhooks);
            subscription1.Headers.ShouldBe(subscription2.Headers);
        }

        #region Async

        [Fact]
        public async Task Should_Insert_And_Get_Subscription_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true");

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            await WithUnitOfWorkAsync(tenantId, async () =>
            {
                var storedSubscription = await webhookSubscriptionManager.GetAsync(newSubscription.Id);
                storedSubscription.ShouldNotBeNull();
                CompareSubscriptions(storedSubscription, newSubscription);
            });
        }

        [Fact]
        public async Task Should_Update_Subscription_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true");

            AbpSession.TenantId = tenantId;
            
            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            WebhookSubscription subscription = null;
            await WithUnitOfWorkAsync(tenantId, async () =>
            {
                subscription = await webhookSubscriptionManager.GetAsync(newSubscription.Id);
                subscription.ShouldNotBeNull();
                CompareSubscriptions(subscription, newSubscription);
            });

            var newWebhookUri = "www.mynewwebhook.com";
            subscription.WebhookUri = newWebhookUri;

            subscription.Webhooks.Add(AppWebhookDefinitionNames.Test);

            var newHeader = new KeyValuePair<string, string>("NewKey", "NewValue");
            subscription.Headers.Add(newHeader);

            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

            await WithUnitOfWorkAsync(tenantId, async () =>
             {
                 var updatedSubscription = await webhookSubscriptionManager.GetAsync(newSubscription.Id);
                 updatedSubscription.ShouldNotBeNull();

                 updatedSubscription.WebhookUri.ShouldBe(newWebhookUri);

                 updatedSubscription.Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
                 updatedSubscription.Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);

                 updatedSubscription.TenantId.ShouldBe(tenantId);

                 updatedSubscription.Headers.ShouldContain(newSubscription.Headers.First());
                 updatedSubscription.Headers.ShouldContain(newHeader);
             });
        }

        [Fact]
        public async Task Should_Not_Update_Subscription_Secret_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true");

            AbpSession.TenantId = tenantId;
            
            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            WebhookSubscription storedSubscription = null;
            await WithUnitOfWorkAsync(tenantId, async () =>
             {
                 storedSubscription = await webhookSubscriptionManager.GetAsync(newSubscription.Id);
                 storedSubscription.ShouldNotBeNull();
                 CompareSubscriptions(storedSubscription, newSubscription);
             });

            //update
            string currentSecret = storedSubscription.Secret;
            storedSubscription.Secret = "TestSecret";

            webhookSubscriptionManager.AddOrUpdateSubscription(storedSubscription);

            await WithUnitOfWorkAsync(tenantId, async () =>
            {
                var updatedSubscription = await webhookSubscriptionManager.GetAsync(newSubscription.Id);
                updatedSubscription.ShouldNotBeNull();
                updatedSubscription.Secret.ShouldNotBeNullOrEmpty();
                updatedSubscription.Secret.ShouldBe(currentSecret);
            });
        }


        [Fact]
        public async Task Should_Insert_Throw_Exception_If_Features_Are_Not_Granted_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync();//needs AppFeatures.WebhookFeature feature

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });

            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.WebhookFeature, "true");
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
        }

        [Fact]
        public async Task Should_Update_Throw_Exception_If_Features_Are_Not_Granted_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync();//needs AppFeatures.WebhookFeature feature

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
            newSubscription.Id = Guid.NewGuid();

            var webhookStoreSubstitute = RegisterFake<IWebhookSubscriptionsStore>();
            webhookStoreSubstitute.GetAsync(Arg.Any<Guid>()).Returns(callback =>
                new WebhookSubscriptionInfo()
                {
                    Id = (Guid)callback.Args()[0],
                    TenantId = newSubscription.TenantId
                }
            );
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
                {
                    await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
                });

            //check error reason
            webhookStoreSubstitute.ClearReceivedCalls();
            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.WebhookFeature, "true");

            Predicate<WebhookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.WebhookUri.ShouldBe(newSubscription.WebhookUri);
                w.Webhooks.ShouldBe(newSubscription.Webhooks.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            await webhookStoreSubstitute.DidNotReceive().InsertAsync(Arg.Any<WebhookSubscriptionInfo>());
            await webhookStoreSubstitute.Received().UpdateAsync(Arg.Is<WebhookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Add_Or_Update_Subscription_Async_Method_Insert_If_Id_Is_Default_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync();

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Test);
            newSubscription.Id = default;

            var webhookStoreSubstitute = RegisterFake<IWebhookSubscriptionsStore>();
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            Predicate<WebhookSubscriptionInfo> predicate = w =>
            {
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookUri.ShouldBe(newSubscription.WebhookUri);
                w.Webhooks.ShouldBe(newSubscription.Webhooks.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            await webhookStoreSubstitute.DidNotReceive().UpdateAsync(Arg.Any<WebhookSubscriptionInfo>());
            await webhookStoreSubstitute.Received().InsertAsync(Arg.Is<WebhookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Add_Or_Update_Subscription_Async_Method_Update_If_Id_Is_Not_Default_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true");

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
            newSubscription.Id = Guid.NewGuid();

            var webhookStoreSubstitute = RegisterFake<IWebhookSubscriptionsStore>();
            webhookStoreSubstitute.GetAsync(Arg.Any<Guid>()).Returns(callback =>
                new WebhookSubscriptionInfo()
                {
                    Id = (Guid)callback.Args()[0],
                    TenantId = newSubscription.TenantId
                }
            );
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            Predicate<WebhookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.WebhookUri.ShouldBe(newSubscription.WebhookUri);
                w.Webhooks.ShouldBe(newSubscription.Webhooks.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            await webhookStoreSubstitute.DidNotReceive().InsertAsync(Arg.Any<WebhookSubscriptionInfo>());
            await webhookStoreSubstitute.Received().UpdateAsync(Arg.Is<WebhookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Get_All_Subscription_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true");

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var userCreatedWebhookSubscription = NewWebhookSubscription("1", tenantId, AppWebhookDefinitionNames.Users.Created);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebhookSubscription);

            var userCreatedWebhookSubscription2 = NewWebhookSubscription("2", tenantId, AppWebhookDefinitionNames.Users.Created);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebhookSubscription2);

            var testWebhookSubscription = NewWebhookSubscription("test", tenantId, AppWebhookDefinitionNames.Test);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(testWebhookSubscription);

            var allSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsAsync(tenantId);

            allSubscriptions.Count.ShouldBe(3);

            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebhookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebhookSubscription2.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(testWebhookSubscription.Id);

            var storedUserCreatedSubscription = allSubscriptions.Single(s => s.Id == userCreatedWebhookSubscription.Id);
            CompareSubscriptions(storedUserCreatedSubscription, userCreatedWebhookSubscription);

            var storedUserCreatedSubscription2 = allSubscriptions.Single(s => s.Id == userCreatedWebhookSubscription2.Id);
            CompareSubscriptions(storedUserCreatedSubscription2, userCreatedWebhookSubscription2);

            var storedTestSubscription = allSubscriptions.Single(s => s.Id == testWebhookSubscription.Id);
            CompareSubscriptions(storedTestSubscription, testWebhookSubscription);
        }

        [Fact]
        public async Task Should_Get_All_Subscriptions_If_Features_Granted_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebhookFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            });

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var userCreatedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebhookSubscription);

            var userCreatedAndThemeChangedWebhookSubscription = NewWebhookSubscription(tenantId,
                AppWebhookDefinitionNames.Users.Created,
                AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedAndThemeChangedWebhookSubscription);

            var defaultThemeChangedSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(defaultThemeChangedSubscription);

            var userCreatedSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebhookDefinitionNames.Users.Created);

            userCreatedSubscriptions.Count.ShouldBe(2);
            userCreatedSubscriptions.Select(s => s.Id).ShouldNotContain(defaultThemeChangedSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebhookSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebhookSubscription.Id);

            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebhookSubscription.Id), userCreatedAndThemeChangedWebhookSubscription);
            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedWebhookSubscription.Id), userCreatedWebhookSubscription);

            var defaultThemeChangedSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(2);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebhookSubscription.Id);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(defaultThemeChangedSubscription.Id);

            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebhookSubscription.Id), userCreatedAndThemeChangedWebhookSubscription);
            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == defaultThemeChangedSubscription.Id), defaultThemeChangedSubscription);
        }

        [Fact]
        public async Task Should_Not_Get_Subscriptions_If_Features_Not_Granted_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebhookFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            });
            
            AbpSession.TenantId = tenantId;

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var userCreatedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebhookSubscription);

            var userCreatedAndThemeChangedWebhookSubscription = NewWebhookSubscription(tenantId,
                AppWebhookDefinitionNames.Users.Created,
                AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedAndThemeChangedWebhookSubscription);

            var defaultThemeChangedSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(defaultThemeChangedSubscription);

            var userCreatedSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebhookDefinitionNames.Users.Created);

            userCreatedSubscriptions.Count.ShouldBe(2);
            userCreatedSubscriptions.Select(s => s.Id).ShouldNotContain(defaultThemeChangedSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebhookSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebhookSubscription.Id);

            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebhookSubscription.Id), userCreatedAndThemeChangedWebhookSubscription);
            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedWebhookSubscription.Id), userCreatedWebhookSubscription);

            var defaultThemeChangedSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(2);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebhookSubscription.Id);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(defaultThemeChangedSubscription.Id);

            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebhookSubscription.Id), userCreatedAndThemeChangedWebhookSubscription);
            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == defaultThemeChangedSubscription.Id), defaultThemeChangedSubscription);

            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.ThemeFeature, "false");

            defaultThemeChangedSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(0);

            userCreatedSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebhookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_Get_Is_Subscribed_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebhookFeature, "true"},
                {AppFeatures.TestFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}

            });

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            await WithUnitOfWorkAsync(async () =>
                {
                    var userCreatedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
                    await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebhookSubscription);

                    var userDeletedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Deleted);
                    await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userDeletedWebhookSubscription);

                    var defaultThemeChangedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
                    await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(defaultThemeChangedWebhookSubscription);
                });

            await WithUnitOfWorkAsync(async () =>
            {
                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Users.Created)).ShouldBeTrue();
                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Users.Deleted)).ShouldBeTrue();
                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeTrue();
            });

            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.TestFeature, "false");//Users.Deleted requires it but not require all 

            await WithUnitOfWorkAsync(async () =>
            {

                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Users.Created)).ShouldBeTrue();
                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Users.Deleted)).ShouldBeTrue();
                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeTrue();
            });

            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.ThemeFeature, "false");//DefaultThemeChanged requires and it is require all 

            await WithUnitOfWorkAsync(async () =>
            {

                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Users.Created)).ShouldBeTrue();
                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Users.Deleted)).ShouldBeTrue();
                (await webhookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeFalse();
            });
        }

        [Fact]
        public async Task Should_Allow_Host_To_Subscribe_Any_Webhooks_Async()
        {
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            var userCreatedWebhookSubscription = NewWebhookSubscription(null, AppWebhookDefinitionNames.Users.Created);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebhookSubscription);

            var userDeletedWebhookSubscription = NewWebhookSubscription(null, AppWebhookDefinitionNames.Users.Deleted);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userDeletedWebhookSubscription);

            var defaultThemeChangedWebhookSubscription = NewWebhookSubscription(null, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(defaultThemeChangedWebhookSubscription);
        }

        [Fact]
        public async Task Should_Activate_Subscription_Async()
        {
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var testWebhookSubscription = NewWebhookSubscription(null, AppWebhookDefinitionNames.Test);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(testWebhookSubscription);

            var storedSubscription = await webhookSubscriptionManager.GetAsync(testWebhookSubscription.Id);
            storedSubscription.Id.ShouldBe(testWebhookSubscription.Id);
            storedSubscription.IsActive.ShouldBeTrue();

            await webhookSubscriptionManager.ActivateWebhookSubscriptionAsync(testWebhookSubscription.Id, false);

            storedSubscription = await webhookSubscriptionManager.GetAsync(testWebhookSubscription.Id);
            storedSubscription.Id.ShouldBe(testWebhookSubscription.Id);
            storedSubscription.IsActive.ShouldBeFalse();

            await webhookSubscriptionManager.ActivateWebhookSubscriptionAsync(testWebhookSubscription.Id, true);

            storedSubscription = await webhookSubscriptionManager.GetAsync(testWebhookSubscription.Id);
            storedSubscription.Id.ShouldBe(testWebhookSubscription.Id);
            storedSubscription.IsActive.ShouldBeTrue();
        }
        
        [Fact]
        public async Task Should_Not_Get_Another_Tenants_Subscriptions()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true");

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            //should get tenant's own data
            await WithUnitOfWorkAsync(tenantId, async () =>
            {
                var storedSubscription = await webhookSubscriptionManager.GetAsync(newSubscription.Id);
                storedSubscription.ShouldNotBeNull();
                CompareSubscriptions(storedSubscription, newSubscription);

                var allSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsAsync(tenantId);
                allSubscriptions.Count.ShouldBe(1);
                CompareSubscriptions(storedSubscription, allSubscriptions.Single());
            });
            
            //should not get another tenant's data
            await WithUnitOfWorkAsync(null, async () =>
            {
                var allSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsAsync(null);
                allSubscriptions.Count.ShouldBe(0);
            });
        }

        [Fact]
        public void Should_Delete_Subscription_Sync()
        {
            var tenantId = AsyncHelper.RunSync(()=> CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true"));

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var userCreatedWebhookSubscription = NewWebhookSubscription("1", tenantId, AppWebhookDefinitionNames.Users.Created);
             webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebhookSubscription);
            
            var allSubscriptions = webhookSubscriptionManager.GetAllSubscriptions(tenantId);

            allSubscriptions.Count.ShouldBe(1);

            webhookSubscriptionManager.DeleteSubscription(userCreatedWebhookSubscription.Id);
            webhookSubscriptionManager.GetAllSubscriptions(tenantId).Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Delete_Subscription_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true");

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var userCreatedWebhookSubscription = NewWebhookSubscription("1", tenantId, AppWebhookDefinitionNames.Users.Created);
            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebhookSubscription);
            
            var allSubscriptions = await webhookSubscriptionManager.GetAllSubscriptionsAsync(tenantId);

            allSubscriptions.Count.ShouldBe(1);

            await webhookSubscriptionManager.DeleteSubscriptionAsync(userCreatedWebhookSubscription.Id);
            (await webhookSubscriptionManager.GetAllSubscriptionsAsync(tenantId)).Count.ShouldBe(0);
        }

        #endregion

        #region Sync

        [Fact]
        public void Should_Insert_And_Get_Subscription_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true"));

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            WithUnitOfWork(tenantId, () =>
            {
                var storedSubscription = webhookSubscriptionManager.Get(newSubscription.Id);
                storedSubscription.ShouldNotBeNull();
                CompareSubscriptions(storedSubscription, newSubscription);
            });
        }

        [Fact]
        public void Should_Update_Subscription_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true"));

            AbpSession.TenantId = tenantId;
            
            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            WebhookSubscription storedSubscription = null;
            WithUnitOfWork(tenantId, () =>
            {
                storedSubscription = webhookSubscriptionManager.Get(newSubscription.Id);
                storedSubscription.ShouldNotBeNull();
                CompareSubscriptions(storedSubscription, newSubscription);
            });

            //update
            var newWebhookUri = "www.mynewwebhook.com";
            storedSubscription.WebhookUri = newWebhookUri;

            storedSubscription.Webhooks.Add(AppWebhookDefinitionNames.Test);

            var newHeader = new KeyValuePair<string, string>("NewKey", "NewValue");
            storedSubscription.Headers.Add(newHeader);

            webhookSubscriptionManager.AddOrUpdateSubscription(storedSubscription);

            WithUnitOfWork(tenantId, () =>
            {
                var updatedSubscription = webhookSubscriptionManager.Get(newSubscription.Id);
                updatedSubscription.ShouldNotBeNull();
                updatedSubscription.Secret.ShouldNotBeNullOrEmpty();
                updatedSubscription.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                updatedSubscription.WebhookUri.ShouldBe(newWebhookUri);

                updatedSubscription.Webhooks.ShouldContain(AppWebhookDefinitionNames.Test);
                updatedSubscription.Webhooks.ShouldContain(AppWebhookDefinitionNames.Users.Created);

                updatedSubscription.TenantId.ShouldBe(tenantId);

                updatedSubscription.Headers.ShouldContain(newSubscription.Headers.First());
                updatedSubscription.Headers.ShouldContain(newHeader);
            });
        }

        [Fact]
        public void Should_Not_Update_Subscription_Secret_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true"));

            AbpSession.TenantId = tenantId;
            
            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            WebhookSubscription storedSubscription = null;
            WithUnitOfWork(tenantId, () =>
            {
                storedSubscription = webhookSubscriptionManager.Get(newSubscription.Id);
                storedSubscription.ShouldNotBeNull();
                CompareSubscriptions(storedSubscription, newSubscription);
            });

            //update
            string currentSecret = storedSubscription.Secret;
            storedSubscription.Secret = "TestSecret";

            webhookSubscriptionManager.AddOrUpdateSubscription(storedSubscription);

            WithUnitOfWork(tenantId, () =>
            {
                var updatedSubscription = webhookSubscriptionManager.Get(newSubscription.Id);
                updatedSubscription.ShouldNotBeNull();
                updatedSubscription.Secret.ShouldNotBeNullOrEmpty();
                updatedSubscription.Secret.ShouldBe(currentSecret);
            });
        }

        [Fact]
        public void Should_Insert_Throw_Exception_If_Features_Are_Not_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync());

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);//needs AppFeatures.WebhookFeature

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });
        }

        [Fact]
        public void Should_Update_Throw_Exception_If_Features_Are_Not_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync());//needs AppFeatures.WebhookFeature feature

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
            newSubscription.Id = Guid.NewGuid();

            var webhookStoreSubstitute = RegisterFake<IWebhookSubscriptionsStore>();
            webhookStoreSubstitute.Get(Arg.Any<Guid>()).Returns(callback =>
                new WebhookSubscriptionInfo()
                {
                    Id = (Guid)callback.Args()[0],
                    TenantId = newSubscription.TenantId
                }
            );
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });

            //check error reason
            webhookStoreSubstitute.ClearReceivedCalls();
            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.WebhookFeature, "true"));

            Predicate<WebhookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.WebhookUri.ShouldBe(newSubscription.WebhookUri);
                w.Webhooks.ShouldBe(newSubscription.Webhooks.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };
            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            webhookStoreSubstitute.DidNotReceive().Insert(Arg.Any<WebhookSubscriptionInfo>());
            webhookStoreSubstitute.Received().Update(Arg.Is<WebhookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public void Should_Add_Or_Update_Subscription_Sync_Method_Insert_If_Id_Is_Default_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true"));

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Deleted);
            newSubscription.Id = default;

            var webhookStoreSubstitute = RegisterFake<IWebhookSubscriptionsStore>();
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            Predicate<WebhookSubscriptionInfo> predicate = w =>
            {
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookUri.ShouldBe(newSubscription.WebhookUri);
                w.Webhooks.ShouldBe(newSubscription.Webhooks.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            webhookStoreSubstitute.DidNotReceive().Update(Arg.Any<WebhookSubscriptionInfo>());
            webhookStoreSubstitute.Received().Insert(Arg.Is<WebhookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public void Should_Add_Or_Update_Subscription_Sync_Method_Update_If_Id_Is_Not_Null_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true"));

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Deleted);
            newSubscription.Id = Guid.NewGuid();

            var webhookStoreSubstitute = RegisterFake<IWebhookSubscriptionsStore>();
            webhookStoreSubstitute.Get(Arg.Any<Guid>()).Returns(callback =>
                new WebhookSubscriptionInfo()
                {
                    Id = (Guid)callback.Args()[0],
                    TenantId = newSubscription.TenantId
                }
            );
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            Predicate<WebhookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.WebhookUri.ShouldBe(newSubscription.WebhookUri);
                w.Webhooks.ShouldBe(newSubscription.Webhooks.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            webhookStoreSubstitute.DidNotReceive().Insert(Arg.Any<WebhookSubscriptionInfo>());
            webhookStoreSubstitute.Received().Update(Arg.Is<WebhookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public void Should_Add_Or_Update_Subscription_Sync_Method_Throw_Exception_Subscription_If_Features_Are_Not_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync());

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Test);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            newSubscription.Webhooks.Add(AppWebhookDefinitionNames.Users.Deleted);

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });
        }

        [Fact]
        public void Should_Get_All_Subscription_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true"));

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var userCreatedWebhookSubscription = NewWebhookSubscription("1", tenantId, AppWebhookDefinitionNames.Users.Created);
            webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebhookSubscription);

            var userCreatedWebhookSubscription2 = NewWebhookSubscription("2", tenantId, AppWebhookDefinitionNames.Users.Created);
            webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebhookSubscription2);

            var testWebhookSubscription = NewWebhookSubscription("test", tenantId, AppWebhookDefinitionNames.Test);
            webhookSubscriptionManager.AddOrUpdateSubscription(testWebhookSubscription);

            var allSubscriptions = webhookSubscriptionManager.GetAllSubscriptions(tenantId);

            allSubscriptions.Count.ShouldBe(3);

            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebhookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebhookSubscription2.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(testWebhookSubscription.Id);

            var storedUserCreatedSubscription = allSubscriptions.Single(s => s.Id == userCreatedWebhookSubscription.Id);
            CompareSubscriptions(storedUserCreatedSubscription, userCreatedWebhookSubscription);

            var storedUserCreatedSubscription2 = allSubscriptions.Single(s => s.Id == userCreatedWebhookSubscription2.Id);
            CompareSubscriptions(storedUserCreatedSubscription2, userCreatedWebhookSubscription2);

            var storedTestSubscription = allSubscriptions.Single(s => s.Id == testWebhookSubscription.Id);
            CompareSubscriptions(storedTestSubscription, testWebhookSubscription);
        }

        [Fact]
        public void Should_Get_All_Subscriptions_If_Features_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebhookFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            }));

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var userCreatedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
            webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebhookSubscription);

            var userCreatedAndThemeChangedWebhookSubscription = NewWebhookSubscription(tenantId,
                AppWebhookDefinitionNames.Users.Created,
                AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedAndThemeChangedWebhookSubscription);

            var defaultThemeChangedSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            webhookSubscriptionManager.AddOrUpdateSubscription(defaultThemeChangedSubscription);

            var userCreatedSubscriptions = webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebhookDefinitionNames.Users.Created);

            userCreatedSubscriptions.Count.ShouldBe(2);
            userCreatedSubscriptions.Select(s => s.Id).ShouldNotContain(defaultThemeChangedSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebhookSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebhookSubscription.Id);

            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebhookSubscription.Id), userCreatedAndThemeChangedWebhookSubscription);
            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedWebhookSubscription.Id), userCreatedWebhookSubscription);

            var defaultThemeChangedSubscriptions = webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(2);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebhookSubscription.Id);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(defaultThemeChangedSubscription.Id);

            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebhookSubscription.Id), userCreatedAndThemeChangedWebhookSubscription);
            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == defaultThemeChangedSubscription.Id), defaultThemeChangedSubscription);
        }

        [Fact]
        public void Should_Not_Get_Subscriptions_If_Features_Not_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebhookFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            }));

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var userCreatedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
            webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebhookSubscription);

            var userCreatedAndThemeChangedWebhookSubscription = NewWebhookSubscription(tenantId,
                AppWebhookDefinitionNames.Users.Created,
                AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedAndThemeChangedWebhookSubscription);

            var defaultThemeChangedSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            webhookSubscriptionManager.AddOrUpdateSubscription(defaultThemeChangedSubscription);

            var userCreatedSubscriptions = webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebhookDefinitionNames.Users.Created);

            userCreatedSubscriptions.Count.ShouldBe(2);
            userCreatedSubscriptions.Select(s => s.Id).ShouldNotContain(defaultThemeChangedSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebhookSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebhookSubscription.Id);

            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebhookSubscription.Id), userCreatedAndThemeChangedWebhookSubscription);
            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedWebhookSubscription.Id), userCreatedWebhookSubscription);

            var defaultThemeChangedSubscriptions = webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(2);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebhookSubscription.Id);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(defaultThemeChangedSubscription.Id);

            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebhookSubscription.Id), userCreatedAndThemeChangedWebhookSubscription);
            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == defaultThemeChangedSubscription.Id), defaultThemeChangedSubscription);

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.ThemeFeature, "false"));

            defaultThemeChangedSubscriptions = webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(0);

            userCreatedSubscriptions = webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebhookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
        }

        [Fact]
        public void Should_Get_Is_Subscribed_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebhookFeature, "true"},
                {AppFeatures.TestFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            }));

            AbpSession.TenantId = tenantId;
            
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            WithUnitOfWork(() =>
            {
                var userCreatedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);
                webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebhookSubscription);

                var userDeletedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Deleted);
                webhookSubscriptionManager.AddOrUpdateSubscription(userDeletedWebhookSubscription);

                var defaultThemeChangedWebhookSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
                webhookSubscriptionManager.AddOrUpdateSubscription(defaultThemeChangedWebhookSubscription);
            });

            WithUnitOfWork(() =>
            {
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Users.Created)).ShouldBeTrue();
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Users.Deleted)).ShouldBeTrue();
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeTrue();
            });

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.TestFeature, "false"));//Users.Deleted requires it but not require all 

            WithUnitOfWork(() =>
            {
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Users.Created)).ShouldBeTrue();
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Users.Deleted)).ShouldBeTrue();
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeTrue();
            });

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.ThemeFeature, "false"));//DefaultThemeChanged requires and it is require all 

            WithUnitOfWork(() =>
            {
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Users.Created)).ShouldBeTrue();
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Users.Deleted)).ShouldBeTrue();
                (webhookSubscriptionManager.IsSubscribed(tenantId, AppWebhookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeFalse();
            });
        }

        [Fact]
        public void Should_Allow_Host_To_Subscribe_Any_Webhooks_Sync()
        {
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            var userCreatedWebhookSubscription = NewWebhookSubscription(null, AppWebhookDefinitionNames.Users.Created);
            webhookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebhookSubscription);

            var userDeletedWebhookSubscription = NewWebhookSubscription(null, AppWebhookDefinitionNames.Users.Deleted);
            webhookSubscriptionManager.AddOrUpdateSubscription(userDeletedWebhookSubscription);

            var defaultThemeChangedWebhookSubscription = NewWebhookSubscription(null, AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            webhookSubscriptionManager.AddOrUpdateSubscription(defaultThemeChangedWebhookSubscription);
        }

        [Fact]
        public void Should_Activate_Subscription_Sync()
        {
            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var testWebhookSubscription = NewWebhookSubscription(null, AppWebhookDefinitionNames.Test);
            webhookSubscriptionManager.AddOrUpdateSubscription(testWebhookSubscription);

            var storedSubscription = webhookSubscriptionManager.Get(testWebhookSubscription.Id);
            storedSubscription.Id.ShouldBe(testWebhookSubscription.Id);
            storedSubscription.IsActive.ShouldBeTrue();

            webhookSubscriptionManager.ActivateWebhookSubscription(testWebhookSubscription.Id, false);

            storedSubscription = webhookSubscriptionManager.Get(testWebhookSubscription.Id);
            storedSubscription.Id.ShouldBe(testWebhookSubscription.Id);
            storedSubscription.IsActive.ShouldBeFalse();

            webhookSubscriptionManager.ActivateWebhookSubscription(testWebhookSubscription.Id, true);

            storedSubscription = webhookSubscriptionManager.Get(testWebhookSubscription.Id);
            storedSubscription.Id.ShouldBe(testWebhookSubscription.Id);
            storedSubscription.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Should_Not_Get_Another_Tenants_Subscriptions_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebhookFeature, "true"));

            var newSubscription = NewWebhookSubscription(tenantId, AppWebhookDefinitionNames.Users.Created);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            webhookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            //should get tenant's own data
            WithUnitOfWork(tenantId, () =>
            {
                var storedSubscription = webhookSubscriptionManager.Get(newSubscription.Id);
                storedSubscription.ShouldNotBeNull();
                CompareSubscriptions(storedSubscription, newSubscription);

                var allSubscriptions = webhookSubscriptionManager.GetAllSubscriptions(tenantId);
                allSubscriptions.Count.ShouldBe(1);
                CompareSubscriptions(storedSubscription, allSubscriptions.Single());
            });

            //should not get another tenant's data
            WithUnitOfWork(null, () =>
            {
                var allSubscriptions = webhookSubscriptionManager.GetAllSubscriptions(null);
                allSubscriptions.Count.ShouldBe(0);
            });
        }

        #endregion
    }
}
