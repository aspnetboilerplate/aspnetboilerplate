### Introduction

This document is for .NET Framework 4.6.1. If you're interested in ASP.NET
Core, see the [SignalR AspNetCore Integration](SignalR-AspNetCore-Integration.md) documentation instead.

The [Abp.Web.SignalR](http://www.nuget.org/packages/Abp.Web.SignalR) NuGet
package makes it easy to use **SignalR** in ASP.NET Boilerplate-based
applications. See the [SignalR documentation](http://www.asp.net/signalr)
for more detailed information on SignalR.

### Installation

#### Server-Side

Install the
[**Abp.Web.SignalR**](http://www.nuget.org/packages/Abp.Web.SignalR)
NuGet package to your project (generally to your Web layer) and add a
**dependency** to your module:

    [DependsOn(typeof(AbpWebSignalRModule))]
    public class YourProjectWebModule : AbpModule
    {
        //...
    }


Then use the **MapSignalR** method in your OWIN startup class as you always
do:

    [assembly: OwinStartup(typeof(Startup))]
    namespace MyProject.Web
    {
        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                //...

                app.MapSignalR();
            }
        }
    }

**Note:** Abp.Web.SignalR only depends on the Microsoft.AspNet.SignalR.Core
package, so you will also need to **install** the
**[Microsoft.AspNet.SignalR](https://www.nuget.org/packages/Microsoft.AspNet.SignalR)**
package to your Web project, if you haven't installed it before (See the [SignalR
documents](http://www.asp.net/signalr) for more info).

#### Client-Side

The **abp.signalr.js** script should be included on the page. It's located
in the
**[Abp.Web.Resources](https://www.nuget.org/packages/Abp.Web.Resources)**
package (It's already installed in the [startup templates](/Templates)). We
should include it after signalr hubs:

    <script src="~/signalr/hubs"></script>
    <script src="~/Abp/Framework/scripts/libs/abp.signalr.js"></script>


That's all you have to do! SignalR is properly configured and integrated into your
project.

### Connection Establishment

ASP.NET Boilerplate **automatically connects** to the server (from the
client) when **abp.signalr.js** is included on your page. This is
generally fine, but there may be cases where you might not want to. You can add
these lines just before including **abp.signalr.js** to disable auto
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
bus documentation](/Pages/Documents/Javascript-API/Event-Bus) for more
information about client-side events.

### Built-In Features

You can use the full power of SignalR in your applications. Additionally,
the **Abp.Web.SignalR** package implements some built-in features.

#### Notification

The **Abp.Web.SignalR** package implements the **IRealTimeNotifier** to send
real-time notifications to clients (see the [notification
system](/Pages/Documents/Notification-System)). This way, your users can get
real-time push notifications.

#### Online Clients

ASP.NET Boilerplate provides the **IOnlineClientManager** to get information
about online users (inject IOnlineClientManager and use the
GetByUserIdOrNull, GetAllClients, and IsOnline methods, for example).
The IOnlineClientManager needs a communication infrastructure to properly
work. The **Abp.Web.SignalR** package provides that infrastructure, so you
can inject and use IOnlineClientManager in any layer of your application,
if SignalR is installed.

#### PascalCase vs. camelCase

The Abp.Web.SignalR package overrides SignalR's default **ContractResolver**
to use the **CamelCasePropertyNamesContractResolver** on serialization.
This way, we can have classes with **PascalCase** properties on the server
and use them as **camelCase** on the client for sending/receiving
objects (because camelCase is preferred notation in JavaScript). If you
want to ignore this for your classes in some assemblies, then you can
add those assemblies to the AbpSignalRContractResolver.**IgnoredAssemblies**
list.

### Your SignalR Code

The **Abp.Web.SignalR** package also simplifies your SignalR code. Imagine
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

        public void SendMessage(string message)
        {
            Clients.All.getMessage(string.Format("User {0}: {1}", AbpSession.UserId, message));
        }

        public async override Task OnConnected()
        {
            await base.OnConnected();
            Logger.Debug("A client connected to MyChatHub: " + Context.ConnectionId);
        }

        public async override Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);
            Logger.Debug("A client disconnected from MyChatHub: " + Context.ConnectionId);
        }
    }

We implemented the **ITransientDependency** to simply register our hub via the
[dependency injection](/Pages/Documents/Dependency-Injection) system
(you can make it singleton based on your needs). We
[property-injected](/Pages/Documents/Dependency-Injection#property-injection-pattern)
the [session](/Pages/Documents/Abp-Session) and
[logger](/Pages/Documents/Logging).

**SendMessage** is a method of our hub that can be used by clients. We
call the **getMessage** function of **all** clients in this method. We can
use [AbpSession](/Pages/Documents/Abp-Session) to get the current user id
(if user logged in) as done above. We also made an override of **OnConnected** and
**OnDisconnected**, but for demonstration purposes only.

Here's the **client-side** JavaScript code to send/receive messages using
our hub.

    var chatHub = $.connection.myChatHub; // Get a reference to the hub

    chatHub.client.getMessage = function (message) { // Register for incoming messages
        console.log('received message: ' + message);
    };

    abp.event.on('abp.signalr.connected', function() { // Register to connect event
        chatHub.server.sendMessage("Hi everybody, I'm connected to the chat!"); // Send a message to the server
    });

We can then use the **chatHub** anytime we need to send a message to the
server. See the [SignalR documentation](http://www.asp.net/signalr) for
more information.
