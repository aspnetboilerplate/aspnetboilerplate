using Abp.NHibernate.EntityMappings;
using Abp.Webhooks;
using System;
using System.Net;

namespace Abp.Zero.NHibernate.EntityMappings;

public class WebhookSendAttemptMap : EntityMap<WebhookSendAttempt, Guid>
{
    public WebhookSendAttemptMap() : base("AbpWebhookSendAttempts")
    {
        Map(x => x.WebhookEventId)
            .Not.Nullable();
        Map(x => x.WebhookSubscriptionId)
            .Not.Nullable();
        Map(x => x.Response)
            .Length(Extensions.NvarcharMax);
        Map(x => x.ResponseStatusCode)
            .CustomType<HttpStatusCode>();

        this.MapCreationTime();
        this.MapLastModificationTime();
        Map(x => x.TenantId);

    }
}