### Introduction

[Hangfire](http://hangfire.io/) is a compherensive background job
manager. You can **integrate** ASP.NET Boilerplate with Hangfire to use
it instead of the [default background job
manager](/Pages/Documents/Background-Jobs-And-Workers). You can use the
**same background job API** for Hangfire. As such, your code will be
**independent** of Hangfire. If you like, you can directly use
**Hangfire's API**, too.

Hangfire Integration depends on the frameworks you are using.

### ASP.NET Core Integration

The [**Abp.HangFire.AspNetCore**](https://www.nuget.org/packages/Abp.HangFire.AspNetCore)
package is used to integrate to ASP.NET Core based applications. It
depends on
[Hangfire.AspNetCore](https://www.nuget.org/packages/Hangfire.AspNetCore/).
[This
document](https://www.hangfire.io/blog/2016/07/16/hangfire-1.6.0.html)
describes how to install hangfire to an ASP.NET Core project. It's similar
for ABP based projects too. First install the
[Abp.HangFire.AspNetCore](https://www.nuget.org/packages/Abp.HangFire.AspNetCore)
package to your web project:

    Install-Package Abp.HangFire.AspNetCore

You can then install any storage for Hangfire. The most common one is SQL
Server (see the
[**Hangfire.SqlServer**](https://www.nuget.org/packages/Hangfire.SqlServer)
NuGet package). After you have installed these NuGet packages, you need to
**configure** your project to use Hangfire.

First, we change the Startup class to add Hangfire to dependency
injection, and then configure the storage and connection string in the
**ConfigureServices** method:

    services.AddHangfire(config =>
    {
        config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
    });

We then add the UseHangfireServer call in the **Configure** method:

    app.UseHangfireServer();

If you want to use hangfire's dashboard, you can add it, too:

    app.UseHangfireDashboard();

If you want to [authorize](Authorization.md) the dashboard, you can
use AbpHangfireAuthorizationFilter as shown below:

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new AbpHangfireAuthorizationFilter("MyHangFireDashboardPermissionName") }
    });

The configuration above is the standard way to integrate hangfire to an
ASP.NET Core application. For ABP based projects, we should also
configure our web module to replace Hangfire for ABP's default
background job manager:

    [DependsOn(typeof (AbpHangfireAspNetCoreModule))]
    public class MyProjectWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.UseHangfire();             
        }

        //...
    }

We added **AbpHangfireAspNetCoreModule** as a dependency and used the
Configuration.BackgroundJobs.**UseHangfire** method to replace Hangfire
for ABP's default background job manager.

Hangfire requires the schema creation permission in your database since it
creates its own schema and tables on first run. See the [Hangfire
documentation](http://docs.hangfire.io/en/latest/) for more information.

### ASP.NET MVC 5.x Integration

The [**Abp.HangFire**](https://www.nuget.org/packages/Abp.HangFire) NuGet
package is used for ASP.NET MVC 5.x projects:

    Install-Package Abp.HangFire

You can then install any storage for Hangfire. The most common one is SQL
Server (see the
[**Hangfire.SqlServer**](https://www.nuget.org/packages/Hangfire.SqlServer)
NuGet package). After you have installed these NuGet packages, you can
**configure** your project to use Hangfire as shown below:

    [DependsOn(typeof (AbpHangfireModule))]
    public class MyProjectWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.UseHangfire(configuration =>
            {
                configuration.GlobalConfiguration.UseSqlServerStorage("Default");
            });
                    
        }

        //...
    }

We added **AbpHangfireModule** as a dependency and used the
Configuration.BackgroundJobs.**UseHangfire** method to enable and
configure Hangfire ("Default" is the connection string in web.config).

Hangfire requires the schema creation permission in your database since it
creates its own schema and tables on first run. See the [Hangfire
documentation](http://docs.hangfire.io/en/latest/) for more information.

#### Dashboard Authorization

Hangfire can show a **dashboard page** so you can see the status of all background
jobs in real time. You can configure it as described in its
[documentation](http://docs.hangfire.io/en/latest/configuration/using-dashboard.html).
By default, this dashboard page is available for all users, and is not
authorized. You can integrate it in to ABP's [authorization
system](Authorization.md) using the **AbpHangfireAuthorizationFilter**
class defined in the Abp.HangFire package. Example configuration:

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new AbpHangfireAuthorizationFilter() }
    });

This checks if the current user has logged in to the application. If you
want to require an additional permission, you can pass into its
constructor:

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new AbpHangfireAuthorizationFilter("MyHangFireDashboardPermissionName") }
    });

**Note**: UseHangfireDashboard should be called after the authentication
middleware in your Startup class (probably as the last line). Otherwise,
authorization will always fail.

#### Limitations

More than one background jobs in a single transaction isn't supported by Hangfire. Because, Hangfire does not participate the current transaction. It does not use the ambient transaction (TransactionScope).

It works with default background job manager since it simply performs a db command and it belongs to the current transaction as expected.
