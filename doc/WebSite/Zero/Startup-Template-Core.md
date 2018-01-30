### Introduction

The easiest way of starting a new project using ABP with **ASP.NET Core MVC** is to create a template on the [templates page](/Templates). After creating and downloading your project, follow below steps to run your application.

-   Open your solution on **Visual Studio 2017 v15.3.5+** and **build**
    the solution.
-   Select the '**Web.Mvc**' project as startup project.
-   Check **connection string** in the **appsettings.json** file of the Web.Mvc project, change if you want.
-   Open **Package Manager Console** and run **Update-Database** command
    to create your database (ensure that Default project is selected as
    **.EntityFrameworkCore** in the Package Manager Console window).
-   Run the application.

If you have problems with running the application, please try close and
open your Visual Studio again. It sometimes fail on first package
restore.

#### Login

Once you run the application, you will see a login page as shown below:

<img src="../images/module-zero-core-template-ui-login.png" alt="Login Page" class="img-thumbnail" />

User name is '**admin**' and password is '**123qwe**' as default. There
is also a "Default" tenant by default. After login, you can see the
sample dashboard page:

<img src="../images/module-zero-core-template-ui-home.png" alt="Dashboard" class="img-thumbnail" />

#### About Multi Tenancy

In this template, **multi-tenancy is enabled by default**. You can
disable it in Core project's module class if you don't need.

### Token Based Authentication

Startup template uses cookie based authentication for browsers. However,
if you want to consume Web APIs or application services (those are
exposed via [dynamic web api](/Pages/Documents/Dynamic-Web-API)) from a
mobile application, you probably want a token based authentication
mechanism. Startup template includes JwtBearer token authentication
infrastructure.

Here, **Postman** (chrome extension) will be used to demonstrate
requests and responses.

#### Authentication

Just send a **POST** request to
**http://localhost:62114/api/TokenAuth/Authenticate** with
**Context-Type="application/json"** header as shown below:

<img src="../images/aspnet-core-token-auth.png" alt="Request for token" class="img-thumbnail" />

We sent values **usernameOrEmailAddress** and **password**. As seen
above, result property of returning JSON contains the token and expire
time (which is 24 hours by default and can be configured). We can save
it and use for next requests.

**About Multi Tenancy  
**API will work as host users by default. You can send **Abp.TenantId**
header value to work with a specified tenant. It's an integer value and
1 for default tenant by default.

#### Use API

After authenticate and get the **token**, we can use it to call any
**authorized** action. All **application services** are available to be
used remotely. For example, we can use the **User service** to get a
**list of users**:

 <img src="../images/token-request-v2.png" alt="Call API" class="img-thumbnail" />

Just made a **GET** request to
**http://localhost:62114/api/services/app/user/GetAll** with
**Content-Type="application/json"** and **Authorization="Bearer
*your-*** ***auth-token*** **"**. Almost all operations available on UI
are also available as API and can be consumed easily.

### Migrator Console Application

Startup template includes a tool, Migrator.exe, to easily migrate your
databases. You can run this application to create/migrate host and
tenant databases.

<img src="../images/database-migrator.png" alt="Database Migrator" class="img-thumbnail" />

This application gets host connection string from it's **own
appsettings.json file**. It will be same in the appsettings.json in the
.Web.Host project at the beggining. Be sure that the connection string
in config file is the database you want. After getting **host**
**connection sring**, it first creates the host database or apply
migrations if it does already exists. Then it gets connection strings of
tenant databases and runs migrations for those databases. It skips a
tenant if it has not a dedicated database or it's database is already
migrated for another tenant (for shared databases between multiple
tenants).

You can use this tool on development or on product environment to
migrate databases on deployment, instead of EntityFramework's own
tooling (which requires some configuration and can work for single
database/tenant in one run).

### Unit Testing

Startup template includes test infrastructure setup and a few tests
under the .Test project. You can check them and write similar tests
easily. Actually, they are integration tests rather than unit tests
since they tests your code with all ASP.NET Boilerplate infrastructure
(including validation, authorization, unit of work...).

### Source Code

This project template is developed as open source and free under Github:
<https://github.com/aspnetboilerplate/module-zero-core-template>
