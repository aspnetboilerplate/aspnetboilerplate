### Introduction

Almost all enterprise applications use authorization in some level.
Authorization is used to check if a user is allowed to perform some
specific operation in the application. ASP.NET Boilerplate defines a
**permission based** infrastructure to implement authorization.

#### About IPermissionChecker

Authorization system uses **IPermissionChecker** to check permissions.
While you can implement it in your own way, it's fully implemented in
**module-zero** project. If it's not implemented, NullPermissionChecker
is used which grants all permissions to everyone.

### Defining Permissions

A unique **permission** is defined for each operation needed to be
authorized. We should define a permission before use it. ASP.NET
Boilerplate is designed to be [modular](/Pages/Documents/Module-System).
So, different modules can have different permissions. A module should
create a class derived from **AuthorizationProvider** in order to define
it's permissions. An example authorization provider is shown below:

    public class MyAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var administration = context.CreatePermission("Administration");

            var userManagement = administration.CreateChildPermission("Administration.UserManagement");
            userManagement.CreateChildPermission("Administration.UserManagement.CreateUser");

            var roleManagement = administration.CreateChildPermission("Administration.RoleManagement");
        }
    }
                

**IPermissionDefinitionContext** has methods to get and create
permissions.

A permission have some properties to define it:

-   **Name**: a system-wide **unique** name. It's good idea to define a
    const string for a permission name instead of a magic string. We
    prefer to use . (dot) notation for hierarchical names but it's not
    required. You can set any name you like. Only rule is that it must
    be unique.
-   **Display name**: A localizable string that can be used to show
    permission, later in UI.
-   **Description**: A localizable string that can be used to show
    definition of the permission, later in UI.
-   **MultiTenancySides**: For multi-tenant application, a permission
    can be used by tenants or the host. This is a **Flags** enumeration
    and thus a permission can be used in both sides.
-   **featureDependency**: Can be used to declare a dependency to
    [features](/Pages/Documents/Feature-Management). Thus, this
    permission can be granted only if feature dependency is satisfied.
    It waits for an object implements IFeatureDependency. Default
    implementation is the SimpleFeatureDependency class. Example usage:
    `new SimpleFeatureDependency("MyFeatureName")`

A permission can have a parent and child permissions. While this does
not effect permission checking, it may help to group permissions in UI.

After creating an authorization provider, we should register it in
PreInitialize method of our module:

    Configuration.Authorization.Providers.Add<MyAuthorizationProvider>();

Authorization providers are registered to [dependency
injection](/Pages/Documents/Dependency-Injection) automatically. So, an
authorization provider can inject any dependency (like a repository) to
build permission definitions using some other sources.

### Checking Permissions

#### Using AbpAuthorize Attribute

**AbpAuthorize** (**AbpMvcAuthorize** for MVC Controllers and
**AbpApiAuthorize** for Web API Controllers) attribute is the easiest
and most common way of checking permissions. Consider the [application
service](/Pages/Documents/Application-Services) method shown below:

    [AbpAuthorize("Administration.UserManagement.CreateUser")]
    public void CreateUser(CreateUserInput input)
    {
        //A user can not execute this method if he is not granted for "Administration.UserManagement.CreateUser" permission.
    }

CreateUser method can not be called by a user who is not granted for
permission "*Administration.UserManagement.CreateUser*".

AbpAuthorize attribute also checks if current user is logged in (using
[IAbpSession.UserId](/Pages/Documents/Abp-Session)). So, if we declare
an AbpAuthorize for a method, it only checks for login:

    [AbpAuthorize]
    public void SomeMethod(SomeMethodInput input)
    {
        //A user can not execute this method if he did not login.
    }

##### AbpAuthorize attribute notes

ASP.NET Boilerplate uses power of dynamic method interception for
authorization. So, there is some restrictions for the methods use
AbpAuthorize attribute.

-   Can not use it for private methods.
-   Can not use it for static methods.
-   Can not use it for methods of a non-injected class (We must use
    [dependency injection](/Pages/Documents/Dependency-Injection)).

Also,

-   Can use it for any **public** method if the method is called over an
    **interface** (like Application Services used over interface).
-   A method should be **virtual** if it's called directly from class
    reference (like ASP.NET MVC or Web API Controllers).
-   A method should be **virtual** if it's **protected**.

**N**<span class="auto-style1">otice</span>: There are four types of
authorize attributes:

-   In an application service (application layer), we use
    **Abp.Authorization.AbpAuthorize** attribute.
-   In an MVC controller (web layer), we use
    **Abp.Web.Mvc.Authorization.AbpMvcAuthorize** attribute.
-   In ASP.NET Web API, we use
    **Abp.WebApi.Authorization.AbpApiAuthorize** attribute.
-   In ASP.NET Core, we use
    **Abp.AspNetCore.Mvc.Authorization.AbpMvcAuthorize** attribute.

This difference comes from inheritance. In application layer it's
completely ASP.NET Boilerplate's implementation and does not extend any
class. But, int MVC and Web API, it inherits from Authorize attributes
of those frameworks.

##### Suppress Authorization

You can disable authorization for a method/class by adding
**AbpAllowAnonymous** attribute to aplication services. Use
**AllowAnonymous** for MVC, Web API and ASP.NET Core Controllers, which
are native attributes of these frameworks.

#### Using IPermissionChecker

While AbpAuthorize attribute pretty enough for most cases, there must be
situations we should check for a permission in a method body. We can
inject and use **IPermissionChecker** for that as shown in the example
below:

    public void CreateUser(CreateOrUpdateUserInput input)
    {
        if (!PermissionChecker.IsGranted("Administration.UserManagement.CreateUser"))
        {
            throw new AbpAuthorizationException("You are not authorized to create user!");
        }

        //A user can not reach this point if he is not granted for "Administration.UserManagement.CreateUser" permission.
    }

Surely, you can code any logic since **IsGranted** simply returns true
or false (It has Async version also). If you simply check a permission
and throw an exception as shown above, you can use the **Authorize**
method:

    public void CreateUser(CreateOrUpdateUserInput input)
    {
        PermissionChecker.Authorize("Administration.UserManagement.CreateUser");

        //A user can not reach this point if he is not granted for "Administration.UserManagement.CreateUser" permission.
    }

Since authorization is widely used, **ApplicationService** and some
common base classes inject and define PermissionChecker property. Thus,
permission checker can be used without injecting in application service
classes.

#### In Razor Views

Base view class defines IsGranted method to check if current user has a
permission. Thus, we can conditionally render the view. Example:

    @if (IsGranted("Administration.UserManagement.CreateUser"))
    {
        <button id="CreateNewUserButton" class="btn btn-primary"><i class="fa fa-plus"></i> @L("CreateNewUser")</button>
    }

#### Client Side (Javascript)

In the client side, we can use API defined in **abp.auth** namespace. In
most case, we need to check if current user has a specific permission
(with permission name). Example:

    abp.auth.isGranted('Administration.UserManagement.CreateUser');

You can also use **abp.auth.grantedPermissions** to get all granted
permissions or **abp.auth.allPermissions** to get all available
permission names in the application. Check **abp.auth** namespace on
runtime for others.

### Permission Manager

We may need to definitions of permission. **IPermissionManager** can be
[injected](/Pages/Documents/Dependency-Injection) and used in that case.
