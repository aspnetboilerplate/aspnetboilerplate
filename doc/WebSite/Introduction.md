### Introduction

Different applications are created based on different needs. At some level,
they implement common and similar structures over and over again. 
**Authorization**, **Validation**, **Exception Handling**,
**Logging**, **Localization**, **Database Connection Management**,
**Setting Management**, and **Audit Logging** are some of
these common structures. They are built with **Architectural structures**
using **best practices** such as **Layered** and **Modular**
Architecture, **Domain Driven Design**, **Dependency Injection** and others. 
These applications are developed based on **conventions**.

Since all of these are very time-consuming and hard to build seperately
and for every project, many companies create private **frameworks**. Using
these framworks, they can develop new applications faster and with fewer bugs. 
Not all companies are this lucky. Most of them have no
**time**, **budget** or **team** to develop such frameworks. Even if they
have the ability to create such a framework, it's hard to **document**,
**train developers** and to **maintain** it.

ASP.NET Boilerplate (ABP) is an **open source and well-documented application framework** 
which started with the idea of "developing a **common**
framework for all companies and all developers!" It's not just a
framework, it also provides a strong **architectural model** based on
**Domain Driven Design**, with all the **best practices** in mind.

### A Quick Sample

Let's investigate a simple class to see ABP's benefits:

    public class TaskAppService : ApplicationService, ITaskAppService
    {
        private readonly IRepository<Task> _taskRepository;

        public TaskAppService(IRepository<Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [AbpAuthorize(MyPermissions.UpdateTasks)]
        public async Task UpdateTask(UpdateTaskInput input)
        {
            Logger.Info("Updating a task for input: " + input);

            var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
            if (task == null)
            {
                throw new UserFriendlyException(L("CouldNotFindTheTaskMessage"));
            }

            input.MapTo(task);
        }
    }

Here we see a sample [Application
Service](/Pages/Documents/Application-Services) method. An application
service, in DDD, is directly used by the presentation layer to perform the **use
cases** of the application. Think of **UpdateTask** as a method that is
called by JavaScript via AJAX. 

Let's see some of ABP's benefits here:

-   **[Dependency Injection](/Pages/Documents/Dependency-Injection)** :
    ABP uses and provides a strong and conventional DI infrastructure.
    Since this class is an application service, it's conventionally
    registered to the DI container as transient (created per request). It
    can simply inject any dependencies (such as the IRepository&lt;Task&gt; in
    this sample).
-   **[Repository](/Pages/Documents/Repositories)** : ABP can create a
    default repository for each entity (such as IRepository&lt;Task&gt; in
    this example). The default repository has many useful methods such as the
    FirstOrDefault method used in this example. We can easily extend the default
    repository to suit our needs. Repositories abstract the DBMS and ORMs and
    simplify the data access logic.
-   **[Authorization](/Pages/Documents/Authorization)** : ABP can check
    permissions. It prevents access to the UpdateTask method if the current user
    has no "update tasks" permission or is not logged in. ABP not only uses declarative 
    attributes, but it also has additional ways in which you can authorize.
-   **[Validation](/Pages/Documents/Validating-Data-Transfer-Objects)**
    : ABP automatically checks if the input is null. It also validates all
    the properties of an input based on standard data annotation attributes
    and custom validation rules. If a request is not valid, it throws a
    proper validation exception.
-   **[Audit Logging](/Pages/Documents/Audit-Logging)** : User, browser,
    IP address, calling service, method, parameters, calling time,
    execution duration and some other information is automatically
    saved for each request based on conventions and configurations.
-   [**Unit Of Work**](/Pages/Documents/Unit-Of-Work): In ABP, each
    application service method is assumed to be a unit of work by default.
    It automatically creates a connection and begins a transaction at
    the beggining of the method. If the method successfully completes
    without an exception, then the transaction is committed and the connection
    is disposed. Even if this method uses different repositories or
    methods, all of them will be atomic (transactional). All changes
    on entities are automatically saved when a transaction is commited.
    We don't even need to call the \_repository.Update(task) method as
    shown above.
-   [**Exception Handling**](/Pages/Documents/Handling-Exceptions): We
    almost never have to manually handle exceptions in ABP on a web application. 
    All exceptions are automatically handled by default! If an exception
    occurs, ABP automatically logs it and returns a proper result to the
    client. For example, if this is an AJAX request, it returns a
    JSON object to the client indicating that an error occured. It hides the actual
    exception from the client unless the exception is a
    UserFriendlyException, as used in this sample. It also understands
    and handles errors on the client side and show appropriate messages to the
    users.
-   **[Logging](/Pages/Documents/Logging)** : As you see, we can write
    logs using the Logger object defined in the base class. Log4Net is used
    by default, but it's changable and configurable.
-   **[Localization](/Pages/Documents/Localization)** : Note that we
    used the 'L' method while throwing the exception? This way, it's automatically
    localized based on the current user's culture. We define
    **CouldNotFindTheTaskMessage** elsewhere (see the
    [localization](/Pages/Documents/Localization) document for more info).
-   **[Auto Mapping](/Pages/Documents/Data-Transfer-Objects)** : In the
    last line, we're using ABP's MapTo extension method to map input
    properties to entity properties. It uses the AutoMapper library to
    perform mapping. We can easily map properties from one object
    to another based on naming conventions.
-   **[Dynamic Web API Layer](/Pages/Documents/Dynamic-Web-API)** :
    TaskAppService is a simple class, actually. We generally have to write a wrapper Web API
    Controller to expose methods to JavaScript clients, but ABP
    automatically does that on runtime. This way, we can use application
    service methods directly from clients.
-   **[Dynamic Javascript AJAX
    Proxy](/Pages/Documents/Dynamic-Web-API#dynamic-javascript-proxies)** : ABP
    creates proxy methods that make calling application
    service methods as simple as calling JavaScript methods on the
    client.

We can see the benefits of ABP in this simple class. All these tasks
normally take significiant time, but are automatically handled
by ABP.

### What Else

Besides this simple example, ABP provides a strong infrastructure and
application model. Here are some other features of ABP:

-   **[Modularity](/Pages/Documents/Module-System)** : Provides a strong
    infrastructure to build reusable modules.
-   **[Data Filters](/Pages/Documents/Data-Filters)** : Provides
    automatic data filtering to implement some patterns like soft-delete
    and multi-tenancy.
-   **[Multi Tenancy](Multi-Tenancy.md)**: It fully supports
    multi-tenancy, including single database or database per tenant
    architectures.
-   **[Setting Management](/Pages/Documents/Setting-Management)** :
    Provides a strong infrastructure to get/change the application, tenant
    and user-level settings.
-   **Unit & Integration Testing**: ABP is built with testability in mind. It also
    provides base classes to simplify unit & integration tests. See
    [this article](http://www.codeproject.com/Articles/871786/Unit-testing-in-Csharp-using-xUnit-Entity-Framewor)
    for more information.

For all the features, see the [documentation](/Pages/Documents).

### Startup Templates

Starting a new solution, creating layers, installing nuget packages,
creating a simple layout and a menu... all these are very time consuming.

ABP provides pre-built [startup
templates](http://www.aspnetboilerplate.com/Templates) that make
starting a new solution much simpler. The templates support the **SPA**
(Single-Page Application) and **MPA** (Multi-Page MVC Applications)
architectures. It also allows us to use different ORMs.

### How To Use ABP

ABP is developed on **[Github](https://github.com/aspnetboilerplate)**
and distributed via **[Nuget](/Pages/Documents/Nuget-Packages)**.
The easiest way of starting with ABP is by creating a [startup
template](http://www.aspnetboilerplate.com/Templates) and by following the
[documentation](/Pages/Documents).
