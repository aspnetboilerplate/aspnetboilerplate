### Introduction

This document describes ASP.NET Core integration for ASP.NET Boilerplate
framework. ASP.NET Core integration is implemented in
[Abp.AspNetCore](https://www.nuget.org/packages/Abp.AspNetCore) nuget
package

#### Migrating to ASP.NET Core?

If you have an existing project and considering to migrate to ASP.NET
Core, you can read our [blog
post](http://volosoft.com/migrating-from-asp-net-mvc-5x-to-asp-net-core/)
for our experince on the migration.

### Startup Template

You can create your project from [startup template](/Templates), which
is a simple, empty web project but properly integrated and configured to
work with ABP framework.

### Configuration

#### Startup Class

To integrate ABP to ASP.NET Core, we should make some changes in the
Startup class as shown below:

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

You can use [startup configuration](Startup-Configuration.md) to
configure AspNet Core Module (using
*Configuration.Modules.AbpAspNetCore()* in PreInitialize of your
module).

### Controllers

Controllers can be any type of classes in ASP.NET Core. It's not
restricted to classes derived from Controller class. By default, a class
ends with Controller (like ProductController) is considered as MVC
Controller. You can also add MVC's \[Controller\] attribute to any class
to make it a controller. This is the way ASP.NET Core MVC is working.
See ASP.NET Core [documentation](https://docs.asp.net) for more.

If you will use web layer classes (like HttpContext) or return a view,
it's better to inherit from **AbpController** (which is derived from
MVC's Controller) class. But if you are creating an API controller just
works with objects, you can consider to create a POCO controller class
or you can use your application services as controllers as described
below.

#### Application Services as Controllers

ASP.NET Boilerplate provides infrastructure to create [application
services](Application-Services.md). If you want to expose your
application services to remote clients as controllers (as previously
done using [dynamic web api](Dynamic-Web-API.md)), you can easily do
it by a simple configuration in [PreInitialize](Module-System.md)
method of your module. Example:

    Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(MyApplicationModule).Assembly, moduleName: 'app', useConventionalHttpVerbs: true);

**CreateControllersForAppServices** method gets an assembly and converts
all application services to MVC controllers in that assembly. You can
use **RemoteService** attribute to enable/disable it for method or class
level.

When an application service is converted to MVC Controller, it's default
route will be like
*/api/services/&lt;module-name&gt;/&lt;service-name&gt;/&lt;method-name&gt;*.
Example: If ProductAppService defines a Create method, it's URL will be
**/api/services/app/product/create** (assumed that module name is
'app').

If **useConventionalHttpVerbs** set to **true** (which is the **default
value**), then HTTP verbs for service methods are determined by **naming
conventions**:

-   **Get**: Used if method name starts with 'Get'.
-   **Put**: Used if method name starts with 'Put' or 'Update'.
-   **Delete**: Used if method name starts with 'Delete' or 'Remove'.
-   **Post**: Used if method name starts with 'Post', 'Create' or
    'Insert'.
-   **Patch**: Used if method name starts with 'Patch'.
-   Otherwise, **Post** is used **as default** HTTP verb.

You can use any ASP.NET Core attributes to change HTTP methods or routes
of the actions (but surely, this requires you to add a reference to
Microsoft.AspNetCore.Mvc.Core package).

**Note**: Previously, dynamic web api system was requiring to create
service **interfaces** for application services. But this is not
required for ASP.NET Core integration. Also, MVC attributes should be
added to the service classes, even you have interfaces.

### Filters

ABP defines some **pre-built filters** for AspNet Core. All of them are
added to **all actions of all controllers** by default.

#### Authorization Filter

**AbpAuthorizationFilter** is used to integrate to [authorization
system](Authorization.md) and [feature
system](Feature-Management.md).

-   You can define **AbpMvcAuthorize** attribute for actions or
    controllers to check desired permissions before action execution.
-   You can define **RequiresFeature** attribute for actions or
    controllers to check desired features before action execution.
-   You can define **AllowAnonymous** (or AbpAllowAnonymous in
    application layer) attribute for actions or controllers to suppress
    authentication/authorization.

#### Audit Action Filter

**AbpAuditActionFilter** is used to integrate to [audit logging
system](Audit-Logging.md). It logs all requests to all actions by
default (if auditing is not disabled). You can control audit logging
using **Audited** and **DisableAuditing** attributes for actions and
controllers.

#### Validation Action Filter

**AbpValidationActionFilter** is used to integrate to [validation
system](Validating-Data-Transfer-Objects.md) and automatically
validate all inputs of all actions. In addition to ABP's built-in
validation & normalization, it also checks MVC's **Model.IsValid**
property and throws validation exception if action inputs have any
invalid value.

You can control validation using **EnableValidation** and
**DisableValidation** attributes for actions and controllers.

#### Unit of Work Action Filter

**AbpUowActionFilter** is used to integrate to [Unit of
Work](Unit-Of-Work.md) system. It automatically begins a new unit of
work before an action execution and completes unit of work after action
exucition (if no exception is thrown).

You can use **UnitOfWork** attribute to control behaviour of UOW for an
action. You can also use startup configuration to change default unit of
work attribute for all actions.

#### Exception Filter

**AbpExceptionFilter** is used to handle exceptions thrown from
controller actions. It **handles** and **logs** exceptions and returns
**wrapped response** to the client.

-   It **only handles object results**, not view results. So, actions
    returns any object, JsonResult or ObjectResult will be handled.
    Action returns a view or any other result type implements
    IActionsResult are not handled. It's suggested to use built-in
    UseExceptionHandler extension method defined in
    Microsoft.AspNetCore.Diagnostics package to handle view exceptions.
-   Exception handling and logging behaviour can be changed using
    **WrapResult** and **DontWrapResult** attributes for methods and
    classes.

#### Result Filter

**AbpResultFilter** is mainly used to wrap result action if action is
successfully executed.

-   It only wraps results for JsonResult, ObjectResult and any object
    which does not implement IActionResult (and also their async
    versions). If your action is returning a view or any other type of
    result, it's not wrapped.
-   **WrapResult** and **DontWrapResult** attributes can be used for
    methods and classes to enable/disable wrapping.
-   You can use startup configuration to change default behaviour for
    result wrapping.

##### Result Caching For Ajax Requests

AbpResultFilter adds **Cache-Control** header (no-cache, no-store...) to
the response for AJAX Requests. Thus, it prevents browser caching of
AJAX responses even for GET requests. This behaviour can be disabled by
the configuration or attributes. You can use **NoClientCache** attrbiute
to prevent caching (default) or **AllowClientCache** attrbiute to allow
browser to cache results. Alternatively you can implement
IClientCacheAttribute to create your special attribute for finer
control.

### Model Binders

**AbpDateTimeModelBinder** is used to normalize DateTime (and
Nullable&lt;DateTime&gt;) inputs using **Clock.Normalize** method.

### Views

MVC Views can be inherited from **AbpRazorPage** to automatically inject
most used infrastructure (LocalizationManager, PermissionChecker,
SettingManager... etc.). It also has shortcut methods (like L(...) for
localize texts). Startup template inherits it by default.

You can inherit your web components from **AbpViewComponent** instead of
ViewComponent to take advantage of base properties and methods.

### Client Proxies

ABP can automatically create javascript proxies for all MVC Controllers
(not only application services). It's created for *Application Services
as Controllers* (see the section above) by default. You can add
\[RemoteService\] attribute to any MVC controller to create client proxy
for it. Javascript proxies are dynamically generated on runtime. You
need to add given script definition to your page:

    <script src="~/AbpServiceProxies/GetAll?type=jquery" type="text/javascript"></script>

Currently, only JQuery proxies are generated. We can then call an MVC
method with javascript as shown below:

    abp.services.app.product.create({
        name: 'My test product',
        price: 99
    }).done(function(result){
        //...
    });

### Integration Testing

Integration testing is fairly easy for ASP.NET Core and it's [documented
it's own web
site](https://docs.asp.net/en/latest/testing/integration-testing.html)
in details. ABP follows this guide and provides
**AbpAspNetCoreIntegratedTestBase** class in
[Abp.AspNetCore.TestBase](https://www.nuget.org/packages/Abp.AspNetCore.TestBase)
package. It makes integration testing even easier.

It's better to start by investigating integration tests in startup
template to see it in action.
