### Introduction

This document describes the ASP.NET Core integration for ASP.NET Boilerplate.
The ASP.NET Core integration is implemented in the
[Abp.AspNetCore](https://www.nuget.org/packages/Abp.AspNetCore) NuGet
package.

#### Migrating to ASP.NET Core?

If you have an existing project and are considering migrating to ASP.NET
Core, you can read our [blog
post](http://volosoft.com/migrating-from-asp-net-mvc-5x-to-asp-net-core/)
about our experience on migrating.

### Startup Template

You can create your project from a [startup template](/Templates), which
is a simple, empty web project. It is properly integrated and configured to
work with the ABP framework.

### Configuration

#### Startup Class

To integrate ABP to ASP.NET Core, we need to make some changes in the
Startup class:

    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //...

            //Configure Abp and Dependency Injection. Should be called last.
            return services.AddAbp<MyProjectWebModule>(options =>
            {
                //Configure Log4Net logging (optional)
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseLog4Net().WithConfig("log4net.config")
                );
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Initializes ABP framework and all modules. Should be called first.
            app.UseAbp();

            //...
        }
    }

#### Module Configuration

You can use the [startup configuration](Startup-Configuration.md) to
configure the AspNetCore Module by using
*Configuration.Modules.AbpAspNetCore()* in the PreInitialize method of your
module.

### Controllers

Controllers can be any type of class in ASP.NET Core and are not
restricted to classes derived from the Controller class. By default, if a class
ends with Controller (like ProductController), it's considered an MVC
Controller. You can also add MVC's \[Controller\] attribute to any class
to make it a controller. This is the way ASP.NET Core MVC handles things.
See the ASP.NET Core [documentation](https://docs.asp.net) for more info.

If you end up using the web layer classes (like HttpContext), or return a view,
it's better to inherit from **AbpController** which is derived from
MVC's Controller class. If you are creating an API controller that just
works with objects, consider creating a POCO controller class,
or use your application services as controllers as described below.

#### Application Services as Controllers

ASP.NET Boilerplate provides the infrastructure to create [application
services](Application-Services.md). If you want to expose your
application services to remote clients as controllers (as previously
done using [dynamic web api](Dynamic-Web-API.md)), you can easily do
that with a simple configuration in the [PreInitialize](Module-System.md)
method of your module. Example:

    Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(MyApplicationModule).Assembly, moduleName: 'app', useConventionalHttpVerbs: true);

The **CreateControllersForAppServices** method gets an assembly and converts
all the application services to MVC controllers in that assembly. You can
use the **RemoteService** attribute to enable or disable it for the class or it's methods.

When an application service is converted to an MVC Controller, it's default
route will look like this:
*/api/services/&lt;module-name&gt;/&lt;service-name&gt;/&lt;method-name&gt;*.
For example, if ProductAppService defines a Create method, it's URL will be
**/api/services/app/product/create** (assuming that the module name is
'app').

If **useConventionalHttpVerbs** is set to **true** (which is the **default
value**), then the HTTP verbs for the service methods are determined by the following **naming
conventions**:

-   **Get**: Used if the method name starts with 'Get'.
-   **Put**: Used if the method name starts with 'Put' or 'Update'.
-   **Delete**: Used if the method name starts with 'Delete' or 'Remove'.
-   **Post**: Used if the method name starts with 'Post', 'Create' or
    'Insert'.
-   **Patch**: Used if the method name starts with 'Patch'.
-   Otherwise, **Post** is used **by default** as an HTTP verb.

You can use any ASP.NET Core attributes to change the HTTP methods or routes
of the actions. This requires you to add a reference to the
Microsoft.AspNetCore.Mvc.Core package.

**Note**: Previously, the dynamic web api system required you to create
service **interfaces** for application services. This is not
required for the ASP.NET Core integration.  The MVC attributes should be
added to the service classes, even if you have interfaces.

**Note**: To use Mvc datetime format options, you can set this property `Configuration.Modules.AbpAspNetCore().UseMvcDateTimeFormatForAppServices`. Its default value is `false`. 

### Filters

ABP defines some **pre-built filters** for ASP.NET Core. All of them are
added to **all actions of all controllers** by default.

#### Authorization Filter

**AbpAuthorizationFilter** is used to integrate the [authorization system](Authorization.md) and [feature
system](Feature-Management.md).

-   Use the **AbpMvcAuthorize** attribute on actions or
    controllers to check the desired permissions before the action execution.
-   Use the the **RequiresFeature** attribute on actions or
    controllers to check for the desired features before the action execution.
-   Use the **AllowAnonymous** (or AbpAllowAnonymous in
    application layer) attribute on actions or controllers to suppress
    authentication/authorization.

#### Audit Action Filter

**AbpAuditActionFilter** is used to integrate with the [audit logging
system](Audit-Logging.md). If auditing is not disabled, it logs all requests
to all actions by default . You can control audit logging
by using the **Audited** and **DisableAuditing** attributes on actions and
controllers.

#### Validation Action Filter

**AbpValidationActionFilter** is used to integrate with the [validation
system](Validating-Data-Transfer-Objects.md). It automatically
validates all the inputs of all actions. In addition to ABP's built-in
validation & normalization, it also checks MVC's **Model.IsValid**
property and throws a validation exception if the action input values are invalid.

You can control validation using the **EnableValidation** and
**DisableValidation** attributes on actions and controllers.

#### Unit of Work Action Filter

**AbpUowActionFilter** integrates with the [Unit of
Work](Unit-Of-Work.md) system. It automatically begins a new unit of
work before an action execution, and if no exception is thrown, completes the unit of
work after the action execution.

You can use the **UnitOfWork** attribute to control the behaviour of the UOW for an
action. You can also use a startup configuration to change the default unit of
work attribute for all actions.

#### Exception Filter

**AbpExceptionFilter** is used to handle exceptions thrown from
controller actions. It **handles** and **logs** exceptions and returns a
**wrapped response** to the client.

-   **This only handles object results**, and not view results. Actions
    returning any object, JsonResult or ObjectResult will be handled.
    Actions are not handled if they return a view or any other result type implementing
    IActionsResult. It is recommend that you use the built-in UseExceptionHandler extension
    method defined in the Microsoft.AspNetCore.Diagnostics package to handle view exceptions.
-   Exception handling and logging behaviour can be changed using the
    **WrapResult** and **DontWrapResult** attributes for methods and
    classes.

#### Result Filter

**AbpResultFilter** is mainly used to wrap the result action if the action is
successfully executed.

-   It only wraps results for JsonResult, ObjectResult and any object
    which does not implement IActionResult (and also their async
    versions). If your action is returning a view or any other type of
    result, it will not be wrapped.
-   The **WrapResult** and **DontWrapResult** attributes can be used for
    methods and classes to enable/disable wrapping.
-   You can use a startup configuration to change the default behavior for
    result wrapping.

##### Result Caching For Ajax Requests

AbpResultFilter adds a **Cache-Control** header (no-cache, no-store...) to
the response of AJAX Requests. Thus, it prevents browser caching of
AJAX responses even for GET requests. This behavior can be disabled by
configuration or attributes. You can use the  **NoClientCache** attribute
to prevent caching (default) or **AllowClientCache** attrbiute to allow the
browser to cache results. Alternatively, you can implement
IClientCacheAttribute to create a custom attribute for finer
control.

### Model Binders

**AbpDateTimeModelBinder** is used to normalize DateTime (and
Nullable&lt;DateTime&gt;) inputs using the **Clock.Normalize** method.

### Views

MVC Views can be inherited from **AbpRazorPage** to automatically inject the
most commonly used infrastructure (LocalizationManager, PermissionChecker,
SettingManager... etc.). It also has shortcut methods, like L(...) for
localized texts. The startup template inherits this by default.

You can inherit your web components from **AbpViewComponent** instead of
ViewComponent to take advantage of it's base properties and methods.

### Client Proxies

ABP can automatically create JavaScript proxies for all MVC Controllers
(not only application services). It's created for *Application Services
as Controllers* (see the section above) by default. You can add the
\[RemoteService\] attribute to any MVC controller to create a client proxy
for it. JavaScript proxies are dynamically generated on runtime. You
need to add a given script definition to your page:

    <script src="~/AbpServiceProxies/GetAll?type=jquery" type="text/javascript"></script>

Currently, only jQuery proxies are generated. We can then call an MVC
method with JavaScript as shown below:

    abp.services.app.product.create({
        name: 'My test product',
        price: 99
    }).done(function(result){
        //...
    });

### Integration Testing

Integration testing is fairly easy for ASP.NET Core and it's [documented on
its own web
site](https://docs.asp.net/en/latest/testing/integration-testing.html)
in detail. ABP follows these guidelines and provides a
**AbpAspNetCoreIntegratedTestBase** class in the
[Abp.AspNetCore.TestBase](https://www.nuget.org/packages/Abp.AspNetCore.TestBase)
package. It makes integration testing even easier.

Start by investigating the integration tests in the startup template to see it in action.
