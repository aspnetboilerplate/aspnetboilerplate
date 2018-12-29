### Server Side

ASP.NET Boilerplate uses Castle Windsor's [logging
facility](http://docs.castleproject.org/Windsor.Logging-Facility.ashx).
It can work with different logging libraries: **Log4Net**, **NLog**,
**Serilog**, and more. Castle provides a **common interface** for all
logger libraries. This way, you're independent from a specific logging library
and can easily change it later if needed.

**[Log4Net](http://logging.apache.org/log4net/)** is one of the most
popular logging libraries for .NET. The ASP.NET Boilerplate
[templates](/Templates) come with Log4Net properly configured and ready to use.
There is just a single-line of code for the dependency to log4net (as
seen in the [configuration](#config) section), so you can easily change it to
your favourite library.

#### Getting The Logger

No matter which logging library you choose, the code to write logs is the
same (thanks to Castle's common ILogger interface).

First, we need to get a Logger object to write logs. Since ASP.NET
Boilerplate strongly uses [dependency
injection](/Pages/Documents/Dependency-Injection), we can easily inject
a **Logger** object using the [property
injection](/Pages/Documents/Dependency-Injection#property-injection-pattern)
(or constructor injection) pattern. Here's a sample class that writes a log
line:

    using Castle.Core.Logging; //1: Import Logging namespace

    public class TaskAppService : ITaskAppService
    {    
        //2: Getting a logger using property injection
        public ILogger Logger { get; set; }

        public TaskAppService()
        {
            //3: Do not write logs if no Logger supplied.
            Logger = NullLogger.Instance;
        }

        public void CreateTask(CreateTaskInput input)
        {
            //4: Write logs
            Logger.Info("Creating a new task with description: " + input.Description);

            //TODO: save task to database...
        }
    }

**First**, we imported the namespace of Castle's ILogger interface.

**Secondly**, we defined a public **ILogger** object named Logger. This is
the object we will write logs with. The dependency injection system will set
(inject) this property after creating the TaskAppService object. This is
known as the property injection pattern.

**Thirdly**, we set Logger to **NullLogger.Instance**. The system will work
fine without this line, but it is best practice to use the property injection
pattern. If no one sets the Logger, it will be **null** and we will get an
"object reference..." exception when we want to use it. This guarantees
that it's not null. So if no one sets the Logger, it will be
NullLogger. This is known as the null object pattern. NullLogger actually
does nothing. It does not write any logs. This way, our class can work with and
without an actual logger.

**Finally**, we're writing a log text with the **info** level.
There are different levels (see the [configuration](#config) section).

If we call the CreateTask method and check the log file, we see a log
line like the one shown below:

    INFO  2014-07-13 13:40:23,360 [8    ] SimpleTaskSystem.Tasks.TaskAppService    - Creating a new task with description: Remember to drink milk before sleeping!

#### Base Classes With Logger

ASP.NET Boilerplate provides **base classes** for MVC controllers, Web
API controllers, [Application
service](/Pages/Documents/Application-Services) classes and more. They
declare a **Logger** property. This way, you can directly use this Logger to
write logs, with no injection needed. Example:

    public class HomeController : SimpleTaskSystemControllerBase
    {
        public ActionResult Index()
        {
            Logger.Debug("A sample log message...");
            return View();
        }
    }

Note that SimpleTaskSystemControllerBase is our application specific
base controller that inherits **AbpController.** This way, it can directly
use the Logger. You can also write your own common base class for other
classes. You will then not have to inject a logger each time.

#### Configuration

All the configuration is done for Log4Net when you create your application
from the ASP.NET Boilerplate [templates](/Templates).

The default configuration's log format is as shown below (for each line):

-   **Log level**: DEBUG, INFO, WARN, ERROR or FATAL.
-   **Date and time**: The time when the log line was written.
-   **Thread number**: The thread number that wrote the log line.
-   **Logger name**: This is generally the class name which writes the
    log line.
-   **Log text**: Actual log text that you write.

It's defined in the **log4net.config** file of the application as shown
below:

    <?xml version="1.0" encoding="utf-8" ?>
    <log4net>
      <appender name="RollingFileAppender" type="log4net.Appender.RollingFileAppender" >
        <file value="Logs/Logs.txt" />
        <appendToFile value="true" />
        <rollingStyle value="Size" />
        <maxSizeRollBackups value="10" />
        <maximumFileSize value="10000KB" />
        <staticLogFileName value="true" />
        <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%-5level %date [%-5.5thread] %-40.40logger - %message%newline" />
        </layout>
      </appender>
      <root>
        <appender-ref ref="RollingFileAppender" />
        <level value="DEBUG" />
      </root>
      <logger name="NHibernate">
        <level value="WARN" />
      </logger>
    </log4net>

Log4Net is highly configurable and is a strong logging library. You can write
logs in different formats and to different targets (text file,
database...). You can set minimum log levels (as set for NHibernate in
this configuration). You can write different loggers to different log
files. It can automatically backup and create a new log file when it
reaches a specific size (RollingFileAppender with 10000 KB per file
in this configuration) and so on... Read its own configuration
[documentation](http://logging.apache.org/log4net/release/config-examples.html)
for more info.

Finally, in the Global.asax file, we declare that we use Log4Net with the
log4net.config file:

    public class MvcApplication : AbpWebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            IocManager.Instance.IocContainer.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));
            base.Application_Start(sender, e);
        }
    }

This is **the only code line we directly depend on for log4net**. Only
the web project depends on log4net library's [nuget
package](https://www.nuget.org/packages/log4net/). You can also easily
change to another library without changing your logging code.

#### Abp.Castle.Log4Net Package

ABP uses the Castle Logging Facility for logging and it does not directly
depend on log4net, as declared above. There is, however, a problem with
Castle's Log4Net integration... It does not support the latest log4net. We
created a NuGet package,
[**Abp.Castle.Log4Net**](http://nuget.org/packages/Abp.Castle.Log4Net),
to solve this issue. After adding this package to our solution, all we
have to do is to change the code in the application start method like this:

    public class MvcApplication : AbpWebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            IocManager.Instance.IocContainer.AddFacility<LoggingFacility>(f => f.UseAbpLog4Net().WithConfig("log4net.config"));
            base.Application_Start(sender, e);
        }
    }

The only difference is that we used the "**UseAbpLog4Net()**" method
(defined in Abp.Castle.Logging.Log4Net namespace) instead of
"UseLog4Net()". When we use Abp.Castle.Log4Net package, you **do not
need** to use the
[Castle.Windsor-log4net](https://www.nuget.org/packages/Castle.Windsor-log4net)
and
[Castle.Core-log4net](https://www.nuget.org/packages/Castle.Core-log4net/)
packages.

If you need to change your log4Net configuration file on runtime and want to make changes take effect immediately without restarting the application you can use as follows:

    public class MvcApplication : AbpWebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.LogUsing(new Log4NetLoggerFactory("log4net.config", reloadOnChange:true))
            );
            base.Application_Start(sender, e);
        }
    }

### Client-Side

ASP.NET Boilerplate defines a simple JavaScript logging API for the client
side. It logs to the browser's console as default. Here's some JavaScript code to
write logs:

    abp.log.warn('a sample log message...');

For more information, see the [logging API
documentation](/Pages/Documents/Javascript-API/Logging).
