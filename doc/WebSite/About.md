-   [Considerations](#considerations)
-   [Source codes](#sourcecodes)
-   [Contributors](#contributors)
-   [Contact](#contact)

ASP.NET Boilerplate was created to help developers build applications using
the best software design practices without repeating themselves. DRY -
**Don't Repeat Yourself!** is the key idea behind ASP.NET Boilerplate.

All applications have some common problems and need some common
structures. ASP.NET Boilerplate works for small projects to large
enterprise web applications, providing a quick start with maintainable
code bases.

### Considerations

Keep these concepts in mind while developing with ASP.NET Boilerplate.

#### Modularity

It should be easy to share [entities](/Pages/Documents/Entities),
[repositories](/Pages/Documents/Repositories),
[services](/Pages/Documents/Application-Services) and views between web
applications. They should be packaged into
[modules](/Pages/Documents/Module-System) and can be easily distributed
(preferred as public/private NuGet packages). Modules may depend on and
use other modules. We should be able to extend models in a module for
our application needs.

Modularity provides us with "code re-usability" (DRY!). For example, we
may develop a module that contains user management, role management,
login and error pages which can be shared by different applications.

#### Best practices

An application should be developed using the best software design
principles. Using [dependency
injection](/Pages/Documents/Dependency-Injection) is one of the most
important subjects in this area. AOP (Aspect Oriented Programming) is
used where it's needed and possible, especially for [cross-cutting
concerns](http://en.wikipedia.org/wiki/Cross-cutting_concern). The
application should also correctly use architectural patterns such as MVC
and MVVM, and it should follow
[SOLID](http://en.wikipedia.org/wiki/SOLID_(object-oriented_design))
principles

Following these best practices makes our code-base more understandable
and extensible. It also prevents us from falling in to common mistakes
that have already been experienced by other developers.

#### Scalable code base

The architecture of an application should provide and enforce a way of
keeping a maintainable code base.
[Layering](/Pages/Documents/NLayer-Architecture) and
[modularity](/Pages/Documents/Module-System) are the main techniques to
accomplish that. Following the best practices is important, otherwise
the application gets complicated when it grows. Many applications have
been rewritten because the code became too unmaintainable.

#### Libraries & Frameworks

An application should use and combine useful libraries & frameworks to
accomplish well-known tasks. It should not try to re-invent the wheel if
an existing tool meets its requirements, and it should focus on its
own job (do its own business logic) as much as possible. The
application may use
[EntityFramework](/Pages/Documents/EntityFramework-Integration) or
[NHibernate](/Pages/Documents/NHibernate-Integration) for
Object-Relational Mapping, and it may also use
[Angular](https://angular.io/) or
[DurandalJs](http://durandaljs.com/) as a Single-Page Application
framework.

Like it or not, we need to learn many different tools to build an
application, even if it's more complicated on the client side. There are
many libraries (thousands of jQuery plug-ins for instance) and
frameworks, so we should carefully choose our libraries and adapt them
for our application.

ASP.NET Boilerplates composes and combines some of the best tools out
there, but it also does not prevent you from using your own favourite
tools.

#### Cross-cutting concerns

Authorization,
[validation](/Pages/Documents/Validating-Data-Transfer-Objects), [error
handling](/Pages/Documents/Handling-Exceptions),
[logging](/Pages/Documents/Logging), caching are common things all
applications implement at some level. The code should be generic and
shared by different applications. It should also be separated from the
business logic code and should be automated as much as possible. This
allows us to focus more on our application specific business logic and
prevents us from re-coding the same stuff over and over again (DRY!).

#### More automation

If it can be automated, it should be automated (at least in most cases).
Database migrations, unit tests, and deployments are some of the tasks
that can be automated. Automation saves us time in a long term and
prevents from making mistakes of manual tasks (DRY!).

#### Convention over configuration

[Convention over
configuration](http://en.wikipedia.org/wiki/Convention_over_configuration)
is a very popular software design principle. An application framework
should implement defaults as much as possible. It should be easy when
following conventions but also configurable when needed.

#### Project startup

It should be easy and fast to start a new application. We should not
repeat some tedious steps to create an empty application (DRY!). A
Project/Solution [templates](/Templates) is a proper way of doing it.

### Source code

ASP.NET Boilerplate is an open source project developed on GitHub.

-   Source code:
    <https://github.com/aspnetboilerplate/aspnetboilerplate>
-   Project templates:
    <https://github.com/aspnetboilerplate/aspnetboilerplate-templates>
-   Sample projects:
    <https://github.com/aspnetboilerplate/aspnetboilerplate-samples>
-   Module Zero: <https://github.com/aspnetboilerplate/module-zero>

### Contributors

ASP.NET Boilerplate is designed and developed by [Halil İbrahim
Kalkan](http://www.halilibrahimkalkan.com/). There are also many
[contributors](https://github.com/aspnetboilerplate/aspnetboilerplate/graphs/contributors)
on GitHub. Please feel free to fork our repositories and send pull
requests!

### Contact

For your questions and other discussions, use [official
forum](http://forum.aspnetboilerplate.com/).

For feature requests or bug reports, use [GitHub
issues](https://github.com/aspnetboilerplate/aspnetboilerplate/issues).

For personal contact with me, visit my [web
page](http://halilibrahimkalkan.com/contact/).
