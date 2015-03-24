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
- Modular architecture
- Unit of work implementation
- Event bus implementation for domain events
- Easily working with embedded resources
- Auto-creating Web API layer for Application Services
- Auto-creating Javascript proxy layer to use Web API layer
- Javascript helper methods for ajax, notifications, message boxes, making UI busy...

Also adds standard stuff:
- Localization
- Exception handling
- Validation
- Logging

Beside the framework, the first module (https://github.com/aspnetboilerplate/module-zero) is being developed to implement:
- Authentication & Authorization (Implementing ASP.NET Identity Framework)
- User & Role management
- Setting management

Official web site: http://www.aspnetboilerplate.com
Forum: http://forum.aspnetboilerplate.com
