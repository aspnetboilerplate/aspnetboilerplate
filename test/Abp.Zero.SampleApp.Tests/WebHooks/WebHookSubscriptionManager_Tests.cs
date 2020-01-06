using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Json;
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookSubscriptionManager_Tests : WebHookTestBase
    {
        [Fact]
        public async Task Should_Not_Try_To_Add_Subscribe_If_Permissions_Are_Not_Granted()
        {
            var user = await CreateNewUserWithWebhookPermissions();

            var newSubscription = new WebHookSubscription()
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywehhook.com",
                WebHookDefinitions = new List<string>() { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>()
                {
                    { "Key","Value"}
                }
            };

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await WebHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });
        }

        [Fact]
        public async Task Should_Not_Try_To_Update_Subscribe_If_Permissions_Are_Not_Granted()
        {
            var user = await CreateNewUserWithWebhookPermissions(new List<string>() { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription()
            {
                Id = Guid.NewGuid(),
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywehhook.com",
                WebHookDefinitions = new List<string>() { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>()
                {
                    { "Key","Value"}
                }
            };

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await WebHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);
            });
        }

        [Fact]
        public async Task Should_Subscribe_To_WebHook()
        {
            var user = await CreateNewUserWithWebhookPermissions(new List<string>() { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription()
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywehhook.com",
                WebHookDefinitions = new List<string>() { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>()
                {
                    { "Key","Value"}
                }
            };


            await WebHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

            var subscription = WebHookSubscriptionManager.Get(newSubscription.Id);

            subscription.ShouldNotBeNull();
            subscription.WebHookDefinitions.ShouldContain(AppWebHookDefinitionNames.Users.Created);
        }

        [Fact]
        public async Task Should_Add_Or_Update_Subscription_Async_Insert_If_Id_Is_Null()
        {
            var webHookStoreSubstitute = RegisterFake<IWebHookSubscriptionsStore>();

            var user = await CreateNewUserWithWebhookPermissions(new List<string>() { AppPermissions.WebHook.UserCreated });

            var newSubscription = new WebHookSubscription()
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string>() { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>()
                {
                    { "Key","Value"}
                }
            };


            await WebHookSubscriptionManager.AddOrUpdateSubscriptionAsync(newSubscription);

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

            await webHookStoreSubstitute.Received().InsertAsync(Arg.Is<WebHookSubscriptionInfo>(w => predicate(w)));
            await webHookStoreSubstitute.DidNotReceive().InsertAsync(Arg.Any<WebHookSubscriptionInfo>());

        }
    }
}
