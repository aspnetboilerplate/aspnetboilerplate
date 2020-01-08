using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.MultiTenancy;
using Abp.Threading;
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using JetBrains.Annotations;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookSubscriptionManager_Tests : WebHookTestBase
    {
        #region Async

        [Fact]
        public async Task Should_Get_Subscription_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            var subscription = webHookSubscriptionManager.Get(newSubscription.Id);
            subscription.ShouldNotBeNull();
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscription.TenantId.ShouldBe(AbpSession.TenantId);
            subscription.UserId.ShouldBe(newSubscription.UserId);
            subscription.Secret.ShouldBe(newSubscription.Secret);
            subscription.WebHookUri.ShouldBe(newSubscription.WebHookUri);
            subscription.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions);
            subscription.Headers.ShouldBe(newSubscription.Headers);
        }

        [Fact]
        public async Task Should_Insert_Subscription_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            var subscription = webHookSubscriptionManager.Get(newSubscription.Id);

            subscription.ShouldNotBeNull();
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscription.TenantId.ShouldBe(AbpSession.TenantId);
            subscription.UserId.ShouldBe(newSubscription.UserId);
            subscription.Secret.ShouldBe(newSubscription.Secret);
            subscription.WebHookUri.ShouldBe(newSubscription.WebHookUri);
            subscription.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions);
            subscription.Headers.ShouldBe(newSubscription.Headers);
        }

        [Fact]
        public async Task Should_Update_Subscription_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            var subscription = webHookSubscriptionManager.Get(newSubscription.Id);
            subscription.ShouldNotBeNull();
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscription.TenantId.ShouldBe(AbpSession.TenantId);
            subscription.UserId.ShouldBe(newSubscription.UserId);
            subscription.Secret.ShouldBe(newSubscription.Secret);
            subscription.WebHookUri.ShouldBe(newSubscription.WebHookUri);
            subscription.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions);
            subscription.Headers.ShouldBe(newSubscription.Headers);

            //update
            string newSecret = "NewSecret";
            subscription.Secret = newSecret;

            var newWebHookUri = "www.mynewwebhook.com";
            subscription.WebHookUri = newWebHookUri;

            subscription.WebHookDefinitions.Add(AppWebHookDefinitionNames.Test);

            var newHeader = new KeyValuePair<string, string>("NewKey", "NewValue");
            subscription.Headers.Add(newHeader);

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);
            //

            var updatedSubscription = webHookSubscriptionManager.Get(newSubscription.Id);
            updatedSubscription.ShouldNotBeNull();
            updatedSubscription.Secret.ShouldBe(newSecret);
            updatedSubscription.WebHookUri.ShouldBe(newWebHookUri);

            updatedSubscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            updatedSubscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

            updatedSubscription.TenantId.ShouldBe(AbpSession.TenantId);
            updatedSubscription.UserId.ShouldBe(newSubscription.UserId);

            updatedSubscription.Headers.ShouldContain(newSubscription.Headers.First());
            updatedSubscription.Headers.ShouldContain(newHeader);
        }

        [Fact]
        public async Task Should_Insert_Throw_Exception_If_Permissions_Are_Not_Granted_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync();

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });
        }

        [Fact]
        public async Task Should_Insert_Throw_Exception_If_Features_Are_Not_Granted_Async()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;

            var user = await CreateNewUserWithWebhookPermissionsAsync();

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Chat.NewMessageReceived },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });

            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true");

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);//after we add feature to tenant it will not throw an exception
        }

        [Fact]
        public async Task Should_Update_Throw_Exception_If_Permissions_Are_Not_Granted_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync();

            var newSubscription = new WebHookSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });
        }

        [Fact]
        public async Task Should_Update_Throw_Exception_If_Features_Are_Not_Granted_Async()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;

            var user = await CreateNewUserWithWebhookPermissionsAsync();

            var newSubscription = new WebHookSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Chat.NewMessageReceived },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };
            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });

            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true");

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);//after we add feature to tenant it will not throw an exception

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.UserId.ShouldBe(newSubscription.UserId);
                w.Secret.ShouldBe(newSubscription.Secret);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            await webHookStoreSubstitute.DidNotReceive().InsertAsync(Arg.Any<WebHookSubscriptionInfo>());
            await webHookStoreSubstitute.Received().UpdateAsync(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Add_Or_Update_Subscription_Async_Method_Insert_If_Id_Is_Null_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.UserId.ShouldBe(newSubscription.UserId);
                w.Secret.ShouldBe(newSubscription.Secret);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            await webHookStoreSubstitute.DidNotReceive().UpdateAsync(Arg.Any<WebHookSubscriptionInfo>());
            await webHookStoreSubstitute.Received().InsertAsync(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Add_Or_Update_Subscription_Async_Method_Update_If_Id_Is_Not_Null_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.UserId.ShouldBe(newSubscription.UserId);
                w.Secret.ShouldBe(newSubscription.Secret);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            await webHookStoreSubstitute.DidNotReceive().InsertAsync(Arg.Any<WebHookSubscriptionInfo>());
            await webHookStoreSubstitute.Received().UpdateAsync(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Add_Or_Update_Subscription_Async_Method_Throw_Exception_Subscription_If_Permissions_Are_Not_Granted_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            newSubscription.WebHookDefinitions.Add(AppWebHookDefinitionNames.Tenant.Deleted);

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
             {
                 await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
             });
        }

        [Fact]
        public async Task Should_Get_All_Subscription_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            //Add 3 subscriptions
            var userCreatedWebHookSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription);

            var userCreatedWebHookSubscription2 = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret2",
                WebHookUri = "www.mywebhook.com/webhook",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value2"}
                }
            };
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription2);

            var testWebHookSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret_test",
                WebHookUri = "www.mywebhook.com/test",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Test },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Test"}
                }
            };
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(testWebHookSubscription);
            //

            var allSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsAsync(new UserIdentifier(user.TenantId, user.Id));

            allSubscriptions.Count.ShouldBe(3);

            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription2.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(testWebHookSubscription.Id);

            var userCreatedSubscription = allSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription.Id);
            userCreatedSubscription.TenantId.ShouldBe(userCreatedWebHookSubscription.TenantId);
            userCreatedSubscription.UserId.ShouldBe(userCreatedWebHookSubscription.UserId);
            userCreatedSubscription.Secret.ShouldBe(userCreatedWebHookSubscription.Secret);
            userCreatedSubscription.WebHookUri.ShouldBe(userCreatedWebHookSubscription.WebHookUri);
            userCreatedSubscription.WebHookDefinitions.ShouldBe(userCreatedWebHookSubscription.WebHookDefinitions);
            userCreatedSubscription.Headers.ShouldBe(userCreatedWebHookSubscription.Headers);

            var userCreatedSubscription2 = allSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription2.Id);
            userCreatedSubscription2.TenantId.ShouldBe(userCreatedWebHookSubscription2.TenantId);
            userCreatedSubscription2.UserId.ShouldBe(userCreatedWebHookSubscription2.UserId);
            userCreatedSubscription2.Secret.ShouldBe(userCreatedWebHookSubscription2.Secret);
            userCreatedSubscription2.WebHookUri.ShouldBe(userCreatedWebHookSubscription2.WebHookUri);
            userCreatedSubscription2.WebHookDefinitions.ShouldBe(userCreatedWebHookSubscription2.WebHookDefinitions);
            userCreatedSubscription2.Headers.ShouldBe(userCreatedWebHookSubscription2.Headers);

            var testSubscription = allSubscriptions.Single(s => s.Id == testWebHookSubscription.Id);
            testSubscription.TenantId.ShouldBe(testWebHookSubscription.TenantId);
            testSubscription.UserId.ShouldBe(testWebHookSubscription.UserId);
            testSubscription.Secret.ShouldBe(testWebHookSubscription.Secret);
            testSubscription.WebHookUri.ShouldBe(testWebHookSubscription.WebHookUri);
            testSubscription.WebHookDefinitions.ShouldBe(testWebHookSubscription.WebHookDefinitions);
            testSubscription.Headers.ShouldBe(testWebHookSubscription.Headers);
        }

        [Fact]
        public async Task Should_Get_All_Subscription_Permissions_Granted_By_User_Async()
        {
            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            //Add subscriptions
            var userCreatedWebHookSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription);

            var userCreatedWebHookSubscription2 = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret2",
                WebHookUri = "www.mywebhook.com/webhook",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Test },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value2"}
                }
            };
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription2);
            //

            await UserManager.ProhibitPermissionAsync(user, PermissionManager.GetPermission(AppPermissions.WebHook.UserCreated)); //remove permission from user 

            var allSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsPermissionGrantedAsync(new UserIdentifier(user.TenantId, user.Id), AppWebHookDefinitionNames.Users.Created);

            allSubscriptions.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Get_All_Subscription_Features_Granted_By_User_Async()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;

            var user = await CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated });

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            //Add subscriptions
            var userCreatedWebHookSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Chat.NewMessageReceived },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var userCreatedWebHookSubscription2 = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret2",
                WebHookUri = "www.mywebhook.com/webhook",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Chat.NewMessageReceived, AppWebHookDefinitionNames.Test },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value2"}
                }
            };

            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true");

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription);
            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(userCreatedWebHookSubscription2);

            var allSubscriptions = await webHookSubscriptionManager
                .GetAllSubscriptionsPermissionGrantedAsync(
                    new UserIdentifier(user.TenantId, user.Id),
                    AppWebHookDefinitionNames.Chat.NewMessageReceived
                );
            allSubscriptions.Count.ShouldBe(2);

            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "false");

            allSubscriptions = await webHookSubscriptionManager
               .GetAllSubscriptionsPermissionGrantedAsync(
                   new UserIdentifier(user.TenantId, user.Id),
                   AppWebHookDefinitionNames.Chat.NewMessageReceived
               );

            allSubscriptions.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Get_All_Subscription_Permissions_Granted_Async()
        {
            var subscriptionResult1 = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated);
            var subscriptionResult2 = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated);
            var subscriptionResult3 = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated);

            await UserManager.ProhibitPermissionAsync(subscriptionResult2.user, PermissionManager.GetPermission(AppPermissions.WebHook.UserCreated)); //remove permission from user2

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var allSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsPermissionGrantedAsync(AppWebHookDefinitionNames.Users.Created);

            allSubscriptions.Count.ShouldBe(2);
            allSubscriptions.Select(s => s.Id).ShouldContain(subscriptionResult1.webHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldNotContain(subscriptionResult2.webHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(subscriptionResult3.webHookSubscription.Id);

            var subscription1Result = allSubscriptions.Single(s => s.Id == subscriptionResult1.webHookSubscription.Id);
            subscription1Result.TenantId.ShouldBe(subscriptionResult1.webHookSubscription.TenantId);
            subscription1Result.UserId.ShouldBe(subscriptionResult1.user.Id);
            subscription1Result.Secret.ShouldBe(subscriptionResult1.webHookSubscription.Secret);
            subscription1Result.WebHookUri.ShouldBe(subscriptionResult1.webHookSubscription.WebHookUri);
            subscription1Result.WebHookDefinitions.ShouldBe(subscriptionResult1.webHookSubscription.WebHookDefinitions);
            subscription1Result.Headers.ShouldBe(subscriptionResult1.webHookSubscription.Headers);

            var subscription3Result = allSubscriptions.Single(s => s.Id == subscriptionResult3.webHookSubscription.Id);
            subscription3Result.TenantId.ShouldBe(subscriptionResult3.webHookSubscription.TenantId);
            subscription3Result.UserId.ShouldBe(subscriptionResult3.user.Id);
            subscription3Result.Secret.ShouldBe(subscriptionResult3.webHookSubscription.Secret);
            subscription3Result.WebHookUri.ShouldBe(subscriptionResult3.webHookSubscription.WebHookUri);
            subscription3Result.WebHookDefinitions.ShouldBe(subscriptionResult3.webHookSubscription.WebHookDefinitions);
            subscription3Result.Headers.ShouldBe(subscriptionResult3.webHookSubscription.Headers);
        }

        [Fact]
        public async Task Should_Get_All_Subscription_Features_Granted_Async()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;
            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true");

            var subscriptionResult1 = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Chat.NewMessageReceived);
            var subscriptionResult2 = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Chat.NewMessageReceived);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var allSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsPermissionGrantedAsync(AppWebHookDefinitionNames.Chat.NewMessageReceived);

            allSubscriptions.Count.ShouldBe(2);
            allSubscriptions.Select(s => s.Id).ShouldContain(subscriptionResult1.webHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(subscriptionResult2.webHookSubscription.Id);

            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "false");

            allSubscriptions = await webHookSubscriptionManager.GetAllSubscriptionsPermissionGrantedAsync(AppWebHookDefinitionNames.Chat.NewMessageReceived);
            allSubscriptions.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Get_Is_Subscribed_Async()
        {
            var subscriptionResult = await CreateUserAndSubscribeToWebhookAsync(
                new List<string> { AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Test },
                new List<string> { AppPermissions.WebHook.UserCreated }
                );

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var userIdentifier = new UserIdentifier(subscriptionResult.user.TenantId, subscriptionResult.user.Id);

            (await webHookSubscriptionManager.IsSubscribedAsync(userIdentifier, AppWebHookDefinitionNames.Users.Created)).ShouldBeTrue();
            (await webHookSubscriptionManager.IsSubscribedAsync(userIdentifier, AppWebHookDefinitionNames.Test)).ShouldBeTrue();

            await UserManager.ProhibitPermissionAsync(subscriptionResult.user, PermissionManager.GetPermission(AppPermissions.WebHook.UserCreated)); //remove permission from user

            (await webHookSubscriptionManager.IsSubscribedAsync(userIdentifier, AppWebHookDefinitionNames.Users.Created)).ShouldBeFalse();
            (await webHookSubscriptionManager.IsSubscribedAsync(userIdentifier, AppWebHookDefinitionNames.Test)).ShouldBeTrue();
        }

        [Fact]
        public async Task Should_DeActivate_Subscription_Async()
        {
            var subscriptionResult = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            await webHookSubscriptionManager.DeactivateSubscriptionAsync(subscriptionResult.webHookSubscription.Id);

            var subscription = await webHookSubscriptionManager.GetAsync(subscriptionResult.webHookSubscription.Id);
            subscription.ShouldNotBeNull();
            subscription.IsActive.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Activate_Subscription_Async()
        {
            var subscriptionResult = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            await webHookSubscriptionManager.DeactivateSubscriptionAsync(subscriptionResult.webHookSubscription.Id);

            var subscription = await webHookSubscriptionManager.GetAsync(subscriptionResult.webHookSubscription.Id);
            subscription.ShouldNotBeNull();
            subscription.IsActive.ShouldBeFalse();

            await webHookSubscriptionManager.ActivateSubscriptionAsync(subscriptionResult.webHookSubscription.Id);

            var activatedSubscription = await webHookSubscriptionManager.GetAsync(subscriptionResult.webHookSubscription.Id);
            activatedSubscription.ShouldNotBeNull();
            activatedSubscription.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Add_WebHook_To_Subscription_Async()
        {
            var subscriptionResult = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var subscription = await webHookSubscriptionManager.GetAsync(subscriptionResult.webHookSubscription.Id);

            subscription.WebHookDefinitions.Count.ShouldBe(1);
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

            await webHookSubscriptionManager.AddWebHookAsync(subscription.Id, AppWebHookDefinitionNames.Test);
        }

        [Fact]
        public async Task Should_Add_WebHook_Async_Throw_Exception_If_Permissions_Are_Not_Granted()
        {
            var subscriptionResult = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Test);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var subscription = await webHookSubscriptionManager.GetAsync(subscriptionResult.webHookSubscription.Id);

            subscription.WebHookDefinitions.Count.ShouldBe(1);
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webHookSubscriptionManager.AddWebHookAsync(subscription.Id, AppWebHookDefinitionNames.Users.Created);
            });
        }

        [Fact]
        public async Task Should_Add_WebHook_Async_Throw_Exception_If_Features_Are_Not_Granted()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;

            var subscriptionResult = await CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Test);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var subscription = await webHookSubscriptionManager.GetAsync(subscriptionResult.webHookSubscription.Id);

            subscription.WebHookDefinitions.Count.ShouldBe(1);
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await webHookSubscriptionManager.AddWebHookAsync(subscription.Id, AppWebHookDefinitionNames.Chat.NewMessageReceived);
            });

            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true");

            await webHookSubscriptionManager.AddWebHookAsync(subscription.Id, AppWebHookDefinitionNames.Chat.NewMessageReceived);
        }

        #endregion

        #region Async

        [Fact]
        public void Should_Get_Subscription_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            var subscription = webHookSubscriptionManager.Get(newSubscription.Id);
            subscription.ShouldNotBeNull();
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscription.TenantId.ShouldBe(AbpSession.TenantId);
            subscription.UserId.ShouldBe(newSubscription.UserId);
            subscription.Secret.ShouldBe(newSubscription.Secret);
            subscription.WebHookUri.ShouldBe(newSubscription.WebHookUri);
            subscription.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions);
            subscription.Headers.ShouldBe(newSubscription.Headers);
        }

        [Fact]
        public void Should_Insert_Subscription_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            var subscription = webHookSubscriptionManager.Get(newSubscription.Id);

            subscription.ShouldNotBeNull();
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscription.TenantId.ShouldBe(AbpSession.TenantId);
            subscription.UserId.ShouldBe(newSubscription.UserId);
            subscription.Secret.ShouldBe(newSubscription.Secret);
            subscription.WebHookUri.ShouldBe(newSubscription.WebHookUri);
            subscription.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions);
            subscription.Headers.ShouldBe(newSubscription.Headers);
        }

        [Fact]
        public void Should_Update_Subscription_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            //insert
            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            var subscription = webHookSubscriptionManager.Get(newSubscription.Id);
            subscription.ShouldNotBeNull();
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
            subscription.TenantId.ShouldBe(AbpSession.TenantId);
            subscription.UserId.ShouldBe(newSubscription.UserId);
            subscription.Secret.ShouldBe(newSubscription.Secret);
            subscription.WebHookUri.ShouldBe(newSubscription.WebHookUri);
            subscription.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions);
            subscription.Headers.ShouldBe(newSubscription.Headers);

            //update
            string newSecret = "NewSecret";
            subscription.Secret = newSecret;

            var newWebHookUri = "www.mynewwebhook.com";
            subscription.WebHookUri = newWebHookUri;

            subscription.WebHookDefinitions.Add(AppWebHookDefinitionNames.Test);

            var newHeader = new KeyValuePair<string, string>("NewKey", "NewValue");
            subscription.Headers.Add(newHeader);

            webHookSubscriptionManager.AddOrUpdateSubscription(subscription);
            //

            var updatedSubscription = webHookSubscriptionManager.Get(newSubscription.Id);
            updatedSubscription.ShouldNotBeNull();
            updatedSubscription.Secret.ShouldBe(newSecret);
            updatedSubscription.WebHookUri.ShouldBe(newWebHookUri);

            updatedSubscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);
            updatedSubscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

            updatedSubscription.TenantId.ShouldBe(AbpSession.TenantId);
            updatedSubscription.UserId.ShouldBe(newSubscription.UserId);

            updatedSubscription.Headers.ShouldContain(newSubscription.Headers.First());
            updatedSubscription.Headers.ShouldContain(newHeader);
        }

        [Fact]
        public void Should_Insert_Throw_Exception_If_Permissions_Are_Not_Granted_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync());

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });
        }

        [Fact]
        public void Should_Insert_Throw_Exception_If_Features_Are_Not_Granted_Sync()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;

            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync());

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Chat.NewMessageReceived },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true"));

            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);//after we add feature to tenant it will not throw an exception
        }

        [Fact]
        public void Should_Update_Throw_Exception_If_Permissions_Are_Not_Granted_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync());

            var newSubscription = new WebHookSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });
        }

        [Fact]
        public void Should_Update_Throw_Exception_If_Features_Are_Not_Granted_Sync()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;

            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync());

            var newSubscription = new WebHookSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Chat.NewMessageReceived },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };
            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true"));

            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);//after we add feature to tenant it will not throw an exception

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.UserId.ShouldBe(newSubscription.UserId);
                w.Secret.ShouldBe(newSubscription.Secret);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            webHookStoreSubstitute.DidNotReceive().Insert(Arg.Any<WebHookSubscriptionInfo>());
            webHookStoreSubstitute.Received().Update(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public void Should_Add_Or_Update_Subscription_Sync_Method_Insert_If_Id_Is_Null_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.UserId.ShouldBe(newSubscription.UserId);
                w.Secret.ShouldBe(newSubscription.Secret);
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
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            var newSubscription = new WebHookSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();
            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            Predicate<WebHookSubscriptionInfo> predicate = w =>
            {
                w.Id.ShouldBe(newSubscription.Id);
                w.TenantId.ShouldBe(newSubscription.TenantId);
                w.UserId.ShouldBe(newSubscription.UserId);
                w.Secret.ShouldBe(newSubscription.Secret);
                w.WebHookUri.ShouldBe(newSubscription.WebHookUri);
                w.WebHookDefinitions.ShouldBe(newSubscription.WebHookDefinitions.ToJsonString());
                w.Headers.ShouldBe(newSubscription.Headers.ToJsonString());
                return true;
            };

            webHookStoreSubstitute.DidNotReceive().Insert(Arg.Any<WebHookSubscriptionInfo>());
            webHookStoreSubstitute.Received().Update(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
        }

        [Fact]
        public void Should_Add_Or_Update_Subscription_Sync_Method_Throw_Exception_Subscription_If_Permissions_Are_Not_Granted_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            var newSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);

            newSubscription.WebHookDefinitions.Add(AppWebHookDefinitionNames.Tenant.Deleted);

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddOrUpdateSubscription(newSubscription);
            });
        }

        [Fact]
        public void Should_Get_All_Subscription_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            //Add 3 subscriptions
            var userCreatedWebHookSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription);

            var userCreatedWebHookSubscription2 = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret2",
                WebHookUri = "www.mywebhook.com/webhook",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value2"}
                }
            };
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription2);

            var testWebHookSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret_test",
                WebHookUri = "www.mywebhook.com/test",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Test },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Test"}
                }
            };
            webHookSubscriptionManager.AddOrUpdateSubscription(testWebHookSubscription);
            //

            var allSubscriptions = webHookSubscriptionManager.GetAllSubscriptions(new UserIdentifier(user.TenantId, user.Id));

            allSubscriptions.Count.ShouldBe(3);

            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(userCreatedWebHookSubscription2.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(testWebHookSubscription.Id);

            var userCreatedSubscription = allSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription.Id);
            userCreatedSubscription.TenantId.ShouldBe(userCreatedWebHookSubscription.TenantId);
            userCreatedSubscription.UserId.ShouldBe(userCreatedWebHookSubscription.UserId);
            userCreatedSubscription.Secret.ShouldBe(userCreatedWebHookSubscription.Secret);
            userCreatedSubscription.WebHookUri.ShouldBe(userCreatedWebHookSubscription.WebHookUri);
            userCreatedSubscription.WebHookDefinitions.ShouldBe(userCreatedWebHookSubscription.WebHookDefinitions);
            userCreatedSubscription.Headers.ShouldBe(userCreatedWebHookSubscription.Headers);

            var userCreatedSubscription2 = allSubscriptions.Single(s => s.Id == userCreatedWebHookSubscription2.Id);
            userCreatedSubscription2.TenantId.ShouldBe(userCreatedWebHookSubscription2.TenantId);
            userCreatedSubscription2.UserId.ShouldBe(userCreatedWebHookSubscription2.UserId);
            userCreatedSubscription2.Secret.ShouldBe(userCreatedWebHookSubscription2.Secret);
            userCreatedSubscription2.WebHookUri.ShouldBe(userCreatedWebHookSubscription2.WebHookUri);
            userCreatedSubscription2.WebHookDefinitions.ShouldBe(userCreatedWebHookSubscription2.WebHookDefinitions);
            userCreatedSubscription2.Headers.ShouldBe(userCreatedWebHookSubscription2.Headers);

            var testSubscription = allSubscriptions.Single(s => s.Id == testWebHookSubscription.Id);
            testSubscription.TenantId.ShouldBe(testWebHookSubscription.TenantId);
            testSubscription.UserId.ShouldBe(testWebHookSubscription.UserId);
            testSubscription.Secret.ShouldBe(testWebHookSubscription.Secret);
            testSubscription.WebHookUri.ShouldBe(testWebHookSubscription.WebHookUri);
            testSubscription.WebHookDefinitions.ShouldBe(testWebHookSubscription.WebHookDefinitions);
            testSubscription.Headers.ShouldBe(testWebHookSubscription.Headers);
        }

        [Fact]
        public void Should_Get_All_Subscription_Permissions_Granted_By_User_Sync()
        {
            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            //Add subscriptions
            var userCreatedWebHookSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription);

            var userCreatedWebHookSubscription2 = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret2",
                WebHookUri = "www.mywebhook.com/webhook",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Test },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value2"}
                }
            };
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription2);
            //

            AsyncHelper.RunSync(() => UserManager.ProhibitPermissionAsync(user, PermissionManager.GetPermission(AppPermissions.WebHook.UserCreated))); //remove permission from user 

            var allSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsPermissionGranted(new UserIdentifier(user.TenantId, user.Id), AppWebHookDefinitionNames.Users.Created);

            allSubscriptions.Count.ShouldBe(0);
        }

        [Fact]
        public void Should_Get_All_Subscription_Features_Granted_By_User_Sync()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;

            var user = AsyncHelper.RunSync(() => CreateNewUserWithWebhookPermissionsAsync(new List<string> { AppPermissions.WebHook.UserCreated }));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            //Add subscriptions
            var userCreatedWebHookSubscription = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Chat.NewMessageReceived },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var userCreatedWebHookSubscription2 = new WebHookSubscription
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret2",
                WebHookUri = "www.mywebhook.com/webhook",
                WebHookDefinitions = new List<string> { AppWebHookDefinitionNames.Chat.NewMessageReceived, AppWebHookDefinitionNames.Test },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value2"}
                }
            };

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true"));

            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription);
            webHookSubscriptionManager.AddOrUpdateSubscription(userCreatedWebHookSubscription2);

            var allSubscriptions = webHookSubscriptionManager
                .GetAllSubscriptionsPermissionGranted(new UserIdentifier(user.TenantId, user.Id),
                    AppWebHookDefinitionNames.Chat.NewMessageReceived
                );
            allSubscriptions.Count.ShouldBe(2);

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "false"));

            allSubscriptions = webHookSubscriptionManager
               .GetAllSubscriptionsPermissionGranted(new UserIdentifier(user.TenantId, user.Id),
                   AppWebHookDefinitionNames.Chat.NewMessageReceived
               );

            allSubscriptions.Count.ShouldBe(0);
        }

        [Fact]
        public void Should_Get_All_Subscription_Permissions_Granted_Sync()
        {
            var subscriptionResult1 = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated));
            var subscriptionResult2 = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated));
            var subscriptionResult3 = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated));

            AsyncHelper.RunSync(() => UserManager.ProhibitPermissionAsync(subscriptionResult2.user, PermissionManager.GetPermission(AppPermissions.WebHook.UserCreated))); //remove permission from user2

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var allSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsPermissionGranted(AppWebHookDefinitionNames.Users.Created);

            allSubscriptions.Count.ShouldBe(2);
            allSubscriptions.Select(s => s.Id).ShouldContain(subscriptionResult1.webHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldNotContain(subscriptionResult2.webHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(subscriptionResult3.webHookSubscription.Id);

            var subscription1Result = allSubscriptions.Single(s => s.Id == subscriptionResult1.webHookSubscription.Id);
            subscription1Result.TenantId.ShouldBe(subscriptionResult1.webHookSubscription.TenantId);
            subscription1Result.UserId.ShouldBe(subscriptionResult1.user.Id);
            subscription1Result.Secret.ShouldBe(subscriptionResult1.webHookSubscription.Secret);
            subscription1Result.WebHookUri.ShouldBe(subscriptionResult1.webHookSubscription.WebHookUri);
            subscription1Result.WebHookDefinitions.ShouldBe(subscriptionResult1.webHookSubscription.WebHookDefinitions);
            subscription1Result.Headers.ShouldBe(subscriptionResult1.webHookSubscription.Headers);

            var subscription3Result = allSubscriptions.Single(s => s.Id == subscriptionResult3.webHookSubscription.Id);
            subscription3Result.TenantId.ShouldBe(subscriptionResult3.webHookSubscription.TenantId);
            subscription3Result.UserId.ShouldBe(subscriptionResult3.user.Id);
            subscription3Result.Secret.ShouldBe(subscriptionResult3.webHookSubscription.Secret);
            subscription3Result.WebHookUri.ShouldBe(subscriptionResult3.webHookSubscription.WebHookUri);
            subscription3Result.WebHookDefinitions.ShouldBe(subscriptionResult3.webHookSubscription.WebHookDefinitions);
            subscription3Result.Headers.ShouldBe(subscriptionResult3.webHookSubscription.Headers);
        }

        [Fact]
        public void Should_Get_All_Subscription_Features_Granted_Sync()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;
            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true"));

            var subscriptionResult1 = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Chat.NewMessageReceived));
            var subscriptionResult2 = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Chat.NewMessageReceived));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var allSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsPermissionGranted(AppWebHookDefinitionNames.Chat.NewMessageReceived);

            allSubscriptions.Count.ShouldBe(2);
            allSubscriptions.Select(s => s.Id).ShouldContain(subscriptionResult1.webHookSubscription.Id);
            allSubscriptions.Select(s => s.Id).ShouldContain(subscriptionResult2.webHookSubscription.Id);

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "false"));

            allSubscriptions = webHookSubscriptionManager.GetAllSubscriptionsPermissionGranted(AppWebHookDefinitionNames.Chat.NewMessageReceived);
            allSubscriptions.Count.ShouldBe(0);
        }

        [Fact]
        public void Should_Get_Is_Subscribed_Sync()
        {
            var subscriptionResult = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(new List<string> { AppWebHookDefinitionNames.Users.Created, AppWebHookDefinitionNames.Test },
                new List<string> { AppPermissions.WebHook.UserCreated }
                ));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var userIdentifier = new UserIdentifier(subscriptionResult.user.TenantId, subscriptionResult.user.Id);

            (webHookSubscriptionManager.IsSubscribed(userIdentifier, AppWebHookDefinitionNames.Users.Created)).ShouldBeTrue();
            (webHookSubscriptionManager.IsSubscribed(userIdentifier, AppWebHookDefinitionNames.Test)).ShouldBeTrue();

            AsyncHelper.RunSync(() => UserManager.ProhibitPermissionAsync(subscriptionResult.user, PermissionManager.GetPermission(AppPermissions.WebHook.UserCreated))); //remove permission from user

            (webHookSubscriptionManager.IsSubscribed(userIdentifier, AppWebHookDefinitionNames.Users.Created)).ShouldBeFalse();
            (webHookSubscriptionManager.IsSubscribed(userIdentifier, AppWebHookDefinitionNames.Test)).ShouldBeTrue();
        }

        [Fact]
        public void Should_DeActivate_Subscription_Sync()
        {
            var subscriptionResult = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            webHookSubscriptionManager.DeactivateSubscription(subscriptionResult.webHookSubscription.Id);

            var subscription = webHookSubscriptionManager.Get(subscriptionResult.webHookSubscription.Id);
            subscription.ShouldNotBeNull();
            subscription.IsActive.ShouldBeFalse();
        }

        [Fact]
        public void Should_Activate_Subscription_Sync()
        {
            var subscriptionResult = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            webHookSubscriptionManager.DeactivateSubscription(subscriptionResult.webHookSubscription.Id);

            var subscription = webHookSubscriptionManager.Get(subscriptionResult.webHookSubscription.Id);
            subscription.ShouldNotBeNull();
            subscription.IsActive.ShouldBeFalse();

            webHookSubscriptionManager.ActivateSubscription(subscriptionResult.webHookSubscription.Id);

            var activatedSubscription = webHookSubscriptionManager.Get(subscriptionResult.webHookSubscription.Id);
            activatedSubscription.ShouldNotBeNull();
            activatedSubscription.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Should_Add_WebHook_To_Subscription_Sync()
        {
            var subscriptionResult = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created, AppPermissions.WebHook.UserCreated));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var subscription = webHookSubscriptionManager.Get(subscriptionResult.webHookSubscription.Id);

            subscription.WebHookDefinitions.Count.ShouldBe(1);
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);

            webHookSubscriptionManager.AddWebHook(subscription.Id, AppWebHookDefinitionNames.Test);
        }

        [Fact]
        public void Should_Add_WebHook_Sync_Throw_Exception_If_Permissions_Are_Not_Granted()
        {
            var subscriptionResult = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Test));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var subscription = webHookSubscriptionManager.Get(subscriptionResult.webHookSubscription.Id);

            subscription.WebHookDefinitions.Count.ShouldBe(1);
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddWebHook(subscription.Id, AppWebHookDefinitionNames.Users.Created);
            });
        }

        [Fact]
        public void Should_Add_WebHook_Sync_Throw_Exception_If_Features_Are_Not_Granted()
        {
            AbpSession.TenantId = GetDefaultTenant().Id;

            var subscriptionResult = AsyncHelper.RunSync(() => CreateUserAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Test));

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var subscription = webHookSubscriptionManager.Get(subscriptionResult.webHookSubscription.Id);

            subscription.WebHookDefinitions.Count.ShouldBe(1);
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Test);

            Assert.Throws<AbpAuthorizationException>(() =>
            {
                webHookSubscriptionManager.AddWebHook(subscription.Id,
                    AppWebHookDefinitionNames.Chat.NewMessageReceived);
            });

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.ChatFeature, "true"));

            webHookSubscriptionManager.AddWebHook(subscription.Id, AppWebHookDefinitionNames.Chat.NewMessageReceived);
        }

        #endregion
    }
}
