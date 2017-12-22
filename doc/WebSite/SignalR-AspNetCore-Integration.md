### Introduction

[Abp.AspNetCore.SignalR](http://www.nuget.org/packages/Abp.AspNetCore.SignalR) NuGet
package makes it easier to use **ASP.NET Core SignalR** in ASP.NET Boilerplate-based
applications.

> NOTICE: This package is currently in preview. If you have a problem, please write to the Github issues: https://github.com/aspnetboilerplate/aspnetboilerplate/issues/new

### Installation

#### Server Side

Install
[**Abp.AspNetCore.SignalR**](http://www.nuget.org/packages/Abp.AspNetCore.SignalR)
NuGet package to your project (generally to your Web layer) and add a
**dependency** to your module:

    [DependsOn(typeof(AbpWebSignalRModule))]
    public class YourProjectWebModule : AbpModule
    {
        //...
    }


Then use **AddSignalR** and **UseSignalR** methods in your Startup class:

    using Abp.Web.SignalR.Hubs;

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

#### Client Side (jQuery)

**abp.signalr-client.js** script should be included in the page. It's located
in
**[Abp.Web.Resources](https://www.nuget.org/packages/Abp.Web.Resources)**
package (It's already installed in [startup templates](/Templates)). We
should include it after signalr-client.min.js:

    <script src="~/lib/signalr-client/signalr-client.min.js"></script>
    <script src="~/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.signalr-client.js"></script>

That's all. SignalR is properly configured and integrated to your
project.

### Connection Establishment

ASP.NET Boilerplate **automatically connects** to the server (from the
client) when **abp.signalr-client.js** is included to your page. This is
generally fine. But there may be cases you don't want it to. You can add
these lines just before including **abp.signalr-client.js** to disable auto
connecting:

    <script>
        abp.signalr = abp.signalr || {};
        abp.signalr.autoConnect = false;
    </script>

In this case, you can call **abp.signalr.connect()** function manually
whenever you need to connect to the server.

ASP.NET Boilerplate also **automatically reconnects** to the server
(from the client) when client disconnects, if
**abp.signalr.autoConnect** is true.

**"abp.signalr.connected"** global event is triggered when client
connects to the server. You can register to this event to take actions
when the connection is successfully established. See JavaScript [event
bus documentation](/Pages/Documents/Javascript-API/Event-Bus) for more
about client side events.

### Built-In Features

You can use all the power of SignalR in your applications. In addition,
**Abp.AspNetCore.SignalR** package implements some built-in features.

#### Notification

**Abp.AspNetCore.SignalR** package implements **IRealTimeNotifier** to send
real time notifications to clients (see [notification
system](/Pages/Documents/Notification-System)). Thus, your users can get
real time push notifications.

#### Online Clients

ASP.NET Boilerplate provides **IOnlineClientManager** to get information
about online users (inject IOnlineClientManager and use
GetByUserIdOrNull, GetAllClients, IsOnline methods for example).
IOnlineClientManager needs a communication infrastructure to properly
work. **Abp.AspNetCore.SignalR** package provides the infrastructure. So, you
can inject and use IOnlineClientManager in any layer of your application
if SignalR is installed.

### Your SignalR Code

**Abp.AspNetCore.SignalR** package also simplifies your SignalR code. Consider
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
            await Clients.All.getMessage(string.Format("User {0}: {1}", AbpSession.UserId, message));
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

    routes.MapHub<MyChatHub>("/myChatHub");

We implemented **ITransientDependency** to simply register our hub to
[dependency injection](/Pages/Documents/Dependency-Injection) system
(you can make it singleton based on your needs). We
[property-injected](/Pages/Documents/Dependency-Injection#property-injection-pattern)
the [session](/Pages/Documents/Abp-Session) and
[logger](/Pages/Documents/Logging).

**SendMessage** is a method of our hub that can be used by clients. We
call **getMessage** function of **all** clients in this method. We can
use [AbpSession](/Pages/Documents/Abp-Session) to get current user id
(if user logged in) as done above. We also overrided **OnConnectedAsync** and
**OnDisconnectedAsync**, which is just for demonstration and not needed here
actually.

Here is the **client side** JavaScript code to send/receive messages using
our hub.

    var chatHub = null;

    abp.signalr.startConnection('/myChatHub', function (connection) {
        chatHub = connection; // Save a reference to the hub

        connection.on('getMessage', function (message) { // Register for incoming messages
            console.log('received message: ' + message);
        });
    });

    abp.event.on('abp.signalr.connected', function() { // Register for connect event
        chatHub.invoke('sendMessage', "Hi everybody, I'm connected to the chat!"); // Send a message to the server
    });

Then we can use **chatHub** anytime we need to send message to the
server.
