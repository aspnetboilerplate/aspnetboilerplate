### Building Dynamic Web API Controllers

This document is for ASP.NET Web API. If you're interested in ASP.NET
Core, see [ASP.NET Core](AspNet-Core.html) documentation.

ASP.NET Boilerplate can automatically generate **ASP.NET Web API layer**
for your **application layer**. Say that we have an [application
service](/Pages/Documents/Application-Services) as shown below:

    public interface ITaskAppService : IApplicationService
    {
        GetTasksOutput GetTasks(GetTasksInput input);
        void UpdateTask(UpdateTaskInput input);
        void CreateTask(CreateTaskInput input);
    }

And we want to expose this service as a Web API Controller for clients.
ASP.NET Boilerplate can automatically and dynamically create a Web API
Controller for this application service with a single line of
configuration:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder.For<ITaskAppService>("tasksystem/task").Build();

Thats all! An api controller is created in the address
'**/api/services/tasksystem/task**' and all methods are now usable by
clients. This configuration should be made in the **Initialize** method
of your [module](Module-System.html).

**ITaskAppService** is the application service that we want to wrap with
an api controller. It is not restricted to application services but this
is the conventional and recommended way. "**tasksystem/task**" is name
of the api controller with an arbitrary namespace. You should define at
least one-level namespace but you can define more deep namespaces like
"myCompany/myApplication/myNamespace1/myNamespace2/myServiceName".
'**/api/services/**' is a prefix for all dynamic web api controllers.
So, address of the api controller will be like
'/api/services/tasksystem/task' and GetTasks method's address will be
'/api/services/tasksystem/task/getTasks'. Method names are converted to
**camelCase** since it's conventional in javascript world.

#### ForAll Method

We may have many application services in an application and building api
controllers one by one may be a tedious and forgettable work.
DynamicApiControllerBuilper provides a method to build web api
controllers for all application services in one call. Example:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .ForAll<IApplicationService>(Assembly.GetAssembly(typeof(SimpleTaskSystemApplicationModule)), "tasksystem")
        .Build();

ForAll method is generic and accepts an interface. First parameter is an
assembly that has classes derived from given interface. Second one is
the namespace prefix of services. Say that we have ITaskAppService and
IPersonAppService in given assembly. For this configuration, services
will be '/api/services/**tasksystem/task**' and
'/api/services/**tasksystem/person**'. To calculate service name:
Service and AppService postfixes and I prefix is removed (for
interfaces). Also, service name is converted to camel case. If you don't
like this convention, there is a '**WithServiceName**' method that you
can determine names. Also, There is a **Where** method to filter
services. This can be useful if you will build for all application
services except a few one.

#### Overriding ForAll

We can override configuration after ForAll method. Example:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .ForAll<IApplicationService>(Assembly.GetAssembly(typeof(SimpleTaskSystemApplicationModule)), "tasksystem")
        .Build();

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .For<ITaskAppService>("tasksystem/task")
        .ForMethod("CreateTask").DontCreateAction().Build();

In this code, we created dynamic web api controllers for all application
services in an assembly. Then overrided configuration for a single
application service (ITaskAppService) to ignore CreateTask method.

##### ForMethods

We can use **ForMethods** method to better adjust each method while
using ForAll method. Example:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .ForAll<IApplicationService>(Assembly.GetExecutingAssembly(), "app")
        .ForMethods(builder =>
        {
            if (builder.Method.IsDefined(typeof(MyIgnoreApiAttribute)))
            {
                builder.DontCreate = true;
            }
        })
        .Build();

In this example, I used a custom attribute (MyIgnoreApiAttribute) to
check for all methods and don't create dynamic web api controller
actions for those methods marked with that attribute.

#### Http Verbs

By default, all methods are created as **POST**. So, a client should
send post requests in order to use created web api actions. We can
change this behaviour in different ways.

##### WithVerb Method

We can use WithVerb for a method like that:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .For<ITaskAppService>("tasksystem/task")
        .ForMethod("GetTasks").WithVerb(HttpVerb.Get)
        .Build();

##### HTTP Attributes

We can add HttpGet, HttpPost... attributes to methods in the service
interface:

    public interface ITaskAppService : IApplicationService
    {
        [HttpGet]
        GetTasksOutput GetTasks(GetTasksInput input);

        [HttpPut]
        void UpdateTask(UpdateTaskInput input);

        [HttpPost]
        void CreateTask(CreateTaskInput input);
    }

In order to use these attributes, we should add reference to
[Microsoft.AspNet.WebApi.Core](https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Core)
nuget package from your project.

##### Naming Convention

Instead of declaring HTTP very for every method, you can use
**WithConventionalVerbs** method as shown below:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .ForAll<IApplicationService>(Assembly.GetAssembly(typeof(SimpleTaskSystemApplicationModule)), "tasksystem")
        .WithConventionalVerbs()
        .Build();

In this case, HTTP verbs are determined by method name prefixes:

-   **Get**: Used if method name starts with 'Get'.
-   **Put**: Used if method name starts with 'Put' or 'Update'.
-   **Delete**: Used if method name starts with 'Delete' or 'Remove'.
-   **Post**: Used if method name starts with 'Post', 'Create' or
    'Insert'.
-   **Patch**: Used if method name starts with 'Patch'.
-   Otherwise, **Post** is used **as default** HTTP verb.

We can override it for a specific method as described before.

#### API Explorer

All dynamic web api controllers are visible to API explorer by default
(They are available in [Swagger](Swagger-UI-Integration.html) for
example). You can control this behaviour with fluent
DynamicApiControllerBuilder API or using RemoteService attribute defined
below.

#### RemoteService Attribute

You can also use **RemoteService** attribute for any **interface** or
**method** definition to enable/disable (**IsEnabled**) dynamic API or
API explorer setting (**IsMetadataEnabled**).

### Dynamic Javascript Proxies

You can use the dynamically created web api controller via ajax in
javascript. ASP.NET Boilerplate also simplifies this by creating dynamic
javascript proxies for dynamic web api controllers. So, you can call a
dynamic web api controller's action from javascript as like a function
call:

    abp.services.tasksystem.task.getTasks({
        state: 1
    }).done(function (result) {
        //use result.tasks here...
    });

Javascript proxies are created dynamically. You should include the
dynamic script to your page before use it:

    <script src="/api/AbpServiceProxies/GetAll" type="text/javascript"></script>

Service methods return promise (See
[jQuery.Deferred](http://api.jquery.com/category/deferred-object/)). You
can register to done, fail, then... callbacks. Service methods use
[abp.ajax](/Pages/Documents/Javascript-API/AJAX) inside. They handle
errors and show error messages if needed.

#### AJAX Parameters

You may want to pass custom ajax parameters to the proxy method. You can
pass them as second argument as sown below:

    abp.services.tasksystem.task.createTask({
        assignedPersonId: 3,
        description: 'a new task description...'
    },{ //override jQuery's ajax parameters
        async: false,
        timeout: 30000
    }).done(function () {
        abp.notify.success('successfully created a task!');
    });

All parameters of [jQuery.ajax](http://api.jquery.com/jQuery.ajax/) are
valid here.

In addition to standard jQuery.ajax parameters, you can add
**abpHandleError: false** to AJAX options in order to disable automatic
message displaying on error cases.

#### Single Service Script

'/api/AbpServiceProxies/GetAll' generates all service proxies in one
file. You can also generate a single service proxy using
'/api/AbpServiceProxies/Get?name=*serviceName*' and include the script
to the page as shown below:

    <script src="/api/AbpServiceProxies/Get?name=tasksystem/task" type="text/javascript"></script>

#### Angular Integration

ASP.NET Boilerplate can expose dynamic api controllers as **angularjs
services**. Consider the sample below:

    (function() {
        angular.module('app').controller('TaskListController', [
            '$scope', 'abp.services.tasksystem.task',
            function($scope, taskService) {
                var vm = this;
                vm.tasks = [];
                taskService.getTasks({
                    state: 0
                }).success(function(result) {
                    vm.tasks = result.tasks;
                });
            }
        ]);
    })();

We can inject a **service** using it's name (with namespace). Then we
can call it's **functions** as regular javascript functions. Notice that
we registered to **success** handler (instead of done) since it's like
that in angular's **$http** service. ASP.NET Boilerplate uses $http
service of AngularJs. If you want to pass $http **configuration**, you
can pass a configuration object as the last parameter of the service
method.

To be able to use auto-generated services, you should include needed
scripts to your page:

    <script src="~/Abp/Framework/scripts/libs/angularjs/abp.ng.js"></script>
    <script src="~/api/AbpServiceProxies/GetAll?type=angular"></script>

### Enable/Disable

If you used **ForAll** method as defined above, the you can use
**RemoteService** attribute to disable it for a service or for method.
Use this attribute in the **service interface**, not in the service
class.

### Wrapping Results

ASP.NET Boilerplate **wraps** return values of dynamic Web API actions
by **AjaxResponse** object. See [ajax
documentation](/Pages/Documents/Javascript-API/AJAX) for more
information on this wrapping. You can enable/disable wrapping **per
method** or **per application service**. See this example application
service:

    public interface ITestAppService : IApplicationService
    {
        [DontWrapResult]
        DoItOutput DoIt(DoItInput input);
    }

We disabled wrapping for DoIt method. This properties should be declared
for **interfaces**, not implementation classes.

Unwrapping can be useful if you want to more control on exact return
values to the client. Especially, disabling it may be needed while
working **3rd party client side libraries** which can not work with
ASP.NET Boilerplate's standard AjaxResponse. In this case, you should
also handle exceptions yourself since [exception
handling](Handling-Exceptions.html) will be disabled (DontWrapResult
attribute has WrapOnError properties that can be used to enable handling
and wrapping for exceptions).

Note: Dynamic javascript proxies can understand if result is unwrapped
and run properly in either case.

### About Parameter Binding

ASP.NET Boilerplate creates Api Controllers on runtime. So, ASP.NET Web
API's [model and parameter
binding](http://www.asp.net/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api)
is used to bind model and parameters. You can read it's
[documentation](http://www.asp.net/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api)
for more information.

#### FormUri and FormBody Attributes

**FromUri** and **FromBody** attributes can be used in service interface
to advanced control on binding.

#### DTOs vs Primitive Types

We strongly advice to use
[DTO](http://www.aspnetboilerplate.com/Pages/Documents/Data-Transfer-Objects)s
as method parameters for application services and web api controllers.
But you can also use primitive types (like string, int, bool... or
nullable types like int?, bool?...) as service arguments. More than one
parameters can be used but only one complex-type parameter is allowed in
these parameters (because of restriction of ASP.NET Web API).
