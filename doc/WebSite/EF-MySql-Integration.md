### Introduction

While our default templates are designed to work with SQL Server, you can
easily modify them to work with MySql. In order to do that, you need to
follow these steps.

#### Download Project

Go to <http://aspnetboilerplate.com/Templates> and download a new
project. Select ASP.NET MVC 5.x tab and don't forget to select Entity
Framework.

#### Install MySql.Data.Entity

You then need to install the
[MySql.Data.Entity](https://www.nuget.org/packages/MySql.Data.Entity/)
NuGet package into your **.EntityFramework** and **.Web** projects.
Installing this NuGet package into your **.Web** project should make the
necessary changes in your web.config file.

Open your DbContext's configuration class (Configuration.cs) and place
the following code in it's constructor

    SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());

#### Configure ConnectionString

You need to change your connection string in the web.config file in
order to work with your MySql database. An example connection string
would be:

    <add name="Default" connectionString="server=localhost;port=3306;database=sampledb;uid=root;password=***" providerName="MySql.Data.MySqlClient"/>

#### Re-generate the migrations

If you choose "Include login, register, user, role and tenant management pages" while downloading your startup
template, there will be some migration files included in your project.
These files are generated for Sql Server. Delete all the migration files
in your **.EntityFramework** project under the Migrations folder. Migration
files start with a timestamp. A migration file name should look like this
"201506210746108\_AbpZero\_Initial"

After deleting all the migration files, select your **.Web** project as the
startup project, open Visual Studio's Package Manager Console and select
the **.EntityFramework** project as the default project in the Package Manager
Console. Then run the following command to add a migration for MySql.

    Add-Migration "AbpZero_Initial"

Now you can create your database using the following command

    Update-Database

That's it! You can now run your project with MySql.
