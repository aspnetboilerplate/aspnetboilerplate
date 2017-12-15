### Introduction

[Abp.Web.SignalR](http://www.nuget.org/packages/Abp.Web.SignalR) NuGet
package makes it easily to use **SignalR** in ASP.NET Boilerplate-based
applications. See [SignalR documentation](http://www.asp.net/signalr)
for detailed information on SignalR.

### Installation

#### Server Side

Install
[**Abp.Web.SignalR**](http://www.nuget.org/packages/Abp.Web.SignalR)
NuGet package to your project (generally to your Web layer) and add a
**dependency** to your module:

    [DependsOn(typeof(AbpWebSignalRModule))]
    public class YourProjectWebModule : AbpModule
    {
        //...
    }


Then use **MapSignalR** method in your OWIN startup class as you always
do:

    [assembly: OwinStartup(typeof(Startup))]
    namespace MyProject.Web
    {
        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.MapSignalR();

                //...
            }
        }
    }

**Note:** Abp.Web.SignalR only depends on Microsoft.AspNet.SignalR.Core
package. So, you will also need to **install**
**[Microsoft.AspNet.SignalR](https://www.nuget.org/packages/Microsoft.AspNet.SignalR)**
package to your Web project if not installed before (See [SignalR
documents](http://www.asp.net/signalr) for more).

#### Client Side

**abp.signalr.js** script should be included to the page. It's located
in
**[Abp.Web.Resources](https://www.nuget.org/packages/Abp.Web.Resources)**
package (It's already installed in [startup templates](/Templates)). We
should include it after signalr hubs:

    <script src="~/signalr/hubs"></script>
    <script src="~/Abp/Framework/scripts/libs/abp.signalr.js"></script>


That's all. SignalR is properly configured and integrated to your
project.

### Connection Establishment

ASP.NET Boilerplate **automatically connects** to the server (from the
client) when **abp.signalr.js** is included to your page. This is
generally fine. But there may be cases you don't want it to. You can add
these lines just before including **abp.signalr.js** to disable auto
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
**Abp.Web.SignalR** package implements some built-in features.

#### Notification

**Abp.Web.SignalR** package implements **IRealTimeNotifier** to send
real time notifications to clients (see [notification
system](/Pages/Documents/Notification-System)). Thus, your users can get
real time push notifications.

#### Online Clients

ASP.NET Boilerplate provides **IOnlineClientManager** to get information
about online users (inject IOnlineClientManager and use
GetByUserIdOrNull, GetAllClients, IsOnline methods for example).
IOnlineClientManager needs a communication infrastructure to properly
work. **Abp.Web.SignalR** package provides the infrastructure. So, you
can inject and use IOnlineClientManager in any layer of your application
if SignalR is installed.

#### PascalCase vs. camelCase

Abp.Web.SignalR package overrides SignalR's default **ContractResolver**
to use **CamelCasePropertyNamesContractResolver** on serialization.
Thus, we can have classes have **PascalCase** properties on the server
and use them as **camelCase** on the client for sending/receiving
objects (because camelCase is preferred notation in JavaScript). If you
want to ignore this for your classes in some assemblies, then you can
add those assemblies to AbpSignalRContractResolver.**IgnoredAssemblies**
list.

### Your SignalR Code

**Abp.Web.SignalR** package also simplifies your SignalR code. Consider
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

We implemented **ITransientDependency** to simply register our hub to
[dependency injection](/Pages/Documents/Dependency-Injection) system
(you can make it singleton based on your needs). We
[property-injected](/Pages/Documents/Dependency-Injection#property-injection-pattern)
the [session](/Pages/Documents/Abp-Session) and
[logger](/Pages/Documents/Logging).

**SendMessage** is a method of our hub that can be used by clients. We
call **getMessage** function of **all** clients in this method. We can
use [AbpSession](/Pages/Documents/Abp-Session) to get current user id
(if user logged in) as done above. We also overrided **OnConnected** and
**OnDisconnected**, which is just for demonstration and not needed here
actually.

Here, the **client side** JavaScript code to send/receive messages using
our hub.

    var chatHub = $.connection.myChatHub; // Get a reference to the hub

    chatHub.client.getMessage = function (message) { // Register for incoming messages
        console.log('received message: ' + message);
    };

    abp.event.on('abp.signalr.connected', function() { // Register for connect event
        chatHub.server.sendMessage("Hi everybody, I'm connected to the chat!"); // Send a message to the server
    });

Then we can use **chatHub** anytime we need to send message to the
server. See [SignalR documentation](http://www.asp.net/signalr) for
detailed information on SignalR.
