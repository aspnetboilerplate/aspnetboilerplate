### Introduction

ASP.NET Boilerplate framework is designed to be independent of any
database schema and to be as generic as possible. Therefore, It leaves
some concepts **abstract** and **optional** (like audit logging, session
management and authorization) which requires some **data store**.

**Module Zero** implements all fundamental concepts of ASP.NET
Boilerplate framework such as [tenant
management](/Pages/Documents/Zero/Tenant-Management)
(**multi-tenancy**), [role
management](/Pages/Documents/Zero/Role-Management), [user
management](/Pages/Documents/Zero/User-Management),
[session](/Pages/Documents/Abp-Session),
[authorization](/Pages/Documents/Authorization) ([permission
management](/Pages/Documents/Zero/Permission-Management)), [setting
management](/Pages/Documents/Setting-Management), [language
management](/Pages/Documents/Zero/Language-Management), [audit
logging](/Pages/Documents/Audit-Logging) and so on.

Module-Zero defines entities and implements **domain logic** (domain
layer). It leaves application and presentation layers to the
application. You can use the [startup
template](/Pages/Documents/Zero/Startup-Template) to create your
application based on module zero.

### Microsoft ASP.NET Identity

This module has 2 versions:

-   Abp.Zero.\* packages are built on Microsoft ASP.NET Identity and
    Entity Framework 6.x.
-   Abp.ZeroCore.\* packages are built on Microsoft ASP.NET Core
    Identity and Entity Framework Core. These packages supports .net
    core too.

### Source code

Module-Zero source code is seperated from ASP.NET Boilerplate and hosted
on <https://github.com/aspnetboilerplate/module-zero>. It's distributed
on [nuget](/Pages/Documents/Zero/Nuget-Packages) as the framework.
