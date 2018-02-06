### Download Starter Template

Download a starter template with **ASP.NET Core** and **Entity Framework Core** to integrate SqlLite. 
[Multi-page template with **ASP.NET Core 2.x** + **.Net Core Framework** + **Authentication**](https://aspnetboilerplate.com/Templates) 
will be explained in this document.

### Install 

Install [`Microsoft.EntityFrameworkCore.Sqlite`](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/) nuget package to **\*.EntityFrameworkCore** project. 

### Configuration

Some configurations and workarounds are needed to use Sqlite with ASP.NET Core and Entity Framework Core. 

#### Configure DbContext 

Replace `YourProjectNameDbContextConfigurer.cs` with the following lines

```c#
public static class SqliteDemoDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<SqliteDemoDbContext> builder, string connectionString)
    {
        builder.UseSqlite(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<SqliteDemoDbContext> builder, DbConnection connection)
    {
        builder.UseSqlite(connection);
    }
 }
 ```

#### Configure connection string 

Change connection string to your Sqlite connection in ***.Web.Mvc/appsettings.json**. For example:

```js
{
  "ConnectionStrings": {
    "Default": "Data Source=SqliteDemoDb"
  },
  ...
}

```

#### A workaround

To prevent EF Core from calling `Program.BuildWebHost()` rename `BuildWebHost`. For example, change it to `InitWebHost`. 
To understand why it needs to be renamed check the following issues,

> **Reason** : [EF Core 2.0: design-time DbContext discovery changes](https://github.com/aspnet/EntityFrameworkCore/issues/9033)
> 
> **Workaround** : [Design: Allow IDesignTimeDbContextFactory to short-circuit service provider creation](https://github.com/aspnet/EntityFrameworkCore/issues/9076#issuecomment-313278753)
>
> **NOTE :** If you don't rename BuildWebHost, you'll get an error running BuildWebHost method.

### Create Database

Remove all migration classes under **\*.EntityFrameworkCore/Migrations** folder, before creating database.
Because `Microsoft.EntityFrameworkCore.Sqlite` will add some of its own configuration to work with Entity Framework Core.

Now it's ready to build database.

- Select **\*.Web.Mvc** as startup project.
- Open **Package Manager Console** and select **\*.EntityFrameworkCore** project.
- Run `add-migration Initial_Migration` command
- Run `update-database` command

Sqlite integration is complete. Now run your project with Sqlite. 
