### Introduction

The [Abp.AspNetCore.SignalR](http://www.nuget.org/packages/Abp.AspNetCore.SignalR) NuGet
package makes it easier to use **ASP.NET Core SignalR** in ASP.NET Boilerplate-based
applications.

### Installation

#### Server-Side

Install the
[**Abp.AspNetCore.SignalR**](http://www.nuget.org/packages/Abp.AspNetCore.SignalR)
NuGet package to your project (generally to your Web layer) and add a
**dependency** to your module:

    [DependsOn(typeof(AbpAspNetCoreSignalRModule))]
    public class YourProjectWebModule : AbpModule
    {
        //...
    }


Then use the **AddSignalR** and **UseSignalR** methods in your Startup class:

    using Abp.AspNetCore.SignalR.Hubs;
    
    namespace MyProject.Web.Startup
    {
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSignalR();
            }
    
            public void Configure(IApplicationBuilder app)
            {
                app.UseSignalR(routes =>
                {
                    routes.MapHub<AbpCommonHub>("/signalr");
                });
            }
        }
    }

#### Client-Side (Angular)

The **@aspnet/signalr** package should be added in package.json, and the signalr.min.js included under **scripts** in angular.json.

The **abp.signalr-client.js** script should be included under **assets** in angular.json.

SignalR cannot send authorization headers, so encryptedAuthToken is sent in the query string. The startup template includes SignalRAspNetCoreHelper. We should call it in ngOnInit in app.component.ts:

    SignalRAspNetCoreHelper.initSignalR();

That's all you have to do. SignalR is properly configured and integrated into your project.

#### Client-Side (jQuery)

The **abp.signalr-client.js** script should be included on the page. It's located
in the
**[Abp.Web.Resources](https://www.nuget.org/packages/Abp.Web.Resources)**
package (and already installed in the [startup templates](/Templates)). We
should include it after the signalr.min.js:

    <script src="~/lib/signalr-client/signalr.min.js"></script>
    <script src="~/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.signalr-client.js"></script>

That's all you have to do. SignalR is properly configured and integrated into your project.

### Connection Establishment

ASP.NET Boilerplate **automatically connects** to the server (from the
client) when **abp.signalr-client.js** is included on your page. This is
generally fine. But there may be some cases where you may not want it. You can add
these lines just before including **abp.signalr-client.js** to disable auto
connecting:

    <script>
        abp.signalr = abp.signalr || {};
        abp.signalr.autoConnect = false;
    </script>

In this case, you can call the **abp.signalr.connect()** function manually
whenever you need to connect to the server.

ASP.NET Boilerplate also **automatically reconnects** to the server
(from the client) when the client disconnects, if
**abp.signalr.autoConnect** is true.

The **"abp.signalr.connected"** global event is triggered when the client
connects to the server. You can register to this event to take actions
when the connection is successfully established. See the JavaScript [event
bus documentation](/Pages/Documents/Javascript-API/Event-Bus) for more information
about client-side events.

### Built-In Features

You can use all the power of SignalR in your applications. Additionally, the
**Abp.AspNetCore.SignalR** package implements some built-in features.

#### Notification

The **Abp.AspNetCore.SignalR** package implements the **IRealTimeNotifier** to send
real-time notifications to clients (see the [notification
system](/Pages/Documents/Notification-System)). This way, your users can get
real-time push notifications!

#### Online Clients

ASP.NET Boilerplate provides the **IOnlineClientManager** to get information
about online users (inject IOnlineClientManager and use
GetByUserIdOrNull, GetAllClients, IsOnline methods for example).
The IOnlineClientManager needs a communication infrastructure to properly
work. The **Abp.AspNetCore.SignalR** package provides this infrastructure, so you
can inject and use IOnlineClientManager in any layer of your application
if SignalR is installed.

### Your SignalR Code

The **Abp.AspNetCore.SignalR** package also simplifies your SignalR code. Consider
that we want to add a Hub to our application:

    public class MyChatHub : Hub, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }
    
        public ILogger Logger { get; set; }
    
        public MyChatHub()
        {
            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
        }
    
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("getMessage", string.Format("User {0}: {1}", AbpSession.UserId, message));
        }
    
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Logger.Debug("A client connected to MyChatHub: " + Context.ConnectionId);
        }
    
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            Logger.Debug("A client disconnected from MyChatHub: " + Context.ConnectionId);
        }
    }

<!-- -->

    routes.MapHub<MyChatHub>("/signalr-myChatHub"); // Prefix with '/signalr'

We implemented the **ITransientDependency** interface to simply register our hub to the
[dependency injection](/Pages/Documents/Dependency-Injection) system
(you can make it a singleton based on your needs). We
[property-injected](/Pages/Documents/Dependency-Injection#property-injection-pattern)
the [session](/Pages/Documents/Abp-Session) and
[logger](/Pages/Documents/Logging).
Alternatively, we can inherit AbpHubBase.

**SendMessage** is a method of our hub that can be used by clients. We
call the **getMessage** function of **all** clients in this method. We can
use the [AbpSession](/Pages/Documents/Abp-Session) to get the current user id
(if user logged in) as done above. We also overrode **OnConnectedAsync** and
**OnDisconnectedAsync**, which is just for demonstration purposes and not needed here.

Here is the **client-side** JavaScript code to send/receive messages using
our hub.

    var chatHub = null;
    
    abp.signalr.startConnection(abp.appPath + 'signalr-myChatHub', function (connection) {
        chatHub = connection; // Save a reference to the hub
    
        connection.on('getMessage', function (message) { // Register for incoming messages
            console.log('received message: ' + message);
        });
    }).then(function (connection) {
        abp.log.debug('Connected to myChatHub server!');
        abp.event.trigger('myChatHub.connected');
    });
    
    abp.event.on('myChatHub.connected', function() { // Register for connect event
        chatHub.invoke('sendMessage', "Hi everybody, I'm connected to the chat!"); // Send a message to the server
    });

We can then use the **chatHub** anytime we need to send messages to the
server.
