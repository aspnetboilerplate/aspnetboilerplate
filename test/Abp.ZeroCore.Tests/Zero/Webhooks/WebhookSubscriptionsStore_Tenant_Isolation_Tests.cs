using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Domain.Entities;
using Abp.Webhooks;
using Shouldly;
using Xunit;

namespace Abp.Zero.Webhooks;

public class WebhookSubscriptionsStore_Tenant_Isolation_Tests : AbpZeroTestBase
{
    private const int OtherTenantId = 42;

    private readonly IWebhookSubscriptionsStore _webhookSubscriptionsStore;

    public WebhookSubscriptionsStore_Tenant_Isolation_Tests()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = true;

        _webhookSubscriptionsStore = Resolve<IWebhookSubscriptionsStore>();
    }

    [Fact]
    public async Task Should_Get_Subscription_Of_Current_Tenant()
    {
        var subscriptionId = CreateSubscription(AbpSession.TenantId);

        var subscription = await _webhookSubscriptionsStore.GetAsync(subscriptionId);

        subscription.ShouldNotBeNull();
        subscription.TenantId.ShouldBe(AbpSession.TenantId);
    }

    [Fact]
    public async Task Should_Not_Get_Subscription_Of_Another_Tenant()
    {
        var subscriptionId = CreateSubscription(OtherTenantId);

        await Should.ThrowAsync<EntityNotFoundException>(() => _webhookSubscriptionsStore.GetAsync(subscriptionId));
    }

    [Fact]
    public async Task Should_Not_Delete_Subscription_Of_Another_Tenant()
    {
        var subscriptionId = CreateSubscription(OtherTenantId);

        await _webhookSubscriptionsStore.DeleteAsync(subscriptionId);

        SubscriptionExists(OtherTenantId, subscriptionId).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Get_All_Subscriptions_Of_A_Given_Tenant()
    {
        // The publisher runs on the host and asks for the subscriptions of a specific tenant.
        var subscriptionId = CreateSubscription(OtherTenantId);

        var subscriptions = await _webhookSubscriptionsStore.GetAllSubscriptionsAsync(OtherTenantId);

        subscriptions.ShouldContain(subscription => subscription.Id == subscriptionId);
    }

    [Fact]
    public async Task Should_Get_All_Subscriptions_Of_Multiple_Tenants()
    {
        // Sending one webhook to several tenants has to reach across the tenant boundary on purpose.
        var ownSubscriptionId = CreateSubscription(AbpSession.TenantId);
        var otherSubscriptionId = CreateSubscription(OtherTenantId);

        var subscriptions = await _webhookSubscriptionsStore.GetAllSubscriptionsOfTenantsAsync(
            new[] { AbpSession.TenantId, OtherTenantId });

        subscriptions.ShouldContain(subscription => subscription.Id == ownSubscriptionId);
        subscriptions.ShouldContain(subscription => subscription.Id == otherSubscriptionId);
    }

    [Fact]
    public async Task Should_Only_Report_Subscribed_For_The_Given_Tenant()
    {
        CreateSubscription(OtherTenantId);

        (await _webhookSubscriptionsStore.IsSubscribedAsync(OtherTenantId, "Test.Webhook")).ShouldBeTrue();
        (await _webhookSubscriptionsStore.IsSubscribedAsync(AbpSession.TenantId, "Test.Webhook")).ShouldBeFalse();
    }

    private Guid CreateSubscription(int? tenantId)
    {
        var subscriptionId = Guid.NewGuid();

        UsingDbContext(tenantId, context =>
        {
            context.WebhookSubscriptions.Add(new WebhookSubscriptionInfo
            {
                Id = subscriptionId,
                TenantId = tenantId,
                WebhookUri = "https://localhost/webhook",
                Secret = "secret",
                IsActive = true,
                Webhooks = "[\"Test.Webhook\"]",
                Headers = "{}"
            });

            context.SaveChanges();
        });

        return subscriptionId;
    }

    private bool SubscriptionExists(int? tenantId, Guid subscriptionId)
    {
        return UsingDbContext(tenantId,
            context => context.WebhookSubscriptions.Any(subscription => subscription.Id == subscriptionId));
    }
}
