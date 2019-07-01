### Building Dynamic Web API Controllers

This document is for the ASP.NET Web API. If you're interested in ASP.NET
Core, see the [ASP.NET Core](AspNet-Core.md) documentation.

ASP.NET Boilerplate can automatically generate an **ASP.NET Web API layer**
for your **application layer**. Say that we have an [application
service](/Pages/Documents/Application-Services) as shown below:

    public interface ITaskAppService : IApplicationService
    {
        GetTasksOutput GetTasks(GetTasksInput input);
        void UpdateTask(UpdateTaskInput input);
        void CreateTask(CreateTaskInput input);
    }

Say that we also want to expose this service as a Web API Controller for clients.
ASP.NET Boilerplate can automatically and dynamically create a Web API
Controller for this application service with a single configuration line:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder.For<ITaskAppService>("tasksystem/task").Build();

Thats it! An api controller is created with the address
'**/api/services/tasksystem/task**' and all methods are now usable by
clients. This configuration should be made in the **Initialize** method
of your [module](Module-System.md).

**ITaskAppService** is the application service that we want to wrap with
an api controller. It is not restricted to application services but this
is the conventional and recommended way. "**tasksystem/task**" is the name
of the api controller with an arbitrary namespace. You should define at
least a one-level namespace, but you can define more deep namespaces like
"myCompany/myApplication/myNamespace1/myNamespace2/myServiceName".
'**/api/services/**' is the prefix for all dynamic web api controllers.
The address of the api controller will look like
'/api/services/tasksystem/task' and the GetTasks method's address will be
'/api/services/tasksystem/task/getTasks'. Method names are converted to
**camelCase** since it's conventional in the world of JavaScript.

#### ForAll Method

We may have many application services in an application and building api
controllers one by one may be a tedious and forgettable work.
DynamicApiControllerBuilder provides a method to build web api
controllers for all application services in one call. Example:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .ForAll<IApplicationService>(Assembly.GetAssembly(typeof(SimpleTaskSystemApplicationModule)), "tasksystem")
        .Build();

The ForAll method is generic and accepts an interface. The first parameter is an
assembly that has classes derived from the given interfaces. The second one is
the namespace prefix of services. Say that we have an ITaskAppService and
IPersonAppService in a given assembly. For this configuration, the services
will be '/api/services/**tasksystem/task**' and
'/api/services/**tasksystem/person**'. To calculate the service name,
use the 'Service' and 'AppService' postfixes, as well as the 'I' prefix, which is removed (only for
interfaces). The service name is also converted to camel case. If you don't
like this convention, there is a '**WithServiceName**' method that you
can use to determine names. There is also a **Where** method to filter
services. This can be useful if you want to skip the builds of some services.

#### Overriding ForAll

We can override the configuration after the ForAll method. Example:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .ForAll<IApplicationService>(Assembly.GetAssembly(typeof(SimpleTaskSystemApplicationModule)), "tasksystem")
        .Build();

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .For<ITaskAppService>("tasksystem/task")
        .ForMethod("CreateTask").DontCreateAction().Build();

In this code, we create dynamic web api controllers for all the application
services in a given assembly. We then overide the configuration for a single
application service, ITaskAppService, to ignore the CreateTask method.

##### ForMethods

We can use the **ForMethods** method to better adjust each method while
using the ForAll method. Example:

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

In this example, we used a custom attribute, MyIgnoreApiAttribute, to ignore a
dynamic web api controller's actions/methods when they are marked with it.

#### Http Verbs

By default, all methods are created as a **POST**. A client has to
send post requests in order to use the created web api actions. We can
change this behavior in different ways.

##### WithVerb Method

We can use WithVerb for a method like this:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .For<ITaskAppService>("tasksystem/task")
        .ForMethod("GetTasks").WithVerb(HttpVerb.Get)
        .Build();

##### HTTP Attributes

We can add the HttpGet, HttpPost, and other related attributes to methods in the service
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

In order to use these attributes, you need to add a reference to the
[Microsoft.AspNet.WebApi.Core](https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Core)
NuGet package to your project.

##### Naming Convention

Instead of declaring the HTTP attributes for every method, you can use the
**WithConventionalVerbs** method as shown below:

    Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
        .ForAll<IApplicationService>(Assembly.GetAssembly(typeof(SimpleTaskSystemApplicationModule)), "tasksystem")
        .WithConventionalVerbs()
        .Build();

In this case, HTTP verbs are determined by method name prefixes:

-   **Get**: Used if the method name starts with 'Get'.
-   **Put**: Used if the method name starts with 'Put' or 'Update'.
-   **Delete**: Used if the method name starts with 'Delete' or 'Remove'.
-   **Post**: Used if the method name starts with 'Post', 'Create' or
    'Insert'.
-   **Patch**: Used if the method name starts with 'Patch'.
-   Otherwise, **Post** is used **by default** as an HTTP verb.

We can override them for specific methods as described before.

#### API Explorer

All dynamic web api controllers are visible to the API explorer by default
(They are available in [Swagger](Swagger-UI-Integration.md), for
example). You can control this behaviour with the fluent
DynamicApiControllerBuilder API or using the RemoteService attribute defined
below.

#### RemoteService Attribute

You can also use the **RemoteService** attribute for any **interface** or
**method** definition to enable or disable (**IsEnabled**) the dynamic API or
API explorer settings (**IsMetadataEnabled**).

### Dynamic JavaScript Proxies

You can use the dynamically created web api controller via ajax in
JavaScript. ASP.NET Boilerplate also simplifies this by creating dynamic
JavaScript proxies for dynamic web api controllers. You can call a
dynamic web api controller's action from JavaScript like a function
call:

    abp.services.tasksystem.task.getTasks({
        state: 1
    }).done(function (result) {
        //use result.tasks here...
    });

JavaScript proxies are created dynamically. You should include the
dynamic script on your page before you use it:

    <script src="/api/AbpServiceProxies/GetAll" type="text/javascript"></script>

Service methods return a promise (See
[jQuery.Deferred](http://api.jquery.com/category/deferred-object/)). You
can register to the done, fail, then... callbacks. Inside, the Service methods use
[abp.ajax](/Pages/Documents/Javascript-API/AJAX). They handle
errors and show error messages if needed.

#### AJAX Parameters

You may want to pass custom ajax parameters to the proxy method. You can
pass them as a second argument as shown below:

    abp.services.tasksystem.task.createTask({
        assignedPersonId: 3,
        description: 'a new task description...'
    },{ //override jQuery's ajax parameters
        async: false,
        timeout: 30000
    }).done(function () {
        abp.notify.success('successfully created a task!');
    });

All the parameters of [jQuery.ajax](http://api.jquery.com/jQuery.ajax/) are
valid here.

In addition to standard jQuery.ajax parameters, you can add
**abpHandleError: false** to AJAX options in order to disable
messages displaying when errors occur.

#### Single Service Script

'/api/AbpServiceProxies/GetAll' generates all service proxies in one
file. You can also generate a single service proxy using
'/api/AbpServiceProxies/Get?name=*serviceName*' and by including the script
in the page as shown below:

    <script src="/api/AbpServiceProxies/Get?name=tasksystem/task" type="text/javascript"></script>

#### AngularJS Integration

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
                }).then(function(result) {
                    vm.tasks = result.data.tasks;
                });
            }
        ]);
    })();

We can inject a **service** using it's name (with a namespace). We
can call it's **functions** as regular JavaScript functions. Notice that
we registered to the **then** handler (instead of done) since it's similar to
what is in angular's **$http** service. ASP.NET Boilerplate uses the $http
service of AngularJS. If you want to pass the $http **configuration**, you
can pass a configuration object as the last parameter of the service
method.

To be able to use auto-generated services, you should include these needed
scripts on your page:

    <script src="~/Abp/Framework/scripts/libs/angularjs/abp.ng.js"></script>
    <script src="~/api/AbpServiceProxies/GetAll?type=angular"></script>

#### Minification

ASP.NET Boilerplate dynamic api, as well as localization javascript can also
be returned minifed, by setting the *minify** flag:

    <script src="~/api/AbpServiceProxies/GetAll?minify=true"></script>
    <script src="~/api/AbpServiceProxies/GetAll?type=angular&minify=true"></script>
	<script src="/api/AbpServiceProxies/Get?name=tasksystem/task&minify=true" type="text/javascript"></script>

Default minification value is **false**.

### Enable/Disable

If you used the **ForAll** method as defined above, then you can use
the **RemoteService** attribute to disable it for a service or for a method.
Use this attribute in the **service interface**, and not in the service's concrete
class!

### Wrapping Results

ASP.NET Boilerplate **wraps** the return values of dynamic Web API actions
using an **AjaxResponse** object. See the [ajax
documentation](/Pages/Documents/Javascript-API/AJAX) for more
information on wrapping. You can enable/disable wrapping **per
method** or **per application service**. See this example application
service:

    public interface ITestAppService : IApplicationService
    {
        [DontWrapResult]
        DoItOutput DoIt(DoItInput input);
    }

We disabled wrapping for the DoIt method. This property is declared
for **interfaces**, not implemented classes.

Unwrapping can be useful if you want greater control on exact return
values to the client. Disabling it may be especially needed while
working with **3rd party client-side libraries** which can not work with
ASP.NET Boilerplate's standard AjaxResponse. In this case, you should
also handle exceptions yourself since [exception
handling](Handling-Exceptions.md) will be disabled (DontWrapResult
attribute has WrapOnError properties that can be used to enable the handling
and wrapping for exceptions).

Note: Dynamic JavaScript proxies can understand if the result is unwrapped
and will run properly in either case.

### About Parameter Binding

ASP.NET Boilerplate creates Api Controllers on runtime. ASP.NET Web
API's [model and parameter
binding](http://www.asp.net/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api)
is used to bind models and parameters. You can read the following
[documentation](http://www.asp.net/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api)
for more information.

#### FormUri and FormBody Attributes

**FromUri** and **FromBody** attributes can be used in the service interface
for advanced control binding.

#### DTOs vs Primitive Types

We strongly advise you to use
[DTO](http://www.aspnetboilerplate.com/Pages/Documents/Data-Transfer-Objects)s
as method parameters for application services and web api controllers.
You can also use primitive types like string, int, bool... or
nullable types like int?, bool?... as service arguments. More than one
parameter can be used but only one complex-type parameter is allowed in
these parameters.  This is because of restrictions in the ASP.NET Web API.
