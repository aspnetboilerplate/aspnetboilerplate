ASP.NET Boilerplate
===

ASP.NET Boilerplate is a starting point for new modern ASP.NET MVC web applications with best practices and most popular tools!

ASP.NET Boilerplate implements __NLayer architecture__ and __Domain Driven Design__ with these tools:

- Server side
  - ASP.NET MVC and ASP.NET Web API (as web framework)
  - Castle windsor (as Dependency Injection container)
  - EntityFramework or NHibernate+FluentNHibernate+FluentMigrations (for ORM and DB migrations)
  - Log4Net (for logging)
  - AutoMapper (for DTO adapters and other mappings)
- Client side
  - Twitter bootstrap (as HTML & CSS framework)
  - Less (as CSS pre-compiler)
  - AngularJs or DurandalJs (as Single Page Application (SPA) framework)
  - jQuery (as DOM & Ajax library)
  - Modernizr (for feature detection)
  - Other JS libraries: jQuery.validate, jQuery.form, jQuery.blockUI, json2

It adds it's own techniques such as:
- Auto-creating Web API layer for Application Services
- Auto-creating Javascript proxy layer to use Web API layer

Also adds standard stuff:
- Authentication & Authorization
- User & Role management
- Exception handling
- Validation

and so on... See http://www.aspnetboilerplate.com for more.

An overall view of layers and tools:

![ScreenShot](https://raw.github.com/hikalkan/aspnetboilerplate/master/AbpOverall.png)
