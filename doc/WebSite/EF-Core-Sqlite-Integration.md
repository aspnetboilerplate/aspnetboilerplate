### Download Starter Template

Download a starter template with **ASP.NET Core** and **Entity Framework Core** to integrate SQLite. 
[Multi-page template with **ASP.NET Core 2.x** + **.NET Core Framework** + **Authentication**](https://aspnetboilerplate.com/Templates) 
will be explained in this document.

### Install 

Install the [`Microsoft.EntityFrameworkCore.Sqlite`](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/) NuGet package to the **\*.EntityFrameworkCore** project. 

### Configuration

Some configuration and workarounds are needed to use SQLite with ASP.NET Core and Entity Framework Core. 

#### Configure DbContext 

Since SQLite doesn't support multithreading, transactions should be disabled in the `SQLiteDemoEntityFrameworkModule.PreInitialize()` method.

> NOTE:Check [here](https://github.com/XdX-Software/EasyDDD/issues/1) for more info and workarounds.

```c#
[DependsOn(
    typeof(SQLiteDemoCoreModule), 
    typeof(AbpZeroCoreEntityFrameworkCoreModule))]
public class SQLiteDemoEntityFrameworkModule : AbpModule
{
    public override void PreInitialize()
    {
        ...
        // add this line to disable transactions
        Configuration.UnitOfWork.IsTransactional = false;
        ...
    }
}
```

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

Change the connection string to your SQLite connection in ***.Web.Mvc/appsettings.json**. Example:

```js
{
  "ConnectionStrings": {
    "Default": "Data Source=SqliteDemoDb.db"
  },
  ...
}

```

### Create Database

Remove all migration classes(include `DbContextModelSnapshot`) under **\*.EntityFrameworkCore/Migrations** folder before creating database.
`Microsoft.EntityFrameworkCore.Sqlite` will add some of its own configuration to work with Entity Framework Core.

Now it's ready to build the database.

- Select **\*.Web.Mvc** as the startup project.
- Open **Package Manager Console** and select the **\*.EntityFrameworkCore** project.
- Run the `add-migration Initial_Migration` command
- Run the `update-database` command

The SQLite integration is now complete. You can now run your project with SQLite. 
