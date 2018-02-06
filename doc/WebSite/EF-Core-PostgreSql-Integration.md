### Download Starter Template

I will use a starter template with **ASP.NET Core** and **Entity Framework Core** to integrate PostgreSQL.
I downloaded a multi-page template with **ASP.NET Core 2.x** + **.NET Core** + **Authentication** from [https://aspnetboilerplate.com/Templates](https://aspnetboilerplate.com/Templates).

### Install

Install [`Npgsql.EntityFrameworkCore.PostgreSQL`](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/) NuGet package to ***.EntityFrameworkCore** project.

### Configuration

We need some configuration and a workaround to use PostgreSQL with ASP.NET Core and Entity Framework Core.

#### Configure connection string

Change the connection string to your PostgreSQL connection in ***.Web.Mvc/appsettings.json**. For example:

```js
{
  "ConnectionStrings": {
    "Default": "User ID=postgres;Password=123;Host=localhost;Port=5432;Database=PostgreSqlDemoDb;Pooling=true;"
  },
  ...
}
```

#### A workaround

To prevent EF Core from calling `Program.BuildWebHost()`, rename `BuildWebHost`. For example, I will change it to `InitWebHost`.
I won't explain this workaround. To understand it better, check the following issues:

> **Reason**: [EF Core 2.0: design-time DbContext discovery changes](https://github.com/aspnet/EntityFrameworkCore/issues/9033)
>
> **Workaround**: [Design: Allow IDesignTimeDbContextFactory to short-circuit service provider creation](https://github.com/aspnet/EntityFrameworkCore/issues/9076#issuecomment-313278753)
>
> **NOTE**: If you don't rename `BuildWebHost`, you will get an error about running `BuildWebHost` method.

### Create Database

Before you create the database, you should change the max length of the "Value" property of ApplicationLanguageText by adding the following lines to DbContext.
This is because the max length of char in MS SQL and PostgreSQL are different.

```c#
public class PostgreSqlDemoDbContext : AbpZeroDbContext<Tenant, Role, User, PostgreSqlDemoDbContext>
{
    public PostgreSqlDemoDbContext(DbContextOptions<PostgreSqlDemoDbContext> options)
        : base(options)
    {
    }

    // add these lines to override max length of property
    // we should set max length smaller than the PostgreSQL allowed size (10485760)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationLanguageText>()
            .Property(p => p.Value)
            .HasMaxLength(100); // any integer that is smaller than 10485760
    }
}
```

Then remove all migration classes in ***.EntityFrameworkCore/Migrations** folder,
because `Npgsql.EntityFrameworkCore.PostgreSQL` will add some of its own configuration to work with Entity Framework Core.

We are ready to create the database and run the project.

- Select **\*.Web.Mvc** as startup project.
- Open **Package Manager Console** and select **\*.EntityFrameworkCore** project.
- Run `add-migration Initial_Migration` command.
- Run `update-database` command.

PostgreSQL integration is complete. Now you can run your project with PostgreSQL.

### Summary

The image below shows all the steps for integrating PostgreSQL.

<img src="images/postgresql-integration-summary.png" alt="PostgreSQL integration summary" class="img-thumbnail" />
