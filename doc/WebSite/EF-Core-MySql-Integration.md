
### Download Starter Template

Download a starter template with **ASP.NET Core** and **Entity Framework Core** to integrate SqlLite. 
[Multi-page template with **ASP.NET Core 2.x** + **.Net Core Framework** + **Authentication**](https://aspnetboilerplate.com/Templates) 
will be explained in this document.

### Getting Started

There are two Entity Framework Core providers for MySQL that are mentioned in Micrososft Docs. One of them is 
[Official MySQL EF Core Database Provider](https://docs.microsoft.com/en-us/ef/core/providers/mysql/) and 
other is [Pomelo EF Core Database Provider for MySQL](https://docs.microsoft.com/en-us/ef/core/providers/pomelo/).

> **NOTE:** Official provider doesn't support EF Core 2.0 yet. Pomelo EF Core Database Provider will be used in this example.
> 
> Related issue: https://github.com/aspnet/EntityFrameworkCore/issues/10065#issuecomment-336495475

### Install 

Install [`Pomelo.EntityFrameworkCore.MySql`](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/) nuget package to ***.EntityFrameworkCore** project. 

### Configuration

#### Configure DbContext 

Replace `YourProjectNameDbContextConfigurer.cs` with the following lines

```c#
public static class SqliteDemoDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<MySqlDemoDbContext> builder, string connectionString)
    {
        builder.UseMySql(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<MySqlDemoDbContext> builder, DbConnection connection)
    {
        builder.UseMySql(connection);
    }
 }
 ```

Some configuration and workaround are needed to use MySQL with ASP.NET Core and Entity Framework Core. 
There is no need to add any configuration for ABP.

#### Configure connection string 

Change connection string to your MySQL connection in ***.Web.Mvc/appsettings.json**. For example:

```js
{
  "ConnectionStrings": {
    "Default": "server=127.0.0.1;uid=root;pwd=1234;database=mysqldemodb"
  },
  ...
}

```

#### A workaround

To prevent EF Core from calling `Program.BuildWebHost()` rename `BuildWebHost`. For example, change it to `InitWebHost`. 
To understand why it needs to be renamed, check the following issues:

> **Reason** : [EF Core 2.0: design-time DbContext discovery changes](https://github.com/aspnet/EntityFrameworkCore/issues/9033)
> 
> **Workaround** : [Design: Allow IDesignTimeDbContextFactory to short-circuit service provider creation](https://github.com/aspnet/EntityFrameworkCore/issues/9076#issuecomment-313278753)
>
> **NOTE :** If you don't rename BuildWebHost, you'll get an error running BuildWebHost method.

### Create Database

Then remove all migration classes under **\*.EntityFrameworkCore/Migrations** folder. 
Because `Pomelo.EntityFrameworkCore.MySql` will add some of its own configurations to work with Entity Framework Core.

Now we are ready to create database and run project. 

- Select **\*.Web.Mvc** as startup project.
- Open **Package Manager Console** and select **\*.EntityFrameworkCore** project.
- Run `add-migration Initial_Migration` command
- Run `update-database` command

MySQL integration is complete. Now you can run your project with MySQL. 

