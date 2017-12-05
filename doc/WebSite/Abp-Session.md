### Introduction

ASP.NET Boilerplate provides **IAbpSession** interface to obtain current
user and tenant **without** using ASP.NET's Session. IAbpSession is also
fully integrated and used by other structures in ASP.NET Boilerplate
([setting](Setting-Management.md) system and
[authorization](Authorization.md) system for instance).

### Injecting Session

IAbpSession is generally **[property
injected](/Pages/Documents/Dependency-Injection#DocPropertyInjection)**
to needed classes unless it's not possible to work without session
informations. If we use property injection, we can use
**NullAbpSession.Instance** as default value as shown below:

    public class MyClass : ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        public MyClass()
        {
            AbpSession = NullAbpSession.Instance;
        }

        public void MyMethod()
        {
            var currentUserId = AbpSession.UserId;
            //...
        }
    }

Since authentication/authorization is an application layer task, it's
adviced to **use IAbpSession in application layer and upper layers** (we
don't use it in domain layer normally). **ApplicationService**,
**AbpController,** **AbpApiController** and some other base classes has
**AbpSession** already injected. So, you can directly use AbpSession
property in an application service method for instance.

### Session Properties

AbpSession defines a few key properties:

-   **UserId**: Id of the current user or null if there is no current
    user. It can not be null if the calling code is authorized.
-   **TenantId**: Id of the current tenant or null if there is no
    current tenant (in case of user has not logged in or he is a host
    user).
-   **ImpersonatorUserId**: Id of the impersonator user if current
    session is impersonated by another user. It's null if this is not an
    impersonated login.
-   **ImpersonatorTenantId**: Id of the impersonator user's tenant, if
    current session is impersonated by another user. It's null if this
    is not an impersonated login.
-   **MultiTenancySide**: It may be Host or Tenant.

UserId and TenantId is **nullable**. There is also non-nullable
**GetUserId()** and **GetTenantId()** methods. If you're sure there is a
current user, you can call GetUserId(). If current user is null, this
method throws exception. GetTenantId() is also similar.

Impersonator properties are not common as other properties and generally
used for [audit logging](/Pages/Documents/Audit-Logging) purposes.

**ClaimsAbpSession**

ClaimsAbpSession is the **default implementation** of IAbpSession
interface. It gets session properties (except MultiTenancySide, it's
calculated) from claims of current user's princical. For a cookie based
form authentication, it gets from cookies. Thus, it' well integrated to
ASP.NET's authentication mechanism.

### Overriding Current Session Values

In some specific cases, you may need to change/override session values
for a limited scope. In such cases, you can use IAbpSession.Use method
as shown below:

    public class MyService
    {
        private readonly IAbpSession _session;

        public MyService(IAbpSession session)
        {
            _session = session;
        }

        public void Test()
        {
            using (_session.Use(42, null))
            {
                var tenantId = _session.TenantId; //42
                var userId = _session.UserId; //null
            }
        }
    }

Use method returns an IDisposable and it **must be disposed**. Once the
return value is disposed, Session values are **automatically restored**
the to previous values.

#### Warning!

Always use it in a using block as shown above. Otherwise, you may get
unexpected session values. You can have nested Use blocks and they will
work as you expect.

### User Identifier

You can use **.ToUserIdentifier()** extension method to create a
UserIdentifier object from IAbpSession. Since UserIdentifier is used in
most API, this will simplify to create a UserIdentifier object for the
current user.
