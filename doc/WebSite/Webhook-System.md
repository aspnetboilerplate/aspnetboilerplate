### Introduction

Webhooks are used to **inform** tenants of specific events. ASP.NET Boilerplate provides a **pub/sub** (publish/subscribe) based webhook system.



### Webhook Definitions

First of all, you have to define webhooks that your system allows.

You must create a class inherited from `WebhookDefinitionProvider` and define your **WebhookDefinition**s as seen below.

```csharp
public class AppWebhookDefinitionProvider : WebhookDefinitionProvider
{
    public override void SetWebhooks(IWebhookDefinitionContext context)
    {
        context.Manager.Add(new WebhookDefinition(
            name: AppWebHookNames.NewUserRegistered,
            displayName: L("NewUserRegisteredWebhookDefinition")
        ));

        context.Manager.Add(new WebhookDefinition(
            name: AppWebHookNames.TenantDeleted,
            displayName: L("TenantDeletedWebhookDefinition"),
            description: L("DescriptionTenantDeletedWebhookDefinition"),
            featureDependency: new SimpleFeatureDependency(AppFeatures.TestCheckFeature)
        ));
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, AbpZeroTemplateConsts.LocalizationSourceName);
    }
}

```

*WebhookDefinition.cs:*

| Parameter                                |                           Summary                            |
| :--------------------------------------- | :----------------------------------------------------------: |
| Name* *(string)*                         | Unique name of the webhook. Must be unique. <br />*(You can use `IWebhookDefinitionManager.Contains` method to check whether it is already exists)* |
| DisplayName *(ILocalizableString)*       |                 Display name of the webhook.                 |
| Description *(ILocalizableString)*       |                 Description for the webhook.                 |
| FeatureDependency *(IFeatureDependency)* | A [Feature Dependency](https://aspnetboilerplate.com/Pages/Documents/Feature-Management). A defined webhook will be available to a tenant if this feature is enabled on the tenant.<br />*(All webhooks are available for the host)* |



After defining such a webhook provider, you must register it in the [PreInitialize](https://aspnetboilerplate.com/Pages/Documents/Module-System#preinitialize) method of our module, as shown below:

```csharp
public class AbpZeroTemplateCoreModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Webhooks.Providers.Add<AppWebhookDefinitionProvider>();
    }

    //...
}
```



### Subscribe to Webhook(s)

The **IWebhookSubscriptionManager** provides an API to **subscribe** to webhook. 

Examples:

```csharp
 private readonly IWebhookSubscriptionManager _webHookSubscriptionManager;

 public WebhookAppService(IWebhookSubscriptionManager webHookSubscriptionManager)
 {
     _webHookSubscriptionManager = webHookSubscriptionManager;
 }

 public async Task<string> AddTestSubscription()
 {
     var webhookSubscription = new WebhookSubscription()
     {
         TenantId = AbpSession.TenantId,
         WebhookUri = "http://localhost:21021/MyWebHookEndpoint",
         Webhooks = new List<string>()
         {
             AppWebHookNames.NewUserRegistered,
             AppWebHookNames.TenantDeleted
         },
         Headers = new Dictionary<string, string>()
         {
             { "MyTestHeaderKey", "MyTestHeaderValue" }, 
             { "MyTestHeaderKey2", "MyTestHeaderValue2" }
         }
     };
     await _webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(webhookSubscription);
     return webhookSubscription.Secret;
 }
```

*WebhookSubscription.cs:*

| Parameter                              |                           Summary                            |
| :------------------------------------- | :----------------------------------------------------------: |
| TenantId* *(int?)*                     |            Subscriber tenant's unique identifier             |
| WebhookUri* *(string)*                 |                    Your webhook endpoint                     |
| Webhooks *(list of string)*            | List of subscribed webhook names (*`WebhookDefinition.Name`*). You can receive a webhook event if you subscribe it. |
| Headers *(Dictionary<string, string>)* | Additional headers. You can add an additional header to the subscription. Your webhook will contain that header(s) additionally. |
| Secret *(string)*                      | Your private webhook secret. You can verify the received webhook by using that key. Do not share it publicly.<br />*(This value is automatically generated when you create a secret. Modification not recommended)* |

 

##### **Check Signature**

Abp currently uses the **SHA256** hash algorithm to create a signature. It creates a signature by hashing the HTTP request's body. Your other application which subscribed to a webhook should hash received body and check whether it is equal with received one. 

```csharp
[HttpPost]
public async Task WebHookTest()
{
    using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
    {
        var body = await reader.ReadToEndAsync();

        if (!IsSignatureCompatible("whs_YOURWEBHOOKSECRET", body))//read webhooksecret from user secret
        {
            throw new Exception("Unexpected Signature");
        }
        //It is certain that Webhook has not been modified.
    }
}

private bool IsSignatureCompatible(string secret, string body)
{
    if (HttpContext.Request.Headers.ContainsKey("abp-webhook-signature"))
    {
        return false;
    }

    var receivedSignature = HttpContext.Request.Headers["abp-webhook-signature"].ToString().Split("=");//will be something like "sha256=whs_XXXXXXXXXXXXXX"
    //It starts with hash method name (currently "sha256") then continue with signature. You can also check if your hash method is true.

    string computedSignature;
    switch (receivedSignature[0])
    {
        case "sha256":
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            using (var hasher = new HMACSHA256(secretBytes))
            {
                var data = Encoding.UTF8.GetBytes(body);
                computedSignature = BitConverter.ToString(hasher.ComputeHash(data));
            }
            break;
        default:
            throw new NotImplementedException();
    }
    return computedSignature == receivedSignature[1];
}

```



### Publish Webhooks

**IWebhookPublisher** is used to publish webhooks. Examples:

```csharp
public class AppWebhookPublisher : DomainService, IAppWebhookPublisher
{
    private readonly IWebhookPublisher _webHookPublisher;

    public AppWebhookPublisher(IWebhookPublisher webHookPublisher)
    {
        _webHookPublisher = webHookPublisher;
    }

    public async Task NewUserRegisteredAsync(User user)
    {
        await _webHookPublisher.PublishAsync(AppWebHookNames.NewUserRegistered,
            new
            {
                UserName = user.UserName,
                EmailAddress = user.EmailAddress
            }
        );
    }

    public async Task OnMyDataChanged(MyDataChangedInput myDataChangedInput)
    {
        await _webHookPublisher.PublishAsync(AppWebHookNames.OnDataChanged, myDataChangedInput);
    }
}
```

It will send webhooks  to all subscriptions of the current tenant. If there is no subscription it does not do anything.

If you want to send webhook(s) to a specific tenant you can set tenant id as seen below

```csharp
 await _webHookPublisher.PublishAsync(AppWebHookNames.OnDataChanged, myDataChangedInput,5);//sends webhook(s) to subscriptions of the tenant whose id is 5

 await _webHookPublisher.PublishAsync(AppWebHookNames.TenantDeleted, tenantDeletedInput,null);//sends webhook(s) to subscriptions of host
```



### Configuration

Webhook configurations:

| Parameter                                                    |                           Summary                            |
| :----------------------------------------------------------- | :----------------------------------------------------------: |
| TimeoutDuration *(TimeSpan)*                                 | HttpClient timeout. **WebhookSender** will wait `TimeoutDuration` second before throw timeout exception. Then the webhook will be considered unsuccessful. |
| MaxSendAttemptCount *(int)*                                  | Max send attempt count that **IWebhookPublisher** will try to resend webhook until gets HttpStatusCode.OK |
| JsonSerializerSettings *([JsonSerializerSettings](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonSerializerSettings.htm))* | Json serializer settings for converting webhook data to json, If this is null default settings will be used.*(JsonExtensions.ToJsonString(object,bool,bool))* |
| Providers *(type list of WebhookDefinitionProvider(s))*      |                      Webhook providers.                      |
| IsAutomaticSubscriptionDeactivationEnabled *(bool)*          | If you enable that, subscriptions will be automatically disabled if they fails `MaxConsecutiveFailCountBeforeDeactivateSubscription` times consecutively.<br/>         Tenants should activate it back manually. |
| MaxConsecutiveFailCountBeforeDeactivateSubscription *(int)*  | Max consecutive fail count to deactivate subscription if `IsAutomaticSubscriptionDeactivationEnabled` is true |



### Auto Subscription Deactivation

Webhook has built-in auto subscription deactivation features. Sometimes webhook endpoints can be unreachable for a long time, not to send webhooks to that endpoints again and again, you can enable auto subscription deactivation. After webhook attempts of subscription fail `MaxConsecutiveFailCountBeforeDeactivateSubscription ` times, the subscription becomes inactive till tenant go and activate it back. 

``````csharp
public class AbpZeroTemplateCoreModule : AbpModule
{
    public override void PreInitialize()
    {
    	Configuration.Webhooks.IsAutomaticSubscriptionDeactivationEnabled = true;//default false
	Configuration.Webhooks.MaxConsecutiveFailCountBeforeDeactivateSubscription = 15;
	
        //if an endpoint fails 15 times consecutively, the subscription will be inactive.
        //default value of MaxConsecutiveFailCountBeforeDeactivateSubscription is MaxSendAttemptCount x 3
    }
    //...
}
``````



### Tables

**WebHookSubscriptionInfo:** Stores webhook subscriptions

**WebhookEvent:** Stores created webhook's data. For example when a new user created then webhook published. One **WebhookEvent** will be created then webhooks will be sent to all subscribed tenants with using that data.

**WebhookSendAttempt:** Table for store webhook send attempts. Each row stores an attempt to send a webhook of **WebhookEvent** to subscriptions of tenants with the webhook result.
