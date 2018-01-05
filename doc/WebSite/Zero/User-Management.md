### User Entity

User entity represents a **user of the application**. It should be
derived from **AbpUser** class as shown below:

    public class User : AbpUser<Tenant, User>
    {
        //add your own user properties here
    }

This class is created when you
[install](/Pages/Documents/Zero/Installation) module-zero. Users are
stored in **AbpUsers** table in the database. You can add your custom
properties to User class (and create database migrations for the
changes).

AbpUser class defines some base properties. Some of the properties are:

-   **UserName**: Login name of the user Should be **unique** for a
    [tenant](/Pages/Documents/Zero/Tenant-Management).
-   **EmailAddress**: Email address of the user. Should be **unique**
    for a [tenant](/Pages/Documents/Zero/Tenant-Management).
-   **Password**: Hashed password of the user.
-   **IsActive**: True, if this user can login to the application.
-   **Name** and **Surname** of the user.

There are also some properties like **Roles**, **Permissions**,
**Tenant**, **Settings**, **IsEmailConfirmed** and so on. Check AbpUser
class for more information.

AbpUser class is inherited from **FullAuditedEntity**. That means it has
creation, modification and deletion audit properties. It's also
**[Soft-Delete](/Pages/Documents/Data-Filters#isoftdelete)** . So,
when we delete a user, it's not deleted from database, just marked as
deleted.

AbpUser class implements
[IMayHaveTenant](/Pages/Documents/Data-Filters#imayhavetenant) filter
to properly work in a multi-tenant application.

Finally, **Id** of the User is defined as **long**.

### User Manager

**UserManager** is a service to perform **domain logic** for users:

    public class UserManager : AbpUserManager<Tenant, Role, User>
    {
        //...
    }

You can [inject](/Pages/Documents/Dependency-Injection) and use
UserManager to create, delete, update users, grant permissions, change
roles for users and much more. You can add your own methods here. Also,
you can **override** any method of **AbpUserManager** base class for
your own needs.

#### Multi Tenancy

*If you're not creating a multi-tenant application, you can skip this
section. See [multi-tenancy documentation](../Multi-Tenancy.md) for
more information about multi-tenancy.*

UserManager is designed to work for a **single tenant** in a time. It
works for the **current tenant** as default. Let's see some usages of
the UserManager:

    public class MyTestAppService : ApplicationService
    {
        private readonly UserManager _userManager;

        public MyTestAppService(UserManager userManager)
        {
            _userManager = userManager;
        }

        public void TestMethod_1()
        {
            //Find a user by email for current tenant
            var user = _userManager.FindByEmail("sampleuser@aspnetboilerplate.com");
        }

        public void TestMethod_2()
        {
            //Switch to tenant 42
            CurrentUnitOfWork.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, 42);

            //Find a user by email for the tenant 42
            var user = _userManager.FindByEmail("sampleuser@aspnetboilerplate.com");
        }

        public void TestMethod_3()
        {
            //Disabling MayHaveTenant filter, so we can reach to all users
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                //Now, we can search for a user name in all tenants
                var users = _userManager.Users.Where(u => u.UserName == "sampleuser").ToList();

                //Or we can add TenantId filter if we want to search for a specific tenant
                var user = _userManager.Users.FirstOrDefault(u => u.TenantId == 42 && u.UserName == "sampleuser");
            }
        }
    }

#### User Login
,
Module-zero defines LoginManager which has a **LoginAsync** method used
to login to the application. It checks all logic for login and returns a
login result. LoginAsync method also **automatically saves all login
attempts** to the database (even it's a failed attempt). You can use
**UserLoginAttempt** entity to query it.

#### About IdentityResults

Some methods of UserManager return IdentityResult as a result instead of
throwing exceptions for some cases. This is nature of ASP.NET Identity
Framework. Module-zero also follows it. So, we should check this
returning result object to know if operation succeeded.

Module-zero defines **CheckErrors** extension method that automatically
checks errors and throws exception (a localized
[UserFriendlyException](/Pages/Documents/Handling-Exceptions#userfriendlyexception))
if needed. Example usage:

    (await UserManager.CreateAsync(user)).CheckErrors();

To get localized exceptions, we should provide a
[ILocalizationManager](/Pages/Documents/Localization) instance:

    (await UserManager.CreateAsync(user)).CheckErrors(LocalizationManager);

### External Authentication

Login method of module-zero authenticate a user from the **AbpUsers**
table in the database. Some applications may require to authenticate
users from some external sources (like active directory, from another
database's tables or even from a remote service).

For such cases, UserManager defines an extension point named 'external
authentication source'. We can create a class derived from
**IExternalAuthenticationSource** and register to the configuration.
There is **DefaultExternalAuthenticationSource** class to simplify
implementation of IExternalAuthenticationSource. Let's see an example:

    public class MyExternalAuthSource : DefaultExternalAuthenticationSource<Tenant, User>,  ITransientDependency
    {
        public override string Name
        {
            get { return "MyCustomSource"; }
        }

        public override Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, Tenant tenant)
        {
            //TODO: authenticate user and return true or false
        }
    }

In TryAuthenticateAsync method, we can check user name and password from
some source and return true if given user is authenticated by this
source. Also, we can override CreateUser and UpdateUser methods to
control user creation and updating for this source.

When a user authenticated by an external source, module-zero checks if
this user does exists in the database (AbpUsers table). If not, it calls
CreateUser to create the user, otherwise it calls UpdateUser to allow
authentication source to update existing user informations.

We can define more than one external authentication source in an
application. AbpUser entity has an AuthenticationSource property that
shows which source authenticated this user.

To register our authenciation source, we can use such a code in
[PreInitialize](/Pages/Documents/Module-System) of our module:

    Configuration.Modules.Zero().UserManagement.ExternalAuthenticationSources.Add<MyExternalAuthSource>();

#### LDAP/Active Directory

LdapAuthenticationSource is an implementation of external authentication
to make users login with their LDAP (active directory) user name and
password.

If we want to use LDAP authentication, we first add
[Abp.Zero.Ldap](https://www.nuget.org/packages/Abp.Zero.Ldap) nuget
package to our project (generally to Core (domain) project). Then we
should extend **LdapAuthenticationSource** for our application as shown
below:

    public class MyLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public MyLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }

Lastly, we should set a module dependency to **AbpZeroLdapModule** and
**enable** LDAP with the auth source created above:

    [DependsOn(typeof(AbpZeroLdapModule))]
    public class MyApplicationCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.ZeroLdap().Enable(typeof (MyLdapAuthenticationSource));    
        }

        ...
    }

After these steps, LDAP module will be enabled for your application. But
LDAP auth is not enabled by default. We can enable it using settings.

##### Settings

**LdapSettingNames** class defines constants for setting names. You can
use these constant names while changing settings (or getting settings).
LDAP settings are **per tenant** (for multi-tenant applications). So,
different tenants have different settings (see setting definitions on
[github](https://github.com/aspnetboilerplate/module-zero/blob/master/src/Abp.Zero.Ldap/Ldap/Configuration/LdapSettingProvider.cs)). 

As you can see in the MyLdapAuthenticationSource **constructor**,
LdapAuthenticationSource expects **ILdapSettings** as a constructor
argument. This interface is used to get LDAP settings like domain, user
name and password to connect to Active Directory. Default implementation
(**LdapSettings** class) gets these settings from the [setting
manager](/Pages/Documents/Setting-Management).

If you work with Setting manager, then no problem. You can change LDAP
settings using [setting manager
API](/Pages/Documents/Setting-Management). If you want, you can add an
initial/seed data to database to enable LDAP auth by default.

Note: If you don't define domain, username and password, LDAP
authentication works for current domain if your application runs in a
domain with appropriate privileges.

##### Custom Settings

If you want to define another setting source, you can implement a custom
ILdapSettings class as shown below:

    public class MyLdapSettings : ILdapSettings
    {
        public async Task<bool> GetIsEnabled(int? tenantId)
        {
            return true;
        }

        public async Task<ContextType> GetContextType(int? tenantId)
        {
            return ContextType.Domain;
        }

        public async Task<string> GetContainer(int? tenantId)
        {
            return null;
        }

        public async Task<string> GetDomain(int? tenantId)
        {
            return null;
        }

        public async Task<string> GetUserName(int? tenantId)
        {
            return null;
        }

        public async Task<string> GetPassword(int? tenantId)
        {
            return null;
        }
    }

And register it to IOC in PreInitialize of your module:

    [DependsOn(typeof(AbpZeroLdapModule))]
    public class MyApplicationCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<ILdapSettings, MyLdapSettings>(); //change default setting source
            Configuration.Modules.ZeroLdap().Enable(typeof (MyLdapAuthenticationSource));
        }

        ...
    }

Then you can get LDAP settings from any other source.

#### Social Logins

See [social authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/)
document for social logins.
