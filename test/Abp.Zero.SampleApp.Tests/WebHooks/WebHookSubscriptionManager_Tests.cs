using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Json;
using Abp.Threading;
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookSubscriptionManager_Tests : WebHookTestBase
    {
        private WebHookSubscription NewWebHookSubscription(int? tenantId, params string[] webHookDefinitions)
        {
            return new WebHookSubscription
            {
                TenantId = tenantId,
                WebHookDefinitions = webHookDefinitions.ToList(),
                WebHookUri = "www.mywebhook.com",
                Headers = new Dictionary<string, string>
                {
                    {"Key", "Value"}
                }
            };
        }

        private WebHookSubscription NewWebHookSubscription(string seed, int? tenantId, params string[] webHookDefinitions)
        {
            return new WebHookSubscription
            {
                TenantId = tenantId,
                WebHookDefinitions = webHookDefinitions.ToList(),
                WebHookUri = "www.mywebhook.com/" + seed,
                Headers = new Dictionary<string, string>
                {
                    {"Key", "Value_" + seed}
                }
            };
        }

        private void CompareSubscriptions(WebHookSubscription subscription1, WebHookSubscription subscription2)
        {
            subscription1.WebHookDefinitions.ShouldBe(subscription2.WebHookDefinitions);
            subscription1.TenantId.ShouldBe(subscription2.TenantId);
            subscription1.Secret.ShouldNotBeNullOrEmpty();
            subscription1.Secret.ShouldStartWith("whs_");
            subscription1.WebHookUri.ShouldBe(subscription2.WebHookUri);
            subscription1.WebHookDefinitions.ShouldBe(subscription2.WebHookDefinitions);
            subscription1.Headers.ShouldBe(subscription2.Headers);
        }

        #region Async

        [Fact]
        public async Task Should_Insert_And_Get_Subscription_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true");

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            await WithUnitOfWorkAsync(tenantId, async () =>
             {
                 var storedSubscription = webHookSubscriptionManager.Get(newSubscription.Id);
                 storedSubscription.ShouldNotBeNull();
                 CompareSubscriptions(storedSubscription, newSubscription);
             });
        }

        [Fact]
        public async Task Should_Update_Subscription_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true");

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            WebHookSubscription subscription = null;
            await WithUnitOfWorkAsync(tenantId, async () =>
            {
                subscription = await webHookSubscriptionManager.GetAsync(newSubscription.Id);
                subscription.ShouldNotBeNull();
                CompareSubscriptions(subscription, newSubscription);
            });
            
            var newWebHookUri = "www.mynewwebhook.com";
            subscription.WebHookUri = newWebHookUri;

            subscription.WebHookDefinitions.Add(AppWebHookDefinitionNames.Test);

            var newHeader = new KeyValuePair<string, string>("NewKey", "NewValue");
            subscription.Headers.Add(newHeader);

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

            await WithUnitOfWorkAsync(tenantId, async () =>
             {
                 var updatedSubscription = await webHookSubscriptionManager.GetAsync(newSubscription.Id);
                 updatedSubscription.ShouldNotBeNull();

                 updatedSubscription.WebHookUri.ShouldBe(newWebHookUri);

                 updatedSubscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
                 updatedSubscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

                 updatedSubscription.TenantId.ShouldBe(tenantId);

                 updatedSubscription.Headers.ShouldContain(newSubscription.Headers.First());
                 updatedSubscription.Headers.ShouldContain(newHeader);
             });
        }

        [Fact]
        public async Task Should_Insert_Throw_Exception_If_Features_Are_Not_Granted_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync();//needs AppFeatures.WebHookFeature feature

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });

            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.WebHookFeature, "true");
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
        }

        [Fact]
        public async Task Should_Update_Throw_Exception_If_Features_Are_Not_Granted_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync();//needs AppFeatures.WebHookFeature feature

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            newSubscription.Id = Guid.NewGuid();

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });

            //check error reason
            webHookStoreSubstitute.ClearReceivedCalls();
            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.WebHookFeature, "true");

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            await webHookStoreSubstitute.DidNotReceive().InsertAsync(Arg.Any<WebHookSubscriptionInfo>());
            await webHookStoreSubstitute.Received().UpdateAsync(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Add_Or_Update_Subscription_Async_Method_Insert_If_Id_Is_Default_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync();

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Test);
            newSubscription.Id = default;

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith("whs_");
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            await webHookStoreSubstitute.DidNotReceive().UpdateAsync(Arg.Any<WebHookSubscriptionInfo>());
            await webHookStoreSubstitute.Received().InsertAsync(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Add_Or_Update_Subscription_Async_Method_Update_If_Id_Is_Not_Default_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true");

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            newSubscription.Id = Guid.NewGuid();

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            await webHookStoreSubstitute.DidNotReceive().InsertAsync(Arg.Any<WebHookSubscriptionInfo>());
            await webHookStoreSubstitute.Received().UpdateAsync(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Get_All_Subscription_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true");

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var userCreatedWebHookSubscription = NewWebHookSubscription("1", tenantId, AppWebHookDefinitionNames.Users.Created);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription);

            var userCreatedWebHookSubscription2 = NewWebHookSubscription("2", tenantId, AppWebHookDefinitionNames.Users.Created);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription2);

            var testWebHookSubscription = NewWebHookSubscription("test", tenantId, AppWebHookDefinitionNames.Test);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(testWebHookSubscription);

            var allSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsAsync(tenantId);

            allSubscriptions.Count.ShouldBe(3);

            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription2.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(testWebHookSubscription.Id);

            var storedUserCreatedSubscription = allSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription.Id);
            CompareSubscriptions(storedUserCreatedSubscription, userCreatedWebHookSubscription);

            var storedUserCreatedSubscription2 = allSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription2.Id);
            CompareSubscriptions(storedUserCreatedSubscription2, userCreatedWebHookSubscription2);

            var storedTestSubscription = allSubscriptions.Single(s => s.Id == testWebHookSubscription.Id);
            CompareSubscriptions(storedTestSubscription, testWebHookSubscription);
        }

        [Fact]
        public async Task Should_Get_All_Subscriptions_If_Features_Granted_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebHookFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            });

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var userCreatedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription);

            var userCreatedAndThemeChangedWebHookSubscription = NewWebHookSubscription(tenantId,
                AppWebHookDefinitionNames.Users.Created,
                AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedAndThemeChangedWebHookSubscription);

            var defaultThemeChangedSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(defaultThemeChangedSubscription);

            var userCreatedSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebHookDefinitionNames.Users.Created);

            userCreatedSubscriptions.Count.ShouldBe(2);
            userCreatedSubscriptions.Select(s => s.Id).ShouldNotContain(defaultThemeChangedSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebHookSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription.Id);

            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebHookSubscription.Id), userCreatedAndThemeChangedWebHookSubscription);
            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription.Id), userCreatedWebHookSubscription);

            var defaultThemeChangedSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(2);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebHookSubscription.Id);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(defaultThemeChangedSubscription.Id);

            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebHookSubscription.Id), userCreatedAndThemeChangedWebHookSubscription);
            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == defaultThemeChangedSubscription.Id), defaultThemeChangedSubscription);
        }

        [Fact]
        public async Task Should_Not_Get_Subscriptions_If_Features_Not_Granted_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebHookFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            });

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var userCreatedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription);

            var userCreatedAndThemeChangedWebHookSubscription = NewWebHookSubscription(tenantId,
                AppWebHookDefinitionNames.Users.Created,
                AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedAndThemeChangedWebHookSubscription);

            var defaultThemeChangedSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(defaultThemeChangedSubscription);

            var userCreatedSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebHookDefinitionNames.Users.Created);

            userCreatedSubscriptions.Count.ShouldBe(2);
            userCreatedSubscriptions.Select(s => s.Id).ShouldNotContain(defaultThemeChangedSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebHookSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription.Id);

            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebHookSubscription.Id), userCreatedAndThemeChangedWebHookSubscription);
            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription.Id), userCreatedWebHookSubscription);

            var defaultThemeChangedSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(2);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebHookSubscription.Id);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(defaultThemeChangedSubscription.Id);

            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebHookSubscription.Id), userCreatedAndThemeChangedWebHookSubscription);
            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == defaultThemeChangedSubscription.Id), defaultThemeChangedSubscription);

            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.ThemeFeature, "false");

            defaultThemeChangedSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(0);

            userCreatedSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, AppWebHookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_Get_Is_Subscribed_Async()
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebHookFeature, "true"},
                {AppFeatures.TestFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            });

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var userCreatedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription);

            var userDeletedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Deleted);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userDeletedWebHookSubscription);

            var defaultThemeChangedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(defaultThemeChangedWebHookSubscription);

            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Users.Created)).ShouldBeTrue();
            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Users.Deleted)).ShouldBeTrue();
            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeTrue();

            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.TestFeature, "false");//Users.Deleted requires it but not require all 

            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Users.Created)).ShouldBeTrue();
            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Users.Deleted)).ShouldBeTrue();
            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeTrue();

            await AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.ThemeFeature, "false");//DefaultThemeChanged requires and it is require all 

            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Users.Created)).ShouldBeTrue();
            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Users.Deleted)).ShouldBeTrue();
            (await webHookSubscriptionManager.IsSubscribedAsync(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeFalse();
        }

        #endregion

        #region Sync

        [Fact]
        public void Should_Insert_And_Get_Subscription_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true"));

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            WithUnitOfWork(tenantId, () =>
            {
                var storedSubscription = webHookSubscriptionManager.Get(newSubscription.Id);
                storedSubscription.ShouldNotBeNull();
                CompareSubscriptions(storedSubscription, newSubscription);
            });
        }

        [Fact]
        public void Should_Update_Subscription_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true"));

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            WebHookSubscription storedSubscription = null;
            WithUnitOfWork(tenantId, () =>
            {
                storedSubscription = webHookSubscriptionManager.Get(newSubscription.Id);
                storedSubscription.ShouldNotBeNull();
                CompareSubscriptions(storedSubscription, newSubscription);
            });

            //update
            var newWebHookUri = "www.mynewwebhook.com";
            storedSubscription.WebHookUri = newWebHookUri;

            storedSubscription.WebHookDefinitions.Add(AppWebHookDefinitionNames.Test);

            var newHeader = new KeyValuePair<string, string>("NewKey", "NewValue");
            storedSubscription.Headers.Add(newHeader);

            webHookSubscriptionManager.AddOrUpdateSubscription(storedSubscription);

            WithUnitOfWork(tenantId, () =>
            {
                var updatedSubscription = webHookSubscriptionManager.Get(newSubscription.Id);
                updatedSubscription.ShouldNotBeNull();
                updatedSubscription.Secret.ShouldNotBeNullOrEmpty();
                updatedSubscription.Secret.ShouldStartWith("whs_");
                updatedSubscription.WebHookUri.ShouldBe(newWebHookUri);

                updatedSubscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
                updatedSubscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

                updatedSubscription.TenantId.ShouldBe(tenantId);

                updatedSubscription.Headers.ShouldContain(newSubscription.Headers.First());
                updatedSubscription.Headers.ShouldContain(newHeader);
            });
        }

        [Fact]
        public void Should_Insert_Throw_Exception_If_Features_Are_Not_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync());

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);//needs AppFeatures.WebHookFeature

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });
        }

        [Fact]
        public void Should_Update_Throw_Exception_If_Features_Are_Not_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync());//needs AppFeatures.WebHookFeature feature

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            newSubscription.Id = Guid.NewGuid();

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });

            //check error reason
            webHookStoreSubstitute.ClearReceivedCalls();
            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.WebHookFeature, "true"));

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            webHookStoreSubstitute.DidNotReceive().Insert(Arg.Any<WebHookSubscriptionInfo>());
            webHookStoreSubstitute.Received().Update(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public void Should_Add_Or_Update_Subscription_Sync_Method_Insert_If_Id_Is_Default_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true"));

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Deleted);
            newSubscription.Id = default;

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith("whs_");
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            webHookStoreSubstitute.DidNotReceive().Update(Arg.Any<WebHookSubscriptionInfo>());
            webHookStoreSubstitute.Received().Insert(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public void Should_Add_Or_Update_Subscription_Sync_Method_Update_If_Id_Is_Not_Null_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true"));

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Deleted);
            newSubscription.Id = Guid.NewGuid();

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            webHookStoreSubstitute.DidNotReceive().Insert(Arg.Any<WebHookSubscriptionInfo>());
            webHookStoreSubstitute.Received().Update(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public void Should_Add_Or_Update_Subscription_Sync_Method_Throw_Exception_Subscription_If_Features_Are_Not_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync());

            var newSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Test);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            newSubscription.WebHookDefinitions.Add(AppWebHookDefinitionNames.Users.Deleted);

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });
        }

        [Fact]
        public void Should_Get_All_Subscription_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(AppFeatures.WebHookFeature, "true"));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var userCreatedWebHookSubscription = NewWebHookSubscription("1", tenantId, AppWebHookDefinitionNames.Users.Created);
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription);

            var userCreatedWebHookSubscription2 = NewWebHookSubscription("2", tenantId, AppWebHookDefinitionNames.Users.Created);
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription2);

            var testWebHookSubscription = NewWebHookSubscription("test", tenantId, AppWebHookDefinitionNames.Test);
            webHookSubscriptionManager.AddOrUpdateSubscription(testWebHookSubscription);

            var allSubscriptions = webHookSubscriptionManager.GetAllSubscriptions(tenantId);

            allSubscriptions.Count.ShouldBe(3);

            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription2.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(testWebHookSubscription.Id);

            var storedUserCreatedSubscription = allSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription.Id);
            CompareSubscriptions(storedUserCreatedSubscription, userCreatedWebHookSubscription);

            var storedUserCreatedSubscription2 = allSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription2.Id);
            CompareSubscriptions(storedUserCreatedSubscription2, userCreatedWebHookSubscription2);

            var storedTestSubscription = allSubscriptions.Single(s => s.Id == testWebHookSubscription.Id);
            CompareSubscriptions(storedTestSubscription, testWebHookSubscription);
        }

        [Fact]
        public void Should_Get_All_Subscriptions_If_Features_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebHookFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            }));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var userCreatedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription);

            var userCreatedAndThemeChangedWebHookSubscription = NewWebHookSubscription(tenantId,
                AppWebHookDefinitionNames.Users.Created,
                AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedAndThemeChangedWebHookSubscription);

            var defaultThemeChangedSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            webHookSubscriptionManager.AddOrUpdateSubscription(defaultThemeChangedSubscription);

            var userCreatedSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebHookDefinitionNames.Users.Created);

            userCreatedSubscriptions.Count.ShouldBe(2);
            userCreatedSubscriptions.Select(s => s.Id).ShouldNotContain(defaultThemeChangedSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebHookSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription.Id);

            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebHookSubscription.Id), userCreatedAndThemeChangedWebHookSubscription);
            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription.Id), userCreatedWebHookSubscription);

            var defaultThemeChangedSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(2);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebHookSubscription.Id);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(defaultThemeChangedSubscription.Id);

            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebHookSubscription.Id), userCreatedAndThemeChangedWebHookSubscription);
            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == defaultThemeChangedSubscription.Id), defaultThemeChangedSubscription);
        }

        [Fact]
        public void Should_Not_Get_Subscriptions_If_Features_Not_Granted_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebHookFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            }));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var userCreatedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription);

            var userCreatedAndThemeChangedWebHookSubscription = NewWebHookSubscription(tenantId,
                AppWebHookDefinitionNames.Users.Created,
                AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedAndThemeChangedWebHookSubscription);

            var defaultThemeChangedSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            webHookSubscriptionManager.AddOrUpdateSubscription(defaultThemeChangedSubscription);

            var userCreatedSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebHookDefinitionNames.Users.Created);

            userCreatedSubscriptions.Count.ShouldBe(2);
            userCreatedSubscriptions.Select(s => s.Id).ShouldNotContain(defaultThemeChangedSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebHookSubscription.Id);
            userCreatedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription.Id);

            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebHookSubscription.Id), userCreatedAndThemeChangedWebHookSubscription);
            CompareSubscriptions(userCreatedSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription.Id), userCreatedWebHookSubscription);

            var defaultThemeChangedSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(2);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(userCreatedAndThemeChangedWebHookSubscription.Id);
            defaultThemeChangedSubscriptions.Select(s => s.Id).ShouldContain(defaultThemeChangedSubscription.Id);

            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == userCreatedAndThemeChangedWebHookSubscription.Id), userCreatedAndThemeChangedWebHookSubscription);
            CompareSubscriptions(defaultThemeChangedSubscriptions.Single(s => s.Id == defaultThemeChangedSubscription.Id), defaultThemeChangedSubscription);

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.ThemeFeature, "false"));

            defaultThemeChangedSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            defaultThemeChangedSubscriptions.Count.ShouldBe(0);

            userCreatedSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, AppWebHookDefinitionNames.Users.Created);
            userCreatedSubscriptions.Count.ShouldBe(2);
        }

        [Fact]
        public void Should_Get_Is_Subscribed_Sync()
        {
            var tenantId = AsyncHelper.RunSync(() => CreateAndGetTenantIdWithFeaturesAsync(new Dictionary<string, string>()
            {
                {AppFeatures.WebHookFeature, "true"},
                {AppFeatures.TestFeature, "true"},
                {AppFeatures.ThemeFeature, "true"}
            }));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var userCreatedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Created);
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription);

            var userDeletedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Users.Deleted);
            webHookSubscriptionManager.AddOrUpdateSubscription(userDeletedWebHookSubscription);

            var defaultThemeChangedWebHookSubscription = NewWebHookSubscription(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            webHookSubscriptionManager.AddOrUpdateSubscription(defaultThemeChangedWebHookSubscription);

            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Users.Created)).ShouldBeTrue();
            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Users.Deleted)).ShouldBeTrue();
            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeTrue();

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.TestFeature, "false"));//Users.Deleted requires it but not require all 

            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Users.Created)).ShouldBeTrue();
            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Users.Deleted)).ShouldBeTrue();
            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeTrue();

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(tenantId, AppFeatures.ThemeFeature, "false"));//DefaultThemeChanged requires and it is require all 

            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Users.Created)).ShouldBeTrue();
            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Users.Deleted)).ShouldBeTrue();
            (webHookSubscriptionManager.IsSubscribed(tenantId, AppWebHookDefinitionNames.Theme.DefaultThemeChanged)).ShouldBeFalse();
        }
        #endregion
    }
}
