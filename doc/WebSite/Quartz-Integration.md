### Introduction

[Quartz](http://www.quartz-scheduler.net/) is a full-featured, open-source job scheduling system that can be used from the smallest apps to large-scale enterprise systems.

The [Abp.Quartz](https://www.nuget.org/packages/Abp.Quartz) package simply integrates Quartz to ASP.NET Boilerplate.

ASP.NET Boilerplate has a built-in [persistent background job queue and
background worker](Background-Jobs-And-Workers.md) system. Quartz can
be a good alternative if you have advanced scheduling requirements for
your background workers. [Hangfire](Hangfire-Integration.md) can also
be a good alternative for persistent background job queues.

### Installation

Install the [**Abp.Quartz**](https://www.nuget.org/packages/Abp.Quartz)
NuGet package to your project and add a **DependsOn** attribute to your
[module](Module-System.md) for **AbpQuartzModule**:

    [DependsOn(typeof (AbpQuartzModule))]
    public class YourModule : AbpModule
    {
        //...
    }

### Creating Jobs

To create a new job, you can either implement Quartz's IJob interface,
or derive from the JobBase class (defined in the Abp.Quartz package) that has
some helper properties/methods for logging and localization, for
example. A simple job class is shown below:

    public class MyLogJob : JobBase, ITransientDependency
    {
        public override Task Execute(IJobExecutionContext context)
        {
            Logger.Info("Executed MyLogJob..!");
            return Task.CompletedTask;
        }
    }

We simply implemented the **Execute** method to write a log. You can see
Quartz's [documentation](http://www.quartz-scheduler.net/) for more info.

### Schedule Jobs

The **IQuartzScheduleJobManager** is used to schedule jobs. You can inject
it to your class (or you can Resolve and use it in your module's
PostInitialize method) to schedule jobs. An example Controller that
schedules a job:

    public class HomeController : AbpController
    {
        private readonly IQuartzScheduleJobManager _jobManager;
    
        public HomeController(IQuartzScheduleJobManager jobManager)
        {
            _jobManager = jobManager;
        }
            
        public async Task<ActionResult> ScheduleJob()
        {
            await _jobManager.ScheduleAsync<MyLogJob>(
                job =>
                {
                    job.WithIdentity("MyLogJobIdentity", "MyGroup")
                        .WithDescription("A job to simply write logs.");
                },
                trigger =>
                {
                    trigger.StartNow()
                        .WithSimpleSchedule(schedule =>
                        {
                            schedule.RepeatForever()
                                .WithIntervalInSeconds(5)
                                .Build();
                        });
                });
    
            return Content("OK, scheduled!");
        }
    }   

### More

Please see Quartz's [documentation](http://www.quartz-scheduler.net/) for more information.
