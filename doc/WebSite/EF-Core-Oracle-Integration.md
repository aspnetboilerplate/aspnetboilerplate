### Download Starter Template

Download the starter template with **ASP.NET Core** and **Entity Framework Core** to integrate Oracle. 
[Multi-page template with **ASP.NET Core** + **.NET Core Framework** + **Authentication**](https://aspnetboilerplate.com/Templates) 
will be explained in this document.

### Install

Install the [`Oracle.EntityFrameworkCore`](https://www.nuget.org/packages/Oracle.EntityFrameworkCore/) NuGet package to the ***.EntityFrameworkCore** project.

Delete `Microsoft.EntityFrameworkCore.SqlServer` from ***.EntityFrameworkCore** project because it will not be used anymore.

### Configuration

Some configuration and workarounds are needed to use Oracle with ASP.NET Core and Entity Framework Core. 

#### Configure DbContext 

Replace `YourProjectNameDbContextConfigurer.cs` with the following lines

```c#
public static class PostgreSqlDemoDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<PostgreSqlDemoDbContext> builder, string connectionString)
    {
        builder.UseOracle(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<PostgreSqlDemoDbContext> builder, DbConnection connection)
    {
        builder.UseOracle(connection);
    }
 }
```

#### Configure connection string 

Change the connection string to your PostgreSQL connection in ***.Web.Mvc/appsettings.json** and ***.Migrator/appsettings.json** projects. For example:

```js
{
  "ConnectionStrings": {
    "Default": "Data Source = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = XE))); User Id = system; Password = 123qwe;"
  },
  ...
}
```

### Configure IsolationLevel for UnitOfWork

In order to use Oracle with EF Core, IsolationLevel of the UnitOfWork must be set to ReadCommitted as shown below. You can do this in the PreInitialize method of the **Core module** of your app.

````c#
Configuration.UnitOfWork.IsolationLevel = IsolationLevel.ReadCommitted;
````

### Create Database

Remove all migration classes(including `DbContextModelSnapshot`) under **\*.EntityFrameworkCore/Migrations** folder.
because `Oracle.EntityFrameworkCore` will add some of its own configuration to work with Entity Framework Core.

Now it's ready to build database.

- Open a terminal or command prompt and go to root directory of **\*.EntityFrameworkCore** project.
- Run `dotnet ef migrations add Initial_Migration` command.
- Place the code below to PreInitialize method of the Migrator project's Core module;

```c#
Configuration.UnitOfWork.IsTransactional = false;
```

- Then, run the Migrator project to create and seed your database.

The Oracle integration is now complete. You can now run your project with Oracle.