### Introduction

ASP.NET Boilerplate provides background jobs and workers those are used
to execute some tasks in **background threads** in an application.

### Background Jobs

Background jobs are used to queue some tasks to be executed in
background in a **queued and persistent** manner. You may need
background jobs for several reasons. Some examples:

-   To perform **long-running tasks** to not wait users. For example; A
    user presses a 'report' button to start a long running reporting
    job. You add this job to the **queue** and send report result to
    your user via email when it's completed.
-   To create **re-trying** and **persistent tasks** to **g**<span
    class="auto-style1">uarantee</span> a code will be **successfully
    executed**. For example; You can send emails in a background job to
    overcome **temporary failures** and **guarantie** that it's
    eventually will be sent. Also, thus, users do not wait while sending
    emails.

#### About Job Persistence

See *Background Job Store* section for more information on job
persistence.

#### Create a Background Job

We can create a background job class by either inheriting from
**BackgroundJob&lt;TArgs&gt;** class or directly implementing
**IBackgroundJob&lt;TArgs&gt; interface**.

Here is the most simple background job:

    public class TestJob : BackgroundJob<int>, ITransientDependency
    {
        public override void Execute(int number)
        {
            Logger.Debug(number.ToString());
        }
    }

A background job defines an **Execute** method gets an input
**argument**. Argument **type** is defined as **generic** class
parameter as shown in the example.

A background job must be registered to [dependency
injection](/Pages/Documents/Dependency-Injection). Implementing
**ITransientDependency** is the simplest way.

Lest's define a more realistic job which sends emails in a background
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
[injected](/Pages/Documents/Dependency-Injection#DocConstructorInjection)
user [repository](/Pages/Documents/Repositories) (to get user emails)
and email sender (a service to send emails) and simply sent the email.
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
stored** in the database. While ASP.NET Boilerplate default background
job manager uses **JSON** serialization (which does not need
\[Serializable\] attribute), it's better to define **\[Serializable\]**
attribute since we may switch to another job manager in the future,
which may use .NET's built-in binary serialization.

**Keep your arguments simple** (like
[DTO](Data-Transfer-Objects.html)s), do not include
[entities](/Pages/Documents/Entities) or other non serializable objects.
As shown in the SimpleSendEmailJob sample, we can only store **Id** of
an entity and get the entity from repository inside the job.

#### Add a New Job To the Queue

After defining a background job, we can inject and use
**IBackgroundJobManager** to add a job to the queue. See a sample for
TestJob defined above:

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

We sent 42 as argument while enqueuing. IBackgroundJobManager will
instantiate and execute the TestJob with 42 as argument.

Let's see to add a new job for SimpleSendEmailJob defined above:

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

Enqueu (or EnqueueAsync) method has other parameters such as
**priority** and **delay**.

#### Default Background Job Manager

IBackgroundJobManager is implemented by **BackgroundJobManager**
default. It can be replaced by another background job provider (see
[hangfire integration](Hangfire-Integration.html)). Some information on
default BackgroundJobManager:

-   It's a simple job queue works as **FIFO** in a **single thread**. It
    uses **IBackgroundJobStore** to persist jobs (see next section).
-   It **retries** job execution until job **successfully runs** (does
    not throw any exception but logs them) or **timeouts**. Default
    timeout is 2 days for a job.
-   It **deletes** a job from store (database) when it's successfully
    executed. If it's timed out, sets as **abandoned** and leaves on the
    database.
-   It **increasingly waits between retries** for a job. Waits 1 minute
    for first retry, 2 minutes for second retry, 4 minutes for third
    retry and so on..
-   It **polls** the store for jobs in fixed intervals. Queries jobs
    ordering by priority (asc) then by try count (asc).

##### Background Job Store

Default BackgroundJobManager needs a data store to save and get jobs. If
you do not implement **IBackgroundJobStore** then it uses
**InMemoryBackgroundJobStore** which does not save jobs in a persistent
database. You can simply implement it to store jobs in a database or you
can use **[module-zero](/Pages/Documents/Zero/Overall)** which already
implements it.

If you are using a 3rd party job manager (like
[Hanfgire](Hangfire-Integration.html)), no need to implement
IBackgroundJobStore.

#### Configuration

You can use **Configuration.BackgroundJobs** in
[PreInitialize](/Pages/Documents/Module-System) method of your module to
configure background job system.

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

This is rarely needed. But, think that you are running multiple
instances of your application working on the same database (in a web
farm). In this case, each application will query same database for jobs
and executes them. This leads multiple execution of same jobs and some
other problems. To prevent it, you have two options:

-   You can enable job execution for only one instance of the
    application.
-   You can disable job execution for all instances of the web
    application and create a seperated standalone application (example:
    a Windows Service) that executes background jobs.

#### Exception Handling

Since default background job manager should re-try failed jobs, it
handles (and logs) all exceptions. In case you want to be informed when
an exception occurred, you can create an event handler to handle
[AbpHandledExceptionData](Handling-Exceptions.html). Background manager
triggers this event with a BackgroundJobException exception object which
wraps the real exception (get InnerException for the actual exception).

#### Hangfire Integration

Background job manager is designed as **replaceable** by another
background job manager. See [hangfire integration
document](/Pages/Documents/Hangfire-Integration) to replace it by
[**Hangfire**](http://hangfire.io/).

### Background Workers

Background workers are different than background jobs. They are simple
**independent threads** in the application running in the background.
Generally, they run **periodically** to perform some tasks. Examples;

-   A background worker can run **periodically** to **delete old logs**.
-   A background worker can **periodically** to **determine inactive
    users** and send emails to return back to your application.

#### Create a Background Worker

To create a background worker, we should implement **IBackgroundWorker**
interface. Alternatively, we can inherit from **BackgroundWorkerBase**
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

This is a real code and directly works in ASP.NET Boilerplate with
[module-zero](/Pages/Documents/Zero/Overall).

-   If you derive from **PeriodicBackgroundWorkerBase** (as in this
    sample), you should implement **DoWork** method to perform your
    periodic working code.
-   If you derive from **BackgroundWorkerBase** or directly implement
    **IBackgroundWorke**r, you will override/implement **Start**,
    **Stop** and **WaitToStop** methods. Start and Stop methods should
    be **non-blocking**, WaitToStop method should **wait** worker to
    finish it's current critical job.

#### Register Background Workers

After creating a background worker, we should add it to
**IBackgroundWorkerManager**. Most common place is the PostInitialize
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

While we generally add workers in PostInitialize, there is no
restriction on that. You can inject IBackgroundWorkerManager anywhere
and add workers on runtime. IBackgroundWorkerManager will stop and
release all registered workers when your applications is being shutdown.

#### Background Worker Lifestyles

Background workers are generally implemented as **singleton**. But there
is no restriction. If you need multiple instance of same worker class,
you can make it transient and add more than one instance to
IBackgroundWorkerManager. In this case, your worker probably will be
parametric (say that you have a single LogCleaner class but two
LogCleaner worker instances they watch and clear different log folders).

#### Advanced Scheduling

ASP.NET Boilerplate's background worker system are simple. It has not a
schedule system, except periodic running workers as demonstrated above.
If you need to more advanced scheduling features, we suggest you to
check [Quartz](Quartz-Integration.html) or another library.

### Making Your Application Always Running

Background jobs and workers only works if your application is running.
An ASP.NET application **shutdowns** by default if no request is
performed to the web application for a long time. So, if you host
background job execution in your web application (this is the default
behaviour), you should ensure that your web application is configured to
always running. Otherwise, background jobs only works while your
applications is in use.

There are some techniques to accomplish that. Most simple way is to make
periodic requests to your web application from an external application.
Thus, you can also check if your web application is up and running.
[Hangfire
documentation](http://docs.hangfire.io/en/latest/deployment-to-production/making-aspnet-app-always-running.html)
explains some other ways.
