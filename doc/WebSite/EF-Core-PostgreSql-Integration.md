### Download Starter Template

Download the starter template with **ASP.NET Core** and **Entity Framework Core** to integrate PostgreSQL. 
[Multi-page template with **ASP.NET Core 2.x** + **.NET Core Framework** + **Authentication**](https://aspnetboilerplate.com/Templates) 
will be explained in this document.

### Install

Install the [`Npgsql.EntityFrameworkCore.PostgreSQL`](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/) NuGet package to the ***.EntityFrameworkCore** project.

### Configuration

Some configuration and workarounds are needed to use PostgreSQL with ASP.NET Core and Entity Framework Core. 

#### Configure DbContext 

Replace `YourProjectNameDbContextConfigurer.cs` with the following lines

```c#
public static class PostgreSqlDemoDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<PostgreSqlDemoDbContext> builder, string connectionString)
    {
        builder.UseNpgsql(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<PostgreSqlDemoDbContext> builder, DbConnection connection)
    {
        builder.UseNpgsql(connection);
    }
 }
```

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
    	base.OnModelCreating(modelBuilder);
    	
        modelBuilder.Entity<ApplicationLanguageText>()
            .Property(p => p.Value)
            .HasMaxLength(100); // any integer that is smaller than 10485760
    }
}
```

Remove all migration classes(include `DbContextModelSnapshot`) under **\*.EntityFrameworkCore/Migrations** folder.
because `Npgsql.EntityFrameworkCore.PostgreSQL` will add some of its own configuration to work with Entity Framework Core.

Now it's ready to build database.

- Select **\*.Web.Mvc** as the startup project.
- Open **Package Manager Console** and select the **\*.EntityFrameworkCore** project.
- Run the `add-migration Initial_Migration` command.
- Run the `update-database` command.

The PostgreSQL integration is now complete. You can now run your project with PostgreSQL.

**Note:** By default ABP's UnitOfWork uses IsolationLevel.ReadUncommitted but it is treated as IsolationLevel.ReadCommitted in PostgreSQL. This behavior can cause problems in some cases. For more information [see](https://github.com/aspnetboilerplate/aspnetboilerplate/issues/3369#issuecomment-433733606).