### Introduction

ASP.NET Boilerplate is integrated into the **ASP.NET Web API Controllers** via
the **Abp.Web.Api** NuGet package. You can create regular ASP.NET Web API
Controllers like you always do. [Dependency
Injection](/Pages/Documents/Dependency-Injection) properly works for
regular ApiControllers, but you should derive your controllers from
**AbpApiController**, which provides several benefits and integrates better
into ASP.NET Boilerplate.

### AbpApiController Base Class

This is a simple api controller derived from **AbpApiController**:

    public class UsersController : AbpApiController
    {

    }

#### Localization

AbpApiController defines an **L** method to make
[localization](/Pages/Documents/Localization) easier. Example:

    public class UsersController : AbpApiController
    {
        public UsersController()
        {
            LocalizationSourceName = "MySourceName";
        }

        public UserDto Get(long id)
        {
            var helloWorldText = L("HelloWorld");

            //...
        }
    }

You must set the **LocalizationSourceName** property in order to make the **L** method work.
You can set this in your own base api controller class so you don't have to repeat it 
for each api controller.

#### Others

You can also use the pre-injected
[AbpSession](/Pages/Documents/Abp-Session),
[EventBus](/Pages/Documents/EventBus-Domain-Events), [PermissionManager,
PermissionChecker](/Pages/Documents/Authorization),
[SettingManager](/Pages/Documents/Setting-Management), [FeatureManager,
FeatureChecker](/Pages/Documents/Feature-Management),
[LocalizationManager](/Pages/Documents/Localization),
[Logger](/Pages/Documents/Logging), and
[CurrentUnitOfWork](/Pages/Documents/Unit-Of-Work) base properties (and
more).

### Filters

ABP defines some **pre-built filters** for the AspNet Web API. All of them
are added to **all actions of all controllers** by default.

#### Audit Logging

The **AbpApiAuditFilter** is used to integrate into the [audit logging
system](Audit-Logging.md). It logs all requests to all actions by
default (if auditing is not disabled). You can control audit logging
using the **Audited** and **DisableAuditing** attributes for actions and
controllers.

#### Authorization

You can use the **AbpApiAuthorize** attribute for your api controllers or
actions to prevent unauthorized users from using your controllers and
actions. Example:

    public class UsersController : AbpApiController
    {
        [AbpApiAuthorize("MyPermissionName")]
        public UserDto Get(long id)
        {
            //...
        }
    }

You can define the **AllowAnonymous** attribute for actions or controllers
to suppress authentication/authorization. AbpApiController also defines
the **IsGranted** method as a shortcut to check the permissions in the code.

See the [authorization](/Pages/Documents/Authorization) documentation for
more info. 

#### Anti Forgery Filter

**AbpAntiForgeryApiFilter** is used to automatically protect the ASP.NET Web API
actions from CSRF/XSRF attacks (including the [dynamic web api](Dynamic-Web-API.md)) (for POST,
PUT and DELETE requests). See the [CSRF
documentation](XSRF-CSRF-Protection.md) for more info. 

#### Unit Of Work

**AbpApiUowFilter** is used to integrate into the [Unit of
Work](Unit-Of-Work.md) system. It automatically begins a new unit of
work before an action execution and if no exception is thrown, completes the unit of work after 
the action execution.

You can use the **UnitOfWork** attribute to control the behavior of the UOW for an
action. You can also use the startup configuration to change the default unit of
work attribute for all actions.

#### Result Wrapping & Exception Handling

ASP.NET Boilerplate **does not wrap** Web API actions **by default** if
an action has successfully executed. It, however, **handles and wraps
exceptions**. You can add the WrapResult/DontWrapResult attributes to actions and
controllers for finer control. You can change this default behavior from the
[startup configuration](Startup-Configuration.md) (using
Configuration.Modules.AbpWebApi()...). See the [AJAX
document](Javascript-API/AJAX.md) for more info about result wrapping. 

#### Result Caching

ASP.NET Boilerplate adds the Cache-Control header (no-cache, no-store) to
the response of Web API requests. This way, it prevents browser caching of
responses even for GET requests. This behavior can be disabled through the
configuration.

#### Validation

**AbpApiValidationFilter** automatically checks **ModelState.IsValid**
and prevents execution of the action if it's not valid. It also implements
input DTO validation as described in the [validation
documentation](Validating-Data-Transfer-Objects.md).

### Model Binders

**AbpApiDateTimeBinder** is used to normalize DateTime (and
Nullable&lt;DateTime&gt;) inputs using the **Clock.Normalize** method.
