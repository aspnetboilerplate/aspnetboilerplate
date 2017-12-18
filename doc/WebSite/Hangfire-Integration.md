### Introduction

[Hangfire](http://hangfire.io/) is a compherensive background job
manager. You can **integrate** ASP.NET Boilerplate with Hangfire to use
it instead of [default background job
manager](/Pages/Documents/Background-Jobs-And-Workers). You can use the
**same background job API** for Hangfire. Thus, your code will be
**independent** of Hangfire. But, if you like, you can directly use
**Hangfire's API** also.

Hangfire Integration depends on the frameworks you are using.

### ASP.NET Core Integration

[**Abp.HangFire.AspNetCore**](https://www.nuget.org/packages/Abp.HangFire.AspNetCore)
package is used to integrate to ASP.NET Core based applications. It
depends on
[Hangfire.AspNetCore](https://www.nuget.org/packages/Hangfire.AspNetCore/).
[This
document](https://www.hangfire.io/blog/2016/07/16/hangfire-1.6.0.html)
describes to install hangfire to an ASP.NET Core project. It's similar
for ABP based projects too. First install
[Abp.HangFire.AspNetCore](https://www.nuget.org/packages/Abp.HangFire.AspNetCore)
package to your web project:

    Install-Package Abp.HangFire.AspNetCore

Then you can install any storage for Hangfire. Most common one is SQL
Server storage (see
[**Hangfire.SqlServer**](https://www.nuget.org/packages/Hangfire.SqlServer)
nuget package). After you installed these nuget packages, you can
**configure** your project to use Hangfire.

First, we are changing Startup class to add Hangfire to dependency
injection and configure storage and connection string in the
**ConfigureServices** method:

    services.AddHangfire(config =>
    {
        config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
    });

Then we can add UseHangfireServer call in the **Configure** method:

    app.UseHangfireServer();

If you want to use hangfire's dashboard, you can add it too:

    app.UseHangfireDashboard();

If you want to [authorize](Authorization.md) the dashboard, you can
use AbpHangfireAuthorizationFilter as shown below:

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new AbpHangfireAuthorizationFilter("MyHangFireDashboardPermissionName") }
    });

The configuration above is almost standard to integrate hangfire to an
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

We added **AbpHangfireAspNetCoreModule** as a dependency and used
Configuration.BackgroundJobs.**UseHangfire** method to replace Hangfire
for ABP's default background job manager.

Hangfire requires schema creation permission in your database since it
creates it's own schema and tables on first run. See [Hangfire
documentation](http://docs.hangfire.io/en/latest/) for more information.

### ASP.NET MVC 5.x Integration

[**Abp.HangFire**](https://www.nuget.org/packages/Abp.HangFire) nuget
package is used for ASP.NET MVC 5.x projects:

    Install-Package Abp.HangFire

Then you can install any storage for Hangfire. Most common one is SQL
Server storage (see
[**Hangfire.SqlServer**](https://www.nuget.org/packages/Hangfire.SqlServer)
nuget package). After you installed these nuget packages, you can
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

We added **AbpHangfireModule** as a dependency and used
Configuration.BackgroundJobs.**UseHangfire** method to enable and
configure Hangfire ("Default" is the connection string in web.config).

Hangfire requires schema creation permission in your database since it
creates it's own schema and tables on first run. See [Hangfire
documentation](http://docs.hangfire.io/en/latest/) for more information.

#### Dashboard Authorization

Hagfire can show a **dashboard page** to see status of all background
jobs in real time. You can configure it as described in it's
[documentation](http://docs.hangfire.io/en/latest/configuration/using-dashboard.html).
By default, this dashboard page is available for all users, not
authorized. You can integrate it to ABP's [authorization
system](Authorization.md) using **AbpHangfireAuthorizationFilter**
class defined in Abp.HangFire package. Example configuration:

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new AbpHangfireAuthorizationFilter() }
    });

This checks if current user has logged in to the application. If you
want to require an additional permission, you can pass into it's
constructor:

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new AbpHangfireAuthorizationFilter("MyHangFireDashboardPermissionName") }
    });

**Note**: UseHangfireDashboard should be called after authentication
middleware in your Startup class (probably as the last line). Otherwise,
authorization always fails.
