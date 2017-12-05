### Introduction

[Identity Server](http://identityserver.io/) is an open source **OpenID
Connect** and **OAuth 2.0** framework. It can be used to make your
application an **authentication / single sign on server**. It can also
issue **access tokens** for 3rd party clients. This document describes
how you can integrate IdentityServer4 (version **2.0+**) to your
project.

#### Startup Project

This document assumes that you have created an ASP.NET Core based
project (including module zero) from [startup templates](/Templates) and
made it working. I created an [ASP.NET Core MVC startup
project](Startup-Template-Core.md) for demonstration.

### Installation

There are two nuget packages:

-   [**Abp.ZeroCore.IdentityServer4**](https://www.nuget.org/packages/Abp.ZeroCore.IdentityServer4)
    is the main integration package.
-   [**Abp.ZeroCore.IdentityServer4.EntityFrameworkCore**](https://www.nuget.org/packages/Abp.ZeroCore.IdentityServer4.EntityFrameworkCore)
    is the storage provider for EF Core.

Since EF Core package already depends on the first one, you can only
install
[**Abp.ZeroCore.IdentityServer4.EntityFrameworkCore**](https://www.nuget.org/packages/Abp.ZeroCore.IdentityServer4.EntityFrameworkCore)
package to your project. Install it to the project contains your
DbContext (.EntityFrameworkCore project for default templates):

    Install-Package Abp.ZeroCore.IdentityServer4.EntityFrameworkCore

Then you can add dependency to your [module](../Module-System.md)
(generally, to your EntityFrameworkCore project):

    [DependsOn(typeof(AbpZeroCoreIdentityServerEntityFrameworkCoreModule))]
    public class MyModule : AbpModule
    {
        //...
    }

### Configuration

Configuring and using IdentityServer4 with Abp.ZeroCore is similar to
independently use IdentityServer4. You should read it's [own
documentation](https://identityserver4.readthedocs.io) to understand and
use it. In this document, we only show additional configuration needed
to integrate to Abp.ZeroCore.

#### Startup Class

In the ASP.NET Core **Startup class**, we should add IdentityServer to
**service collection** and to ASP.NET Core **middleware pipeline**.
Highlighted the **differences** from standard IdentityServer4 usage:

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                    .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                    .AddInMemoryClients(IdentityServerConfig.GetClients())
                    .AddAbpPersistedGrants<IAbpPersistedGrantDbContext>()
                    .AddAbpIdentityServer<User>(); ;

            //...
        }

        public void Configure(IApplicationBuilder app)
        {
            //...

                app.UseJwtTokenMiddleware("IdentityBearer");
                app.UseIdentityServer();
                
            //...
        }
    }

Added **services.AddIdentityServer()** just after
**IdentityRegistrar.Register(services)** and added
**app.UseJwtTokenMiddleware("IdentityBearer")** just after
**app.UseAuthentication()** in the startup project.

#### IdentityServerConfig Class

We have used IdentityServerConfig class to get identity resources, api
resources and clients. You can find more information about this class in
it's own
[documentation](https://identityserver4.readthedocs.io/en/release/quickstarts/1_client_credentials.html).
For the simplest case, it can be a static class like below:

    public static class IdentityServerConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("default-api", "Default (all) API")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone()
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials.Union(GrantTypes.ResourceOwnerPassword).ToList(),
                    AllowedScopes = {"default-api"},
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };
        }
    }

#### DbContext Changes

**AddAbpPersistentGrants()** method is used to save consent responses to
the persistent data store. In order to use it, **YourDbContext** must
implement **IAbpPersistedGrantDbContext** interface as shown below:

    public class YourDbContext : AbpZeroDbContext<Tenant, Role, User, YourDbContext>, IAbpPersistedGrantDbContext
    {
        public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public YourDbContext(DbContextOptions<YourDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }

IAbpPersistedGrantDbContext defines **PersistedGrants** DbSet. We also
should call modelBuilder.**ConfigurePersistedGrantEntity()** extension
method as shown above in order to configure EntityFramework for
**PersistedGrantEntity**.

Notice that this change in YourDbContext cause a new database migration.
So, remember to use "Add-Migration" and "Update-Database" commands to
update your database.

IdentityServer4 will continue to work even if you don't call
AddAbpPersistedGrants&lt;YourDbContext&gt;() extension method, but user
consent responses will be stored an in-memory data store in that case
(which is cleared when you restart your application).

#### JWT Authentication Middleware

If we want to authorize clients against the same application we can use
[IdentityServer authentication
middleware](http://docs.identityserver.io/en/release/topics/apis.html?highlight=UseIdentityServerAuthentication#the-identityserver-authentication-middleware)
for that.

First, install IdentityServer4.AccessTokenValidation package from nuget
to your project:

    Install-Package IdentityServer4.AccessTokenValidation

Then we can add the middleware to Startup class as shown below:

                services.AddAuthentication().AddIdentityServerAuthentication("IdentityBearer", options =>
                {
                    options.Authority = "http://localhost:62114/";
                    options.RequireHttpsMetadata = false;
                });

I added this just after **services.AddIdentityServer()** in the startup
project.

#### IdentityServer4.AccessTokenValidation Status

*IdentityServer4.AccessTokenValidation* package is not ready for ASP.NET
Core 2.0 yet (at the time we write this document). See
https://github.com/IdentityServer/IdentityServer4/issues/1055

### Test

Now, our identity server is ready to get requests from clients. We can
create a console application to make requests and get responses.

-   Create a new **Console Application** inside your solution.
-   Add **IdentityModel** nuget package to the console application. This
    package is used to create clients for OAuth endpoints.

While **IdentityModel** nuget package is enough to create a client and
consume your API, I want to show to use API in more type safe way: We
will convert incoming data to DTOs returned by application services.

-   Add reference to **Application** layer from the console application.
    This will allow us to use same DTO classes returned by the
    application layer in the client side.
-   Add **Abp.Web.Common** nuget package. This will allow us to use
    AjaxResponse class defined in ASP.NET Boilerplate class. Otherwise,
    we will deal with raw JSON strings to handle the server response.

Then we can change Program.cs as shown below:

    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Abp.Json;
    using IdentityModel.Client;
    using Abp.MultiTenancy;
    using Abp.Web.Models;
    using IdentityServerIntegrationDemo.Users.Dto;
    using Newtonsoft.Json;

    namespace IdentityServerIntegrationDemo.ConsoleApiClient
    {
        class Program
        {
            static void Main(string[] args)
            {
                RunDemoAsync().Wait();
                Console.ReadLine();
            }

            public static async Task RunDemoAsync()
            {
                var accessToken = await GetAccessTokenViaOwnerPasswordAsync();
                await GetUsersListAsync(accessToken);
            }

            private static async Task<string> GetAccessTokenViaOwnerPasswordAsync()
            {
                var disco = await DiscoveryClient.GetAsync("http://localhost:62114");

                var httpHandler = new HttpClientHandler();
                httpHandler.CookieContainer.Add(new Uri("http://localhost:62114/"), new Cookie(MultiTenancyConsts.TenantIdResolveKey, "1")); //Set TenantId
                var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret", httpHandler);
                var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("admin", "123qwe");

                if (tokenResponse.IsError)
                {
                    Console.WriteLine("Error: ");
                    Console.WriteLine(tokenResponse.Error);
                }

                Console.WriteLine(tokenResponse.Json);

                return tokenResponse.AccessToken;
            }

            private static async Task GetUsersListAsync(string accessToken)
            {
                var client = new HttpClient();
                client.SetBearerToken(accessToken);

                var response = await client.GetAsync("http://localhost:62114/api/services/app/user/GetAll");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.StatusCode);
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                var ajaxResponse = JsonConvert.DeserializeObject<AjaxResponse<PagedResultDto<UserListDto>>>(content);
                if (!ajaxResponse.Success)
                {
                    throw new Exception(ajaxResponse.Error?.Message ?? "Remote service throws exception!");
                }

                Console.WriteLine();
                Console.WriteLine("Total user count: " + ajaxResponse.Result.TotalCount);
                Console.WriteLine();
                foreach (var user in ajaxResponse.Result.Items)
                {
                    Console.WriteLine($"### UserId: {user.Id}, UserName: {user.UserName}");
                    Console.WriteLine(user.ToJsonString(indented: true));
                }
            }
        }
        
        internal class UserListDto
        {
            public int Id { get; set; }
            public string UserName { get; set; }
        }
    }

Before running this application, ensure that your web project is up and
running, because this console application will make request to the web
application. Also, ensure that the requesting port (62114) is the same
of your web application.
