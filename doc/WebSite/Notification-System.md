### Introduction

Notifications are used to **inform** users on specific events in the
system. ASP.NET Boilerplate provides a **pub/sub** based **real time**
notification infrastructure.

#### Sending Models

There are two ways of sending notifications to users:

-   User **subscribes** to a specific notification type. Then we
    **publish** a notification of this type which is delivered to all
    **subscribed** users. This is the **pub/sub** model.
-   We can **directly send** a notification to **target user(s)**.

#### Notification Types

There are also two types of notifications:

-   **General notifications** are arbitrary type of notifications.
    "Notify me if a user sends me a friendship request" is an example of
    this type notifications.
-   **Entity notifications** are associated to a specific entity.
    "Notify me if a user comment on **this** photo" is an entity based
    notification since it's associated to a specific photo entity. Users
    may want to get notifications for some photos, not for all.

#### Notification Data

A notification generally include a **notification data**. For example:
"Notify me if a user sends me a friendship request" notification may
have two data properties: *sender user name* (which user sent this
friendship request) and *request note* (a note that user did write in
the request). It's obvious that the notification data type is tightly
coupled to notification types. Different notification types have
different data types.

Notification data is **optional**. Some notifications may not require a
data. There are some pre-defined notification data types those can be
enough for most cases. **MessageNotificationData** can be used for
simple messages and **LocalizableMessageNotificationData** can be used
for localizable and parametric notification messages. We will see
example usage in later sections.

#### Notification Severity

There are 5 levels of notification severity, defined in
**NotificationSeverity** enum: **Info**, **Success**, **Warn**,
**Error** and **Fatal**. Default value is **Info**.

#### About Notification Persistence

See *Notification Store* section for more information on notification
persistence.

### Subscribe to Notifications

**INotificationSubscriptionManager** provides API to **subscribe** to
notifications. Examples:

    public class MyService : ITransientDependency
    {
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;

        public MyService(INotificationSubscriptionManager notificationSubscriptionManager)
        {
            _notificationSubscriptionManager = notificationSubscriptionManager;
        }

        //Subscribe to a general notification
        public async Task Subscribe_SentFrendshipRequest(int? tenantId, long userId)
        {
            await _notificationSubscriptionManager.SubscribeAsync(new UserIdentifier(tenantId, userId), "SentFrendshipRequest");    
        }

        //Subscribe to an entity notification
        public async Task Subscribe_CommentPhoto(int? tenantId, long userId, Guid photoId)
        {
            await _notificationSubscriptionManager.SubscribeAsync(new UserIdentifier(tenantId, userId), "CommentPhoto", new EntityIdentifier(typeof(Photo), photoId));   
        }
    }

First, we [injected](/Pages/Documents/Dependency-Injection)
**INotificationSubscriptionManager**. First method subscribes to a
**general notification**. User wants to get notified when someone sends
a friendship request. Second one subscribes to a notification related to
a **specific entity** (Photo). User wants to get notified if anyone
write comment to a specified photo.

Every notification type should have **unique names** (like
*SentFrendshipRequest* and *CommentPhoto* in the samples)

**INotificationSubscriptionManager** has also **UnsubscribeAsync,
IsSubscribedAsync, GetSubscriptionsAsync**... methods to manage
subscriptions.

### Publish Notifications

**INotificationPublisher** is used to publish notifications. Examples:

    public class MyService : ITransientDependency
    {
        private readonly INotificationPublisher _notiticationPublisher;

        public MyService(INotificationPublisher notiticationPublisher)
        {
            _notiticationPublisher = notiticationPublisher;
        }

        //Send a general notification to a specific user
        public async Task Publish_SentFrendshipRequest(string senderUserName, string friendshipMessage, UserIdentifier targetUserId)
        {
            await _notiticationPublisher.PublishAsync("SentFrendshipRequest", new SentFrendshipRequestNotificationData(senderUserName, friendshipMessage), userIds: new[] { targetUserId });
        }

        //Send an entity notification to a specific user
        public async Task Publish_CommentPhoto(string commenterUserName, string comment, Guid photoId, UserIdentifier photoOwnerUserId)
        {
            await _notiticationPublisher.PublishAsync("CommentPhoto", new CommentPhotoNotificationData(commenterUserName, comment), new EntityIdentifier(typeof(Photo), photoId), userIds: new[] { photoOwnerUserId });
        }

        //Send a general notification to all subscribed users in current tenant (tenant in the session)
        public async Task Publish_LowDisk(int remainingDiskInMb)
        {
            //Example "LowDiskWarningMessage" content for English -> "Attention! Only {remainingDiskInMb} MBs left on the disk!"
            var data = new LocalizableMessageNotificationData(new LocalizableString("LowDiskWarningMessage", "MyLocalizationSourceName"));
            data["remainingDiskInMb"] = remainingDiskInMb;

            await _notiticationPublisher.PublishAsync("System.LowDisk", data, severity: NotificationSeverity.Warn);    
        }
    }

In the first example, we published a notification to a single user.
*SentFrendshipRequestNotificationData* should be derived from
**NotificationData** like that:

    [Serializable]
    public class SentFrendshipRequestNotificationData : NotificationData
    {
        public string SenderUserName { get; set; }

        public string FriendshipMessage { get; set; }

        public SentFrendshipRequestNotificationData(string senderUserName, string friendshipMessage)
        {
            SenderUserName = senderUserName;
            FriendshipMessage = friendshipMessage;
        }
    }

In the second example, we sent a notification to a **specific user** for
a **specific entity**. Notification data classes don't need to be
**serialzable** normally (since JSON serialization is used by default).
But it's suggested to mark it as serializable since you may need to move
notifications between applications and may want to use binary
serialization in the future. Also, as declared before, notification data
is optional and may not be required for all notifications.

**Note**: If we publish a notification to **specific users**, they
**don't need** to be subscribed to those notifications.

In the third example, we did not define a dedicated notification data
class. instead, directly used built-in
**LocalizableMessageNotificationData** with **dictionary** based data
and published notification as '**Warn**'.
**LocalizableMessageNotificationData** can store dictionary-based
arbitrary data (this is also true for custom notification data classes
since they also inherit from **NotificationData** class). We used
"**remainingDiskInMb**" as argument on
[localization](/Pages/Documents/Localization). Localization message can
include these arguments (like "*Attention! Only {remainingDiskInMb} MBs
left on the disk!*" in the example). We will see how to localize it in
the Client Side section.

### User Notification Manager

**IUserNotificationManager** is used to manage notifications of users.
It has methods to **get**, **update** or **delete** notifications for a
user. You can use it to prepare a notification list page for your
application.

### Real Time Notifications

While you can use IUserNotificationManager to query notifications, we
generally want to push real time notifications to the client.

Notification system uses **IRealTimeNotifier** to send real time
notifications to users. This can be implemented with any type of real
time communication system. It's implemented using **SignalR** in a
seperated package. [Startup templates](/Templates) have already SignalR
installed. See [SignalR Integration
document](/Pages/Documents/SignalR-Integration) for more information.

**Note**: Notification system calls **IRealTimeNotifier** asynchronously
in a [**background job**](/Pages/Documents/Background-Jobs-And-Workers).
So, notifications may be sent with a small delay.

#### Client Side

When a real time notification is received, ASP.NET Boilerplate triggers
a **global event** in the client side. You can register like that to get
notifications:

    abp.event.on('abp.notifications.received', function (userNotification) {
        console.log(userNotification);
    });

**abp.notifications.received** event is triggered for each received real
time notification. You can register to this event as shown above to get
notifications. See [javascript event
bus](/Pages/Documents/Javascript-API/Event-Bus) documentation for more
information on events. An example incoming notification JSON for
"System.LowDisk" example:

    {
        "userId": 2,
        "state": 0,
        "notification": {
            "notificationName": "System.LowDisk",
            "data": {
                "message": {
                    "sourceName": "MyLocalizationSourceName",
                    "name": "LowDiskWarningMessage"
                },
                "type": "Abp.Notifications.LocalizableMessageNotificationData",
                "properties": {
                    "remainingDiskInMb": "42"
                }
            },
            "entityType": null,
            "entityTypeName": null,
            "entityId": null,
            "severity": 0,
            "creationTime": "2016-02-09T17:03:32.13",
            "id": "0263d581-3d8a-476b-8e16-4f6a6f10a632"
        },
        "id": "4a546baf-bf17-4924-b993-32e420a8d468"
    }

In this object;

-   **userId**: Current user id. Generally you don't need this since you
    know the current user.
-   **state**: Value of **UserNotificationState** enum. 0: **Unread**,
    1: **Read**.
-   **notification**: Notification details.
    -   **notificationName**: Unique name of the notification (same
        value used while publishing the notification).
    -   **data**: notification data. In this example, we used
        **LocalizableMessageNotificationData** (as published in the
        example before).
        -   **message**: Localizable message information. We can use
            **sourceName** and **name** to localize message on the UI.
        -   **type**: Notification data type. Full type name, including
            namespaces. We can check this type while processing the
            notification data.
        -   **properties**: Dictionary based custom properties.
    -   **entityType**, **entityTypeName** and **entityId**: Entity
        information if this is an entity related notification.
    -   **severity**: Value of **NotificationSeverity** enum. 0:
        **Info**, 1: **Success**, 2: **Warn**, 3: **Error**, 4:
        **Fatal**.
    -   **creationTime**: Time of when this notification is created.
    -   **id**: Notification id.
-   **id**: User notification id.

Surely, you will not just log the notification. You can use notification
data to show notification information to the user. Example:

    abp.event.on('abp.notifications.received', function (userNotification) {
        if (userNotification.notification.data.type === 'Abp.Notifications.LocalizableMessageNotificationData') {
            var localizedText = abp.localization.localize(
                userNotification.notification.data.message.name,
                userNotification.notification.data.message.sourceName
            );

            $.each(userNotification.notification.data.properties, function (key, value) {
                localizedText = localizedText.replace('{' + key + '}', value);
            });

            alert('New localized notification: ' + localizedText);
        } else if (userNotification.notification.data.type === 'Abp.Notifications.MessageNotificationData') {
            alert('New simple notification: ' + userNotification.notification.data.message);
        }
    });

To be able to process notification data, we should check the data type.
This example simply gets message from notification data. For the
localized message (LocalizableMessageNotificationData), we are
localizing the message and replacing parameters. For simple message
(MessageNotificationData), we directly get the message. Surely, in a
real project, we will not use alert function. We can use
[abp.notify](/Pages/Documents/Javascript-API/Notification) api to show
nice UI notifications.

If you need to implement such a logic above, there is an easier and
scaleable way. You can just use single line of code to show a [UI
notification](/Pages/Documents/Javascript-API/Notification) when a push
notification is received:

    abp.event.on('abp.notifications.received', function (userNotification) {
        abp.notifications.showUiNotifyForUserNotification(userNotification);
    });

This shows a [UI
notification](/Pages/Documents/Javascript-API/Notification) like that
(for System.LowDisk notification published above):

<img src="images/notification-warn.png" class="img-thumbnail" />

It works for built-in notification data types
(LocalizableMessageNotificationData and MessageNotificationData). If you
have custom notification data types, then you should register data
formatters like that:

    abp.notifications.messageFormatters['MyProject.MyNotificationDataType'] = function(userNotification) {
        return ...; //format and return message here
    };

Thus, **showUiNotifyForUserNotification** can create shown messages for
your data types. If you just need to the formatted message, you can
directly use
**abp.notifications.getFormattedMessageFromUserNotification(userNotification)**
which is internally used by showUiNotifyForUserNotification.

[Startup templates](/Templates) include the code to show UI
notifications when a push notification is received.

### Notification Store

Notification system uses **INotificationStore** to persist
notifications. This should be implemented in order to make notification
system properly working. You can implement it yourself or use
**[module-zero](/Pages/Documents/Zero/Overall)** which already
implements it.

### Notification Definitions

You don't have to **define** a notification before usage. You can just
use any **notification name** without defining it. But, defining it may
bring you some additional benefits. For example, you can then
**investigate** all notifications in your application. In this case, we
can define a **notification provider** for our
[module](/Pages/Documents/Module-System) as shown below:

    public class MyAppNotificationProvider : NotificationProvider
    {
        public override void SetNotifications(INotificationDefinitionContext context)
        {
            context.Manager.Add(
                new NotificationDefinition(
                    "App.NewUserRegistered",
                    displayName: new LocalizableString("NewUserRegisteredNotificationDefinition", "MyLocalizationSourceName"),
                    permissionDependency: new SimplePermissionDependency("App.Pages.UserManagement")
                    )
                );
        }
    }

"**App.NewUserRegistered**" is unique name of the notification. We
defined a localizable **displayName** (then we can show it while
subscribing to the notification on UI). And finally, we declared that
this notification is available to a user only if he has
"**App.Pages.UserManagement**"
[permission](/Pages/Documents/Authorization).

There are also some other parameters, you can investigate in the code.
Only notification name is **required** for a notification definition.

After defining such a notification provider, we should register it in
[PreInitialize](/Pages/Documents/Module-System#preinitialize) event
of our module, as shown below:

    public class AbpZeroTemplateCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Notifications.Providers.Add<MyAppNotificationProvider>();
        }

        //...
    }

Finally, you can inject and use **INotificationDefinitionManager** in
your application to get notification definitions. Then you may want to
prepare a automatic page to allow user to subscribe those notifications.
