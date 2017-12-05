### Introduction

Layering of an application's codebase is a widely accepted technique to
help reduce complexity and improve code reusability. To achieve layered
architecture, ASP.NET Boilerplate follows the principles of **Domain
Driven Design**.

### Domain Driven Design Layers

There are four fundamental layers in Domain Driven Design (DDD):

-   **Presentation Layer**: Provides an interface to the user. Uses the
    Application Layer to achieve user interactions.
-   **Application Layer**: Mediates between the Presentation and Domain
    Layers. Orchestrates business objects to perform specific
    application tasks.
-   **Domain Layer**: Includes business objects and their rules. This is
    heart of the application.
-   **Infrastructure Layer**: Provides generic technical capabilities
    that support higher layers mostly using 3rd-party libraries.

### ASP.NET Boilerplate Application Architecture Model

In addition to DDD, there are also other logical and physical layers in
a modern architected application. The model below is suggested and
implemented for ASP.NET Boilerplate applications. ASP.NET Boilerplate
not only makes to implement this model easier by providing base classes
and services, but also provides [startup templates](/Templates) to
directly start with this model.

[<img src="images/abp-nlayer-architecture.png" alt="ASP.NET Boilerplate NLayer Architecture" class="img-thumbnail" width="1220" height="1236" />](https://raw.githubusercontent.com/aspnetboilerplate/aspnetboilerplate/master/doc/WebSite/images/abp-nlayer-architecture.png)

#### Client Applications

These are remote clients uses the application as a service via HTTP APIs
(API Controllers, [OData](OData-Integration.html) Controllers, maybe
GraphQL endpoint). A remote client can be a SPA, a mobile application or
a 3rd-party consumer. [Localization](Localization.html) and
[Navigation](Navigation.html) can be done inside this applications.

#### Presentation Layer

ASP.NET \[Core\] MVC (Model-View-Controller) can be considered as the
presentation layer. It can be a physical layer (uses application via
HTTP APIs) or a logical layer (directly injects and uses [application
services](Application-Services.html)). In either case it can include
[Localization](Localization.html), [Navigation](Navigation.html),
[Object Mapping](Object-To-Object-Mapping.html),
[Caching](Caching.html), [Configuration
Management](Setting-Management.html), [Audit
Logging](Audit-Logging.html) and so on. It should also deal with
[Authorization](Authorization.html), [Session](Abp-Session.html),
[Features](Feature-Management.html) (for
[multi-tenant](Multi-Tenancy.html) applications) and [Exception
Handling](Handling-Exceptions.html).

#### Distributed Service Layer

This layer is used to serve application/domain functionality via remote
APIs like REST, OData, GraphQL... They don't contain business logic but
only translates HTTP requests to domain interactions or can use
application services to delegate the operation. This layer generally
include [Authorization](Authorization.html), [Caching](Caching.html),
[Audit Logging](Audit-Logging.html), [Object
Mapping](Object-To-Object-Mapping.html), [Exception
Handling](Handling-Exceptions.html), [Session](Abp-Session.html) and so
on...

#### Application Layer

Application layer mainly includes [Application
Services](Application-Services.html) those use domain layer and domain
objects ([Domain Services](Domain-Services.html),
[Entities](Entities.html)...) to perform requested application
functionalities. It uses [Data Transfer
Objects](Data-Transfer-Objects.html) to get data from and to return data
to presentation or distributed service layer. It can also deal with
[Authorization](Authorization.html), [Caching](Caching.html), [Audit
Logging](Audit-Logging.html), [Object
Mapping](Object-To-Object-Mapping.html), [Session](Abp-Session.html) and
so on...

#### Domain Layer

This is the main layer that implements our domain logic. It includes
[Entities](Entities.html), [Value Objects](Value-Objects.html), [Domain
Services](Domain-Services.html) to perform business/domain logic. It can
also include [Specifications](Specifications.html) and trigger [Domain
Events](EventBus-Domain-Events.html). It defines Repository Interfaces
to read and persist entities from data source (generally a DBMS).

#### Infrastructure Layer

Infrastructure layer makes other layers working: It implements
repository interfaces (using [Entity Framework
Core](Entity-Framework-Core.html) for example) to actually work with a
real database, it may include integration to a vendor to [send
emails](Email-Sending.html) and so on. This is not a strict layer below
all layers, but actually supports other layers by implementing abstract
concepts of them.
