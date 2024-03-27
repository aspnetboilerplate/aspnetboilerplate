### Introduction

[OpenIddict](https://github.com/openiddict/openiddict-core) aims at providing a versatile solution to implement OpenID Connect client, server and token validation support in any ASP.NET Core 2.1 (and higher) application. ASP.NET 4.6.1 (and higher) applications are also fully supported thanks to a native Microsoft.Owin 4.2 integration.

#### Startup Project

This document assumes that you have already created an ASP.NET Core based
project (including Module Zero) from the [startup templates](/Templates) and
have set it up to work. We created an [ASP.NET Core MVC startup
project](/Pages/Documents/Zero/Startup-Template-Core) for this demonstration.

### Installation

There are 3 NuGet packages:

-   [**Abp.ZeroCore.OpenIddict**](https://www.nuget.org/packages/Abp.ZeroCore.OpenIddict)
    is the main integration package.
-   [**Abp.ZeroCore.OpenIddict.EntityFrameworkCore**](https://www.nuget.org/packages/Abp.ZeroCore.OpenIddict.EntityFrameworkCore)
    is the storage provider for EF Core.
-   [**Abp.AspNetCore.OpenIddict**](https://www.nuget.org/packages/Abp.AspNetCore.OpenIddict)
    is the package for ASP.NET Core.    

Install **Abp.ZeroCore.OpenIddict** package to your Core project and add a module dependency to `AbpZeroCoreOpenIddictModule`.

Install **Abp.AspNetCore.OpenIddict** package to your Web project and add a module dependency to `AbpZeroCoreOpenIddictEntityFrameworkCoreModule`.

Install **Abp.ZeroCore.OpenIddict.EntityFrameworkCore** package to your EntityFrameworkCore project and add a module dependency to `AbpAspNetCoreOpenIddictModule`.

### Configuration

Configuring and using OpenIddict with Abp.ZeroCore is similar to
independently using OpenIddict. You should read its [own
documentation](https://documentation.openiddict.com/index.html) to better understand how

#### EntityFrameworkCore Project

You need to implement `IOpenIddictDbContext` in your DbContext class. This will add Entities required by OpenIddict to your DbContext.

After this, call `modelBuilder.ConfigureOpenIddict();` in the `OnModelCreating` method of your DbContext.

Then, create repositories inherited from below classes;

* `EfCoreOpenIddictApplicationRepository`
* `EfCoreOpenIddictAuthorizationRepository`
* `EfCoreOpenIddictScopeRepository`
* `EfCoreOpenIddictTokenRepository`

#### Web Project

In your web project, create Controllers inherited from generic controllers defined in ASP.NET Boilerplate;

* `AuthorizeController`
* `TokenController`
* `UserInfoController`

Create a static class to Configure `OpenIddict` in your Web project and call it in Startup.cs;

```csharp
public static class OpenIddictRegistrar
    {
        public static void Register(
            IServiceCollection services, 
            IConfigurationRoot configuration,
            Action<OpenIddictCoreOptions> setupOptions)
        {
            services.Configure<AbpOpenIddictClaimsPrincipalOptions>(options =>
            {
                options.ClaimsPrincipalHandlers.Add<AbpDefaultOpenIddictClaimsPrincipalHandler>();
            });

            services.AddOpenIddict()

                // Register the OpenIddict core components.
                .AddCore(builder =>
                {
                    builder
                        .SetDefaultApplicationEntity<OpenIddictApplicationModel>()
                        .SetDefaultAuthorizationEntity<OpenIddictAuthorizationModel>()
                        .SetDefaultScopeEntity<OpenIddictScopeModel>()
                        .SetDefaultTokenEntity<OpenIddictTokenModel>();

                    builder
                        .AddApplicationStore<AbpOpenIddictApplicationStore>()
                        .AddAuthorizationStore<AbpOpenIddictAuthorizationStore>()
                        .AddScopeStore<AbpOpenIddictScopeStore>()
                        .AddTokenStore<AbpOpenIddictTokenStore>();
                })

                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    // Enable the token endpoint.
                    options.SetAuthorizationEndpointUris("connect/authorize", "connect/authorize/callback")
                        .SetTokenEndpointUris("connect/token")
                        .SetUserinfoEndpointUris("connect/userinfo");

                    // Enable the client credentials flow.
                    options.AllowClientCredentialsFlow();
                    options.AllowPasswordFlow();
                    options.AllowAuthorizationCodeFlow();

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableUserinfoEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableVerificationEndpointPassthrough()
                        .EnableStatusCodePagesIntegration();

                    options.DisableAccessTokenEncryption();
                })

                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });
        }
    }
```

You also need to fill data into OpenIddict tables by following its own [documentation](https://documentation.openiddict.com/)