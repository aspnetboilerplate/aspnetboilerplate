### Introduction

We are creating different applications based on different needs. But
implementing common and similar structures over and over again, at least
in some level. **Authorization**, **Validation**, **Exception
Handling**, **Logging**, **Localization**, **Database Connection
Management**, **Setting Management**, **Audit Logging** are some of
these common structures. Also, we are building **architectural
structures** and **best practices** like **Layered** and **Modular**
Architecture, **Domain Driven Design**, **Dependency Injection** and so
on. And trying to develop applications based on some **conventions**.

Since all of these are very time-consuming and hard to build seperately
for every project, many companies create private **frameworks**. They're
developing new applications faster with less bugs using these
frameworks. Surely, not all companies that lucky. Most of them have no
**time**, **budget** and **team** to develop such frameworks. Even they
have possibility to create a framework, it's hard to **document**,
**train developers** and **maintain** it.

ASP.NET Boilerplate (ABP) is an **open source and well documented
application framework** started idea of "developing a **common**
framework for all companies and all developers!" It's not just a
framework but also provides a strong **architectural model** based on
**Domain Driven Design** and **best practices** in mind.

### A Quick Sample

Let's investigate a simple class to see ABP's benefits:

    public class TaskAppService : ApplicationService, ITaskAppService
    {
        private readonly IRepository<Task> _taskRepository;

        public TaskAppService(IRepository<Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [AbpAuthorize(MyPermissions.UpdatingTasks)]
        public async Task UpdateTask(UpdateTaskInput input)
        {
            Logger.Info("Updating a task for input: " + input);

            var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
            if (task == null)
            {
                throw new UserFriendlyException(L("CouldNotFoundTheTaskMessage"));
            }

            input.MapTo(task);
        }
    }

Here, we see a sample [Application
Service](/Pages/Documents/Application-Services) method. An application
service, in DDD, is directly used by presentation layer to perform **use
cases** of the application. We can think that **UpdateTask** method is
called by javascript via AJAX. Let's see ABP's some benefits here:

-   **[Dependency Injection](/Pages/Documents/Dependency-Injection)** :
    ABP uses and provides a strong and conventional DI infrastructure.
    Since this class is an application service, it's conventionally
    registered to DI container as transient (created per request). It
    can simply inject all dependencies (as IRepository&lt;Task&gt; in
    this sample).
-   **[Repository](/Pages/Documents/Repositories)** : ABP can create a
    default repository for each entity (as IRepository&lt;Task&gt; in
    this example). Default repository has many useful methods as
    FirstOrDefault used in this example. We can easily extend default
    repository upon our needs. Repositories abstracts DBMS and ORMs and
    simplifies data access logic.
-   **[Authorization](/Pages/Documents/Authorization)** : ABP can check
    permissions. It prevents access to UpdateTask method if current user
    has no "updating task" permission or not logged in. It simplifies
    authorization using declarative attributes but also has additional
    ways of authorization.
-   **[Validation](/Pages/Documents/Validating-Data-Transfer-Objects)**
    : ABP automatically checks if input is null. It also validates all
    properties of an input based on standard data annotation attributes
    and custom validation rules. If request is not valid, it throws a
    proper validation exception.
-   **[Audit Logging](/Pages/Documents/Audit-Logging)** : User, browser,
    IP address, calling service, method, parameters, calling time,
    execution duration and some other informations are automatically
    saved for each request based on conventions and configurations.
-   [**Unit Of Work**](/Pages/Documents/Unit-Of-Work): In ABP, each
    application service method is assumed as a unit of work as default.
    It automatically creates a connection and begins a transaction at
    the beggining of the method. If the method successfully completed
    without exception, then the transaction is commited and connection
    is disposed. Even this method uses different repositories or
    methods, all of them will be atomic (transactional). And all changes
    on entities are automatically saved when transaction is commited.
    Thus, we don't even need to call \_repository.Update(task) method as
    shown here.
-   [**Exception Handling**](/Pages/Documents/Handling-Exceptions): We
    almost never handle exceptions in ABP in a web application. All
    exceptions are automatically handled by default. If an exception
    occurs, ABP automatically logs it and returns a proper result to the
    client. For example, if this is an AJAX request, the it returns a
    JSON to client indicates that an error occured. If hides actual
    exception from client unless the exception is a
    UserFriendlyException as used in this sample. It also understands
    and handles errors on client side and show appropriate messages to
    users.
-   **[Logging](/Pages/Documents/Logging)** : As you see, we can write
    logs using the Logger object defined in base class. Log4Net is used
    as default but it's changable or configurable.
-   **[Localization](/Pages/Documents/Localization)** : Notice that we
    used L method while throwing exception. Thus, it's automatically
    localized based on current user's culture. Surely, we're defining
    CouldNotFoundTheTask<span lang="tr">Message </span>in somewhere (see
    [localization](/Pages/Documents/Localization) document for more).
-   **[Auto Mapping](/Pages/Documents/Data-Transfer-Objects)** : In the
    last line, we're using ABP's MapTo extension method to map input
    properties to entity properties. It uses AutoMapper library to
    perform mapping. Thus, we can easily map properties from one object
    to another based on naming conventions.
-   **[Dynamic Web API Layer](/Pages/Documents/Dynamic-Web-API)** :
    TaskAppService is a simple class actually (even no need to deliver
    from ApplicationService). We generally write a wrapper Web API
    Controller to expose methods to javascript clients. ABP
    automatically does that on runtime. Thus, we can use application
    service methods directly from clients.
-   **[Dynamic Javascript AJAX
    Proxy](/Pages/Documents/Dynamic-Web-API#DocDynamicProxy)** : ABP
    creates javascript proxy methods those make calling application
    service methods just as simple as calling javascript methods on the
    client.

We can see benefit of ABP in such a simple class. All these tasks
normally take significiant time, but all they are automatically handled
by ABP.

### What Else

Beside this simple example, ABP provides a strong infrastructure and
application model. Here, some other features of ABP:

-   **[Modularity](/Pages/Documents/Module-System)** : Provides a strong
    infrastructure to build reusable modules.
-   **[Data Filters](/Pages/Documents/Data-Filters)** : Provides
    automatic data filtering to implement some patterns like soft-delete
    and multi-tenancy.
-   **[Multi Tenancy](Multi-Tenancy.md)**: It fully supports
    multi-tenancy, including single database or database per tenant
    architectures.
-   **[Setting Management](/Pages/Documents/Setting-Management)** :
    Provides a strong infrastructure to get/change application, tenant
    and user level settings.
-   **Unit & Integration Testing**: It's built testability in mind. Also
    provides base classes to simplify unit & integration tests. See
    [this
    article](http://www.codeproject.com/Articles/871786/Unit-testing-in-Csharp-using-xUnit-Entity-Framewor)
    for more information.

For all features, see [documentation](/Pages/Documents).

### Startup Templates

Starting a new solution, creating layers, installing nuget packages,
creating a simple layout and a menu... all these are also time consuming
stuff.

ABP provides pre-built [startup
templates](http://www.aspnetboilerplate.com/Templates) that makes
starting a new solution is much more easier. Templates support **SPA**
(Single-Page Application) and **MPA** (Multi-Page MVC Applications)
architectures. Also, allows us to use different ORMs.

### How To Use

ABP is developed on **[Github](https://github.com/aspnetboilerplate)**
and distributed on **[Nuget](/Pages/Documents/Nuget-Packages)** .
Easiest way of starting with ABP is creating a [startup
template](http://www.aspnetboilerplate.com/Templates) and following the
[documentation](/Pages/Documents).
