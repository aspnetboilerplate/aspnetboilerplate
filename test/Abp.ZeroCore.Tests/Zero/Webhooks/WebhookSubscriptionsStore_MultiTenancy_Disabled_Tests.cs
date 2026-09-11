using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Webhooks;
using Shouldly;
using Xunit;

namespace Abp.Zero.Webhooks;

public class WebhookSubscriptionsStore_MultiTenancy_Disabled_Tests : AbpZeroTestBase
{
    private readonly IWebhookSubscriptionsStore _webhookSubscriptionsStore;

    public WebhookSubscriptionsStore_MultiTenancy_Disabled_Tests()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = false;

        _webhookSubscriptionsStore = Resolve<IWebhookSubscriptionsStore>();
    }

    [Fact]
    public async Task Should_Set_Default_Tenant_On_Insert()
    {
        var subscriptionId = await InsertSubscriptionAsync();

        UsingDbContext(context => context.WebhookSubscriptions.Single(subscription => subscription.Id == subscriptionId))
            .TenantId.ShouldBe(MultiTenancyConsts.DefaultTenantId);
    }

    [Fact]
    public async Task Should_Get_Inserted_Subscription()
    {
        var subscriptionId = await InsertSubscriptionAsync();

        var subscription = await _webhookSubscriptionsStore.GetAsync(subscriptionId);

        subscription.ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_Find_Inserted_Subscription_For_Session_Tenant()
    {
        // The publisher uses AbpSession.TenantId when no tenant is given, which is the default tenant here.
        var subscriptionId = await InsertSubscriptionAsync();

        var subscriptions =
            await _webhookSubscriptionsStore.GetAllSubscriptionsAsync(AbpSession.TenantId, "Test.Webhook");

        subscriptions.ShouldContain(subscription => subscription.Id == subscriptionId);
        (await _webhookSubscriptionsStore.IsSubscribedAsync(AbpSession.TenantId, "Test.Webhook")).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Update_Inserted_Subscription()
    {
        var subscriptionId = await InsertSubscriptionAsync();

        var subscription = await _webhookSubscriptionsStore.GetAsync(subscriptionId);
        subscription.IsActive = false;
        await _webhookSubscriptionsStore.UpdateAsync(subscription);

        (await _webhookSubscriptionsStore.GetAsync(subscriptionId)).IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Delete_Inserted_Subscription()
    {
        var subscriptionId = await InsertSubscriptionAsync();

        await _webhookSubscriptionsStore.DeleteAsync(subscriptionId);

        UsingDbContext(context => context.WebhookSubscriptions.Any(subscription => subscription.Id == subscriptionId))
            .ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Find_Existing_Subscription_Without_Tenant_When_Publishing_Without_Tenant()
    {
        // Subscriptions stored before WebhookSubscriptionInfo became IMayHaveTenant may have a null TenantId.
        // Getting or deleting them by id requires updating their TenantId to the default tenant.
        var subscriptionId = CreateSubscriptionWithoutTenant();

        var subscriptions = await _webhookSubscriptionsStore.GetAllSubscriptionsAsync(null, "Test.Webhook");

        subscriptions.ShouldContain(subscription => subscription.Id == subscriptionId);
    }

    private async Task<Guid> InsertSubscriptionAsync()
    {
        var subscription = new WebhookSubscriptionInfo
        {
            Id = Guid.NewGuid(),
            WebhookUri = "https://localhost/webhook",
            Secret = "secret",
            IsActive = true,
            Webhooks = "[\"Test.Webhook\"]",
            Headers = "{}"
        };

        await _webhookSubscriptionsStore.InsertAsync(subscription);

        return subscription.Id;
    }

    private Guid CreateSubscriptionWithoutTenant()
    {
        var subscriptionId = Guid.NewGuid();

        UsingDbContext(context =>
        {
            context.SuppressAutoSetTenantId = true;

            context.WebhookSubscriptions.Add(new WebhookSubscriptionInfo
            {
                Id = subscriptionId,
                TenantId = null,
                WebhookUri = "https://localhost/webhook",
                Secret = "secret",
                IsActive = true,
                Webhooks = "[\"Test.Webhook\"]",
                Headers = "{}"
            });
        });

        return subscriptionId;
    }
}
