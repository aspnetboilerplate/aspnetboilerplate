## Introduction

In this article, I will explain how to add custom field to AbpSession in ASP.NET Core.

### Create Custom Session

First, create a custom session object that implements `ClaimsAbpSession`. Define your own session and add your custom field to it. Then, you can inject `MyAppSession` and use it's new property in your project.

````csharp
public class MyAppSession : ClaimsAbpSession, ITransientDependency
{
    public MyAppSession(
        IPrincipalAccessor principalAccessor,
        IMultiTenancyConfig multiTenancy,
        ITenantResolver tenantResolver,
        IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider) : 
        base(principalAccessor, multiTenancy, tenantResolver, sessionOverrideScopeProvider)
    {

    }

    public string UserEmail
    {
        get
        {
            var userEmailClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Application_UserEmail");
            if (string.IsNullOrEmpty(userEmailClaim?.Value))
            {
                return null;
            }

            return userEmailClaim.Value;
        }
    }
}
````

### Create Claim

Override `CreateAsync` method in `UserClaimsPrincipalFactory.cs` to add your custom claim when the user is logged in.

````csharp
public override async Task<ClaimsPrincipal> CreateAsync(User user)
{
    var claim = await base.CreateAsync(user);
    claim.Identities.First().AddClaim(new Claim("Application_UserEmail", user.EmailAddress));

    return claim;
}
````
