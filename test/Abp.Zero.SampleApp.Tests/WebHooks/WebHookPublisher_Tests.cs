using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Json;
using Abp.WebHooks;
using Abp.WebHooks.BackgroundWorker;
using Abp.Zero.SampleApp.Application;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookPublisher_Tests : WebHookTestBase
    {
        private IWebHookPublisher _webhookPublisher;
        private IBackgroundJobManager _backgroundJobManagerSubstitute;

        public WebHookPublisher_Tests()
        {
            AbpSession.UserId = 1;
            AbpSession.TenantId = null;

            _backgroundJobManagerSubstitute = RegisterFake<IBackgroundJobManager>();
            _webhookPublisher = Resolve<IWebHookPublisher>();
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_If_There_Is_No_Users()
        {
            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Users.Created,
                new
                {
                    Name = "Musa",
                    Surname = "Demir"
                });

            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Authorized_User()
        {
            var subscriptionResult = await CreateUserAndSubscribeToWebhook(AppWebHookDefinitionNames.Users.Created,
                AppPermissions.WebHook.UserCreated);

            var webHooksConfiguration = Resolve<IWebHooksConfiguration>();

            var data = new { Name = "Musa", Surname = "Demir" };

            await _webhookPublisher.PublishAsync(
                new UserIdentifier(subscriptionResult.user.TenantId, subscriptionResult.user.Id),
                AppWebHookDefinitionNames.Users.Created, data);

            Predicate<WebHookSenderInput> predicate = w =>
            {
                w.Secret.ShouldBe("secret");
                w.WebHookDefinition.ShouldContain(AppWebHookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                w.WebHookSubscriptionId.ShouldBe(subscriptionResult.webHookSubscription.Id);
                w.Data.ShouldBe(
                    webHooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webHooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_UnAuthorized_User()
        {
            var subscriptionResult = await CreateUserAndSubscribeToWebhook(AppWebHookDefinitionNames.Users.Created,
                AppPermissions.WebHook.UserCreated); //will create user with permission then subscribe to webhook

            await UserManager.ProhibitPermissionAsync(subscriptionResult.user, PermissionManager.GetPermission(AppPermissions.WebHook.UserCreated)); //then remove permission from user 

            await _webhookPublisher.PublishAsync(
                new UserIdentifier(subscriptionResult.user.TenantId, subscriptionResult.user.Id),
                AppWebHookDefinitionNames.Users.Created, new { Name = "Musa", Surname = "Demir" });

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_If_User_Has_All_Permissions()
        {
            var subscriptionResult = await CreateUserAndSubscribeToWebhook(new List<string>() { AppWebHookDefinitionNames.Tenant.Deleted },
                new List<string>() { AppPermissions.WebHook.TenantMainPermission, AppPermissions.WebHook.Tenant.TenantDeleted });

            var webHooksConfiguration = Resolve<IWebHooksConfiguration>();

            var data = new { Name = "Musa", Surname = "Demir" };

            await _webhookPublisher.PublishAsync(
                new UserIdentifier(subscriptionResult.user.TenantId, subscriptionResult.user.Id),
                AppWebHookDefinitionNames.Tenant.Deleted, data);

            Predicate<WebHookSenderInput> predicate = w =>
            {
                w.Secret.ShouldBe("secret");
                w.WebHookDefinition.ShouldContain(AppWebHookDefinitionNames.Tenant.Deleted);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                w.WebHookSubscriptionId.ShouldBe(subscriptionResult.webHookSubscription.Id);

                w.Data.ShouldBe(
                    webHooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webHooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_If_User_Does_Not_Have_All_Permissions_When_Its_Required_All()
        {
            //subscribe to webhook with permissions
            var subscriptionResult = await CreateUserAndSubscribeToWebhook(new List<string>() { AppWebHookDefinitionNames.Tenant.Deleted },
                new List<string>() { AppPermissions.WebHook.TenantMainPermission, AppPermissions.WebHook.Tenant.TenantDeleted });

            //then remove permission from user 
            await UserManager.ProhibitPermissionAsync(subscriptionResult.user, PermissionManager.GetPermission(AppPermissions.WebHook.TenantMainPermission));
            
            await _webhookPublisher.PublishAsync(
                new UserIdentifier(subscriptionResult.user.TenantId, subscriptionResult.user.Id),
                AppWebHookDefinitionNames.Tenant.Deleted, new { Name = "Musa", Surname = "Demir" });

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }
    }
}
