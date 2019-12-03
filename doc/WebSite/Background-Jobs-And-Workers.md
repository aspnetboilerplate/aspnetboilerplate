### Introduction

ASP.NET Boilerplate provides background jobs and workers that are used
to execute some tasks in the **background threads** of an application.

### Background Jobs

In a **queued and persistent** manner, background jobs are used to queue some tasks to be executed in the
background. You may need background jobs for several reasons. Here are some examples:

-   To perform **long-running tasks** without having the users wait. For example, a
    user presses a 'report' button to start a long-running reporting
    job. You add this job to the **queue** and send the report's result to
    your user via email when it's completed.
-   To create **re-trying** and **persistent tasks** to **guarantee** code will be **successfully
    executed**. For example, you can send emails in a background job to
    overcome **temporary failures** and **guarantee** that it
    eventually will be sent. That way users do not wait while sending
    emails.

#### About Job Persistence

See the *Background Job Store* section for more information on job
persistence.

#### Create a Background Job

We can create a background job class by either inheriting from the
**BackgroundJob&lt;TArgs&gt;** class or by directly implementing the
**IBackgroundJob&lt;TArgs&gt; interface**.

Here is the most simple background job:

    public class TestJob : BackgroundJob<int>, ITransientDependency
    {
        public override void Execute(int number)
        {
            Logger.Debug(number.ToString());
        }
    }

A background job defines an **Execute** method that gets an input
**argument**. The argument **type** is defined as a **generic** class
parameter as shown in the example.

A background job must be registered via [dependency
injection](/Pages/Documents/Dependency-Injection). Implementing
**ITransientDependency** is the simplest way.

Let's define a more realistic job which sends emails in a background
queue:

    public class SimpleSendEmailJob : BackgroundJob<SimpleSendEmailJobArgs>, ITransientDependency
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IEmailSender _emailSender;
    
        public SimpleSendEmailJob(IRepository<User, long> userRepository, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
        }
    
        [UnitOfWork]
        public override void Execute(SimpleSendEmailJobArgs args)
        {
            var senderUser = _userRepository.Get(args.SenderUserId);
            var targetUser = _userRepository.Get(args.TargetUserId);
    
            _emailSender.Send(senderUser.EmailAddress, targetUser.EmailAddress, args.Subject, args.Body);
        }
    }

We
[injected](/Pages/Documents/Dependency-Injection#constructor-injection-pattern) the
user [repository](/Pages/Documents/Repositories) to get user emails,  
and injected the email sender (a service to send emails) and simply sent the email.
**SimpleSendEmailJobArgs** is the job argument here and defined as shown
below:

    [Serializable]
    public class SimpleSendEmailJobArgs
    {
        public long SenderUserId { get; set; }
    
        public long TargetUserId { get; set; }
    
        public string Subject { get; set; }
    
        public string Body { get; set; }
    }

A job argument should be **serializable**, because it's **serialized and
stored** in the database. While ASP.NET Boilerplate's default background
job manager uses **JSON** serialization (which does not need the
\[Serializable\] attribute), it's better to define the **\[Serializable\]**
attribute since we may switch to another job manager in the future, for which we may use
something like .NET's built-in binary serialization.

**Keep your arguments simple** (like
[DTO](Data-Transfer-Objects.md)s), do not include
[entities](/Pages/Documents/Entities) or other non-serializable objects.
As shown in the SimpleSendEmailJob sample, we can only store the **Id** of
an entity and get the entity from the repository inside the job.

#### Add a New Job To the Queue

After defining a background job, we can inject and use
**IBackgroundJobManager** to add a job to the queue. See this example for
TestJob as defined above:

    public class MyService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
    
        public MyService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }
    
        public void Test()
        {
            _backgroundJobManager.Enqueue<TestJob, int>(42);
        }
    }

We sent 42 as an argument while enqueuing. IBackgroundJobManager will
instantiate and execute the TestJob with 42 as the argument.

Let's add a new job for SimpleSendEmailJob, as we defined before:

    [AbpAuthorize]
    public class MyEmailAppService : ApplicationService, IMyEmailAppService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
    
        public MyEmailAppService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }
    
        public async Task SendEmail(SendEmailInput input)
        {
                await _backgroundJobManager.EnqueueAsync<SimpleSendEmailJob, SimpleSendEmailJobArgs>(
                new SimpleSendEmailJobArgs
                {
                    Subject = input.Subject,
                    Body = input.Body,
                    SenderUserId = AbpSession.GetUserId(),
                    TargetUserId = input.TargetUserId
                });
        }
    }

Enqueue (or EnqueueAsync) method has other parameters such as
**priority** and **delay**.

#### Default Background Job Manager

IBackgroundJobManager is implemented by **BackgroundJobManager**,
by default. It can be replaced by another background job provider (see
[hangfire integration](Hangfire-Integration.md)). Some information on the
default BackgroundJobManager:

-   It's a simple job queue that works as **FIFO** in a **single thread**. It
    uses **IBackgroundJobStore** to persist jobs (see the next section).
-   It **retries** job execution until the job **successfully runs** (if it does
    not throw any exceptions, but logs them) or **timeouts**. Default
    timeout is 2 days for a job.
-   It **deletes** a job from the store (database) when it's successfully
    executed. If it's timed out, it sets it as **abandoned**, and leaves it in the
    database.
-   It **increasingly waits between retries** for a job. It waits 1 minute
    for the first retry, 2 minutes for the second retry, 4 minutes for the third
    retry and so on.
-   It **polls** the store for jobs in fixed intervals. It queries jobs,
    ordering by priority (asc) and then by try count (asc).

##### Background Job Store

The default BackgroundJobManager needs a data store to save and get jobs. If
you do not implement **IBackgroundJobStore** then it uses
**InMemoryBackgroundJobStore** which does not save jobs in a persistent
database. You can simply implement it to store jobs in a database or you
can use **[Module Zero](/Pages/Documents/Zero/Overall)** which already
implements it.

If you are using a 3rd party job manager (like
[Hangfire](Hangfire-Integration.md)), there is no need to implement
IBackgroundJobStore.

#### Configuration

You can use **Configuration.BackgroundJobs** in the
[PreInitialize](/Pages/Documents/Module-System) method of your module to
configure the background job system.

##### Disabling Job Execution

You may want to disable background job execution for your application:

    public class MyProjectWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }
    
        //...
    }

This is rarely needed. An example of this is if you're running multiple
instances of your application working on the same database (in a web
farm). In this case, each application will query the same database for jobs
and execute them. This leads to multiple executions of the same jobs and
other problems. To prevent it, you have two options:

-   You can enable job execution for only one instance of the
    application.
-   You can disable job execution for all instances of the web
    application and create a separated, standalone application (example:
    a Windows Service) that executes background jobs.

#### User token removal period

ABP Framework defines a background worker named UserTokenExpirationWorker which cleans the records in table AbpUserTokens. If you disable background job execution, this worker will not run. By default, UserTokenExpirationWorker runs every one hour. If you want to change this period, you can configure it like below:

    public class MyProjectWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.CleanUserTokenPeriod = 1 * 60 * 60 * 1000; // 1 hour
        }
    
        //...
    }
#### Exception Handling

Since the default background job manager should re-try failed jobs, it
handles (and logs) all exceptions. In case you want to be informed when
an exception occurred, you can create an event handler to handle
[AbpHandledExceptionData](Handling-Exceptions.md). The background manager
triggers this event with a BackgroundJobException exception object which
wraps the real exception (get InnerException for the actual exception).

#### Hangfire Integration

The background job manager is designed to be **replaceable** by another
background job manager. See [hangfire integration
document](/Pages/Documents/Hangfire-Integration) to replace it with
[**Hangfire**](http://hangfire.io/).

### Background Workers

Background workers are different than background jobs. They are simple
**independent threads** in the application running in the background.
Generally, they run **periodically** to perform some tasks. Examples;

-   A background worker can run **periodically** to **delete old logs**.
-   A background worker can run **periodically** to **determine inactive users** and send emails to get users to return to your application.

#### Create a Background Worker

To create a background worker, we implement the **IBackgroundWorker**
interface. Alternatively, we can inherit from the **BackgroundWorkerBase**
or **PeriodicBackgroundWorkerBase** based on our needs.

Assume that we want to make a user passive, if he did not login to the
application in last 30 days. See the code:

    public class MakeInactiveUsersPassiveWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<User, long> _userRepository;
    
        public MakeInactiveUsersPassiveWorker(AbpTimer timer, IRepository<User, long> userRepository)
            : base(timer)
        {
            _userRepository = userRepository;
            Timer.Period = 5000; //5 seconds (good for tests, but normally will be more)
        }
    
        [UnitOfWork]
        protected override void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var oneMonthAgo = Clock.Now.Subtract(TimeSpan.FromDays(30));
    
                var inactiveUsers = _userRepository.GetAllList(u =>
                    u.IsActive &&
                    ((u.LastLoginTime < oneMonthAgo && u.LastLoginTime != null) || (u.CreationTime < oneMonthAgo && u.LastLoginTime == null))
                    );
    
                foreach (var inactiveUser in inactiveUsers)
                {
                    inactiveUser.IsActive = false;
                    Logger.Info(inactiveUser + " made passive since he/she did not login in last 30 days.");
                }
    
                CurrentUnitOfWork.SaveChanges();
            }
        }
    }

This real code directly works in ASP.NET Boilerplate with
[Module Zero](/Pages/Documents/Zero/Overall).

-   If you derive from **PeriodicBackgroundWorkerBase** (as in this
    sample), you should implement the **DoWork** method to perform your
    periodic working code.
-   If you derive from the **BackgroundWorkerBase** or directly implement
    **IBackgroundWorker**, you will override/implement the **Start**,
    **Stop** and **WaitToStop** methods. Start and Stop methods should
    be **non-blocking**, the WaitToStop method should **wait** for the worker to
    finish its current critical job.

#### Register Background Workers

After creating a background worker, add it to the
**IBackgroundWorkerManager**. The most common place is the PostInitialize
method of your module:

    public class MyProjectWebModule : AbpModule
    {
        //...
    
        public override void PostInitialize()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<MakeInactiveUsersPassiveWorker>());
        }
    }

While we generally add workers in PostInitialize, there are no
restrictions on that. You can inject IBackgroundWorkerManager anywhere
and add workers at runtime. IBackgroundWorkerManager will stop and
release all registered workers when your application is being shut down.

#### Background Worker Lifestyles

Background workers are generally implemented as a **singleton**, but there
are no restrictions to this. If you need multiple instances of the same worker class,
you can make it transient and add more than one instance to the
IBackgroundWorkerManager. In this case, your worker will probably be
parametric (say that you have a single LogCleaner class but two
LogCleaner worker instances and they watch and clear different log folders).

#### Advanced Scheduling

ASP.NET Boilerplate's background worker systems are simple. It does not have a
schedule system, except for periodic running workers as demonstrated above.
If you need more advanced scheduling features, we suggest you
check out [Quartz](Quartz-Integration.md) or another library.

### Making Your Application Always Run

Background jobs and workers only work if your application is running.
An ASP.NET application **shuts down** by default if no request is
performed to the web application for a long period of time. So, if you host the
background job execution in your web application (this is the default
behavior), you should ensure that your web application is configured to always
be running. Otherwise, background jobs only work while your
application is in use.

There are some techniques to accomplish that. The most simple way is to make
periodic requests to your web application from an external application.
Thus, you can also check if your web application is up and running. The
[Hangfire
documentation](http://docs.hangfire.io/en/latest/deployment-to-production/making-aspnet-app-always-running.html)
explains some other ways to accomplish this.
