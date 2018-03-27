### Introduction

The ASP.NET Boilerplate framework is designed to be independent of any
database schema and to be as generic as possible. Therefore, it leaves
some concepts **abstract** and **optional**, like audit logging, session
management and authorization, which require some sort of **data store**.

**Module Zero** implements all the fundamental concepts of the ASP.NET
Boilerplate framework such as [tenant management](/Pages/Documents/Zero/Tenant-Management)
(**multi-tenancy**), [role management](/Pages/Documents/Zero/Role-Management), [user management](/Pages/Documents/Zero/User-Management),
[session](/Pages/Documents/Abp-Session), [authorization](/Pages/Documents/Authorization) ([permission management](/Pages/Documents/Zero/Permission-Management)), [setting management](/Pages/Documents/Setting-Management),
[language management](/Pages/Documents/Zero/Language-Management), [audit logging](/Pages/Documents/Audit-Logging) and more.

### Microsoft ASP.NET Identity

This module has two versions:

-   The Abp.Zero.\* packages are built on Microsoft ASP.NET Identity and
    Entity Framework 6.x.
-   The Abp.ZeroCore.\* packages are built on Microsoft ASP.NET Core
    Identity and Entity Framework Core. 

See [all packages](Nuget-Packages.md).

### Source code

The source code of this module is also maintained in [the main GitHub repository](https://github.com/aspnetboilerplate/aspnetboilerplate/tree/dev/src) (Abp.Zero.* and Abp.ZeroCore.* projects).
