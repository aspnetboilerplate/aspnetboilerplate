### What is Dependency Injection?

If you already know the Dependency Injection, Constructor and
Property Injection pattern concepts, you can skip to the [next
section](#abpInfrastructure).

Wikipedia says: "*Dependency injection is a software design pattern in
which one or more dependencies (or services) are injected, or passed by
reference, into a dependent object (or client) and are made part of the
client's state. The pattern separates the creation of a client's
dependencies from its own behavior, which allows program designs to be
loosely coupled and to follow the dependency inversion and single
responsibility principles. It directly contrasts the service locator
pattern, which allows clients to know about the system they use to find
dependencies.*".

It's very hard to manage dependencies and develop a modular and 
well-structured application without using dependency injection techniques.

#### Problems of the Traditional Way

In an application, classes depend on each other. Assume that we have an
[application service](/Pages/Documents/Application-Services) that uses a
[repository](/Pages/Documents/Entities) to insert
[entities](/Pages/Documents/Entities) into a database. In this situation,
the application service class is dependent on the repository class. See
the following example:

    public class PersonAppService
    {
        private IPersonRepository _personRepository;

        public PersonAppService()
        {
            _personRepository = new PersonRepository();            
        }

        public void CreatePerson(string name, int age)
        {
            var person = new Person { Name = name, Age = age };
            _personRepository.Insert(person);
        }
    }
                

**PersonAppService** uses **PersonRepository** to insert a **Person** into
the database. Although this looks harmless, there are some problems with this code:

-   PersonAppService uses the **IPersonRepository** reference for the
    **CreatePerson** method. This method depends on the 
    IPersonRepository interface instead of the PersonRepository concrete class. 
    In the constructor, however, the PersonAppService depends on the PersonRepository
    rather than the interface. Components should depend on interfaces rather than concrete 
    implementations. This is known as the Dependency Inversion principle.
-   If the PersonAppService creates the PersonRepository itself, it
    becomes dependent on a specific implementation of the IPersonRepository
    interface. This can not work with other implementations. Thus,
    separating the interface from the implementation becomes meaningless.
    Hard-dependencies make the code base tightly-coupled, making reusability negligent.
-   We may need to change the creation of PersonRepository in the future.
    Say we want to make it a singleton (single shared instance rather
    than creating an object for each use). Or we may want to create
    more than one class that implements IPersonRepository and want to
    create one of them conditionally. In this situation, we would have to
    change all the classes that depend on IPersonRepository.
-   With such a dependency, it's very hard (or impossible) to unit test
    the PersonAppService.

To overcome some of these problems, the factory pattern can be used. Thus,
the creation of the repository class is abstracted. See the code below:

    public class PersonAppService
    {
        private IPersonRepository _personRepository;

        public PersonAppService()
        {
            _personRepository = PersonRepositoryFactory.Create();            
        }

        public void CreatePerson(string name, int age)
        {
            var person = new Person { Name = name, Age = age };
            _personRepository.Insert(person);
        }
    }
                

PersonRepositoryFactory is a static class that creates and returns an
IPersonRepository. This is known as the **Service Locator** pattern.
Creation problems are solved since PersonAppService does not know how to
create an implementation of IPersonRepository and it's independent from
the PersonRepository implementation. There are still a few problems:

-   At this time, PersonAppService depends on PersonRepositoryFactory.
    This is more acceptable, but there is still a hard-dependency.
-   It's tedious to write a factory class/method for each repository or
    for each dependency.
-   Again, it's not easy to test, since it's hard to make
    PersonAppService use a mock implementation of
    IPersonRepository.

#### Solution

There are some best practices (patterns) to help us depend on other classes.

##### Constructor Injection Pattern

The example above can be re-written as shown below:

    public class PersonAppService
    {
        private IPersonRepository _personRepository;

        public PersonAppService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public void CreatePerson(string name, int age)
        {
            var person = new Person { Name = name, Age = age };
            _personRepository.Insert(person);
        }
    }
                

This is known as **constructor injection**. PersonAppService does
not know which classes implement IPersonRepository or how it is created.
When an PersonAppService is needed, we first create an IPersonRepository
and pass it to the constructor of the PersonAppService:

    var repository = new PersonRepository();
    var personService = new PersonAppService(repository);
    personService.CreatePerson("John Doe", 32);

Constructor Injection is a great way of making a class independent to the
creation of dependent objects, but there are some problems with the code
above:

-   Creating a PersonAppService becomes harder. It has 4 dependencies. 
    We must create these 4 dependent objects and pass them
    into the constructor of the PersonAppService.
-   Dependent classes may have other dependencies (Here,
    PersonRepository has dependencies). We have to create all the
    dependencies of PersonAppService, all the dependencies of these dependencies
    and so on and so forth.. We might not even be able to create a single object
    because the dependency graph is too complex!

Fortunately, there are [Dependency Injection
frameworks](#dIFrameworks), which automate the management of dependencies.

##### Property Injection pattern

The constructor injection pattern is a great way of providing the dependencies
of a class. In this way, you can not create an instance of the class
without supplying dependencies. It's also a strong way of explicitly
declaring what the requirements are of the class so that it can work properly.

In some situations the class may depend on another class, but can
work without it. This is usually true for cross-cutting concerns such as
logging. A class can work without logging, but it can write logs if you
supply a logger to it. In this case, you can define dependencies as public
properties rather than getting them in constructor. Think about how we would write
to logs in PersonAppService. We can re-write the class like this:

    public class PersonAppService
    {
        public ILogger Logger { get; set; }

        private IPersonRepository _personRepository;

        public PersonAppService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
            Logger = NullLogger.Instance;
        }

        public void CreatePerson(string name, int age)
        {
            Logger.Debug("Inserting a new person to database with name = " + name);
            var person = new Person { Name = name, Age = age };
            _personRepository.Insert(person);
            Logger.Debug("Successfully inserted!");
        }
    }

The NullLogger.Instance is a singleton object that implements ILogger, but
it doesn't do anything. It does not write logs. It implements ILogger with
empty method bodies. PersonAppService can then write logs if you
set the Logger property after creating the PersonAppService object:

    var personService = new PersonAppService(new PersonRepository());
    personService.Logger = new Log4NetLogger();
    personService.CreatePerson("John Doe", 32);

Assume that Log4NetLogger implements ILogger and it writes logs using the
Log4Net library so that PersonAppService can actually write logs. If we
do not set the Logger, it does not write logs. We can say that ILogger is
an **optional dependency** of PersonAppService.

Almost all Dependency Injection frameworks support the Property Injection
pattern.

##### Dependency Injection frameworks

There are many dependency injection frameworks that automate resolving
dependencies. They can create objects with all the dependencies, and the
dependencies of dependencies, recursively. Simply write your classes
with the constructor & property injection patterns, and the DI framework will handle the
rest! In a good application, your classes are independent even from the DI
framework. There will only be a few lines of code or classes that explicitly
interact with the DI framework in your whole application.

ASP.NET Boilerplate uses the [Castle
Windsor](https://github.com/castleproject/Windsor/blob/master/docs/README.md)
framework for Dependency Injection. It's one of the most mature DI
frameworks out there. There are many other frameworks, such as Unity, Ninject,
StructureMap, and Autofac.

In a dependency injection framework, you first register your
interfaces/classes to the dependency injection framework, and then 
resolve (create) an object. In Castle Windsor, it's something like that:

    var container = new WindsorContainer();

    container.Register(
            Component.For<IPersonRepository>().ImplementedBy<PersonRepository>().LifestyleTransient(),
            Component.For<IPersonAppService>().ImplementedBy<PersonAppService>().LifestyleTransient()
        );

    var personService = container.Resolve<IPersonAppService>();
    personService.CreatePerson("John Doe", 32);

First, we created the **WindsorContainer** and **registered** 
PersonRepository and PersonAppService with their interfaces. We then
used the container to create an IPersonAppService. It created the concrete class
PersonAppService with it's dependencies and then returned it. In this simple
example, it may not be clear what the advantages are of using a DI framework.
You will, however, have many classes and dependencies in a real enterprise
application. The registration of dependencies are separated from the creation and use of 
objects, and is made only once during the application's startup.

Note that we also set the **life cycle** of the objects as **transient**.
This means that whenever we resolve an object of these types, a new instance
is created. There are many different life cycles, such as the **singleton**, 
for example.

### ASP.NET Boilerplate Dependency Injection Infrastructure

ASP.NET Boilerplate makes using the dependency injection framework almost 
invisible. It also helps you write your application by following the best
practices and conventions.

#### Registering Dependencies

There are different ways of registering your classes to the Dependency
Injection system in ASP.NET Boilerplate. Most of time, conventional
registration will be sufficient.

##### Conventional Registrations

ASP.NET Boilerplate automatically registers all
[Repositories](/Pages/Documents/Repositories), [Domain
Services](/Pages/Documents/Domain-Services), [Application
Services](/Pages/Documents/Application-Services), MVC Controllers and
Web API Controllers by convention. For example, you may have a
IPersonAppService interface and a PersonAppService class that implements
it:

    public interface IPersonAppService : IApplicationService
    {
        //...
    }

    public class PersonAppService : IPersonAppService
    {
        //...
    }

ASP.NET Boilerplate automatically registers it since it implements the
**IApplicationService** interface (it's just an empty interface). It is
registered as **transient**, meaning it is created each time, per use. When you
inject (using constructor injection) the IPersonAppService interface into a
class, a PersonAppService object will be created and passed into the constructor,
automatically.

**Naming conventions** are very important here. For example, you can
change the name of PersonAppService to MyPersonAppService or another name
which contains the 'PersonAppService' postfix. This registers it to IPersonAppService
because it has the same postfix. You can not, however, name your service without the postfix,
such as 'PeopleService'. If you do so, it's not registered to the IPersonAppService automatically. Instead, it's
registered to the DI framework using self-registration (not the
interface). In this case, you can manually register it.

ASP.NET Boilerplate can register assemblies by convention. It's pretty
easy:

    IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

Assembly.GetExecutingAssembly() gets a reference to the assembly which
contains this code. You can pass other assemblies to the
RegisterAssemblyByConvention method. This is generally done when your
module is being initialized. See ASP.NET Boilerplate's [module
system](/Pages/Documents/Module-System) for more info.

You can write your own conventional registration class by implementing the
**IConventionalRegisterer** interface and then calling the
**IocManager.AddConventionalRegisterer** method in your class. You
should add it in pre-initialize method of your module.

##### Helper Interfaces

You may want to register a specific class that does not fit into the
conventional registration rules. ASP.NET Boilerplate provides the
**ITransientDependency**, the **IPerWebRequestDependency** and the **ISingletonDependency** interfaces as a
shortcut. For example:

    public interface IPersonManager
    {
        //...
    }

    public class MyPersonManager : IPersonManager, ISingletonDependency
    {
        //...
    }

In this way, you can easily register MyPersonManager. When you need to
inject IPersonManager, the MyPersonManager class is used. Note that the
dependency is declared as a **Singleton**. A single instance of
MyPersonManager is created and the same object is passed to all needed
classes. It's instantiated in it's first use, and then used in the
whole life of the application.

**NOTE:** The **IPerWebRequestDependency** can only be used in the web layer.

##### Custom/Direct Registration

If conventional registrations are not sufficient for your needs, you
can either use the **IocManager** or **Castle Windsor** to register your
classes and dependencies.

###### Using IocManager

You can use the **IocManager** to register dependencies (generally in the
PreInitialize method of your [module definition](Module-System.md) class):

    IocManager.Register<IMyService, MyService>(DependencyLifeStyle.Transient);

Using the Castle Windsor API

You can use the **IIocManager.IocContainer** property to access the
Castle Windsor Container and register dependencies. Example:

    IocManager.IocContainer.Register(Classes.FromThisAssembly().BasedOn<IMySpecialInterface>().LifestylePerThread().WithServiceSelf());

For more information, read [Windsor's
documentation](https://github.com/castleproject/Home/blob/master/README.md). 

#### Resolving Dependencies

Registration informs the IOC (Inversion of Control) Container (a.k.a. the DI
framework) about your classes, their dependencies and lifetimes.
Somewhere in your application you need to create objects using an IOC
Container. ASP.NET Provides a few options for resolving dependencies.

#### Constructor & Property Injection

As a **best practice**, you should use constructor and property injection to get the
dependencies to your classes. You should do it this way whenever possible. 
Example:

    public class PersonAppService
    {
        public ILogger Logger { get; set; }

        private IPersonRepository _personRepository;

        public PersonAppService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
            Logger = NullLogger.Instance;
        }

        public void CreatePerson(string name, int age)
        {
            Logger.Debug("Inserting a new person to database with name = " + name);
            var person = new Person { Name = name, Age = age };
            _personRepository.Insert(person);
            Logger.Debug("Successfully inserted!");
        }
    }

IPersonRepository is injected from the constructor and ILogger is injected
with a public property. In this way, your code will be unaware of the
dependency injection system at all. This is the most proper way of using
DI system.

#### IIocResolver, IIocManager and IScopedIocResolver

You may have to directly resolve your dependency instead of using constructor
& property injection. This should be avoided when possible, but it may
be impossible. ASP.NET Boilerplate provides some services that can be
injected and used easily. Example:

    public class MySampleClass : ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public MySampleClass(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public void DoIt()
        {
            //Resolving, using and releasing manually
            var personService1 = _iocResolver.Resolve<PersonAppService>();
            personService1.CreatePerson(new CreatePersonInput { Name = "John", Surname = "Doe" });
            _iocResolver.Release(personService1);

            //Resolving and using in a safe way
            using (var personService2 = _iocResolver.ResolveAsDisposable<PersonAppService>())
            {
                personService2.Object.CreatePerson(new CreatePersonInput { Name = "John", Surname = "Doe" });
            }
        }
    }

MySampleClass in an example class in an application. It is
constructor-injected with **IIocResolver** and uses it to resolve and release
objects. There are a few overloads of the **Resolve** method which can be used as
needed. The **Release** method is used to release a component (object). It's
**critical** to call Release if you're manually resolving an object.
Otherwise, your application may have memory leaks. To be sure of
releasing the object, use **ResolveAsDisposable** (as shown in the
example above) wherever possible. Release is automatically called at
the end of the using block.

The IIocResolver (and IIocManager) also have the **CreateScope** extension
method (defined in the Abp.Dependency namespace) to safely release all
resolved dependencies. Example:

    using (var scope = _iocResolver.CreateScope())
    {
        var simpleObj1 = scope.Resolve<SimpleService1>();
        var simpleObj2 = scope.Resolve<SimpleService2>();
        //...
    }

At the end of using block, all resolved dependencies are automatically
removed. A scope is also injectable using the **IScopedIocResolver**. You
can inject this interface and resolve dependencies. When your class is
released, all resolved dependencies will be released. Use this
carefully! If your class has a long life (say it's a singleton), and you 
are resolving too many objects, then all of them will
remain in memory until your class is released.

If you want to directly reach the IOC Container (Castle Windsor) to
resolve dependencies, you can constructor-inject **IIocManager** and use
the **IIocManager.IocContainer** property. If you are in a static context or
can not inject IIocManager, as a last resort, you can use a singleton
object **IocManager.Instance** everywhere. However, in this case your code
will not be easy to test.

#### Extras

##### IShouldInitialize interface

Some classes need to be initialized before their first usage.
IShouldInitialize has an Initialize() method. If you implement it, then
your Initialize() method is automatically called just after creating
your object (before it's used). You need to inject/resolve the object
in order to work with this feature.

#### ASP.NET MVC & ASP.NET Web API integration

We must call the dependency injection system to resolve the root object in
the dependency graph. In an **ASP.NET MVC** application, it's generally a
**Controller** class. We can also use the contructor and property
injection patterns in controllers. When a request gets to our
application, the controller is created using an IOC container and all
dependencies are resolved recursively. What makes this happen? It's all done
automatically by ASP.NET Boilerplate by extending ASP.NET MVC's default
controller factory. This is true for the ASP.NET Web API, too. You don't 
have to worry about creating and disposing objects.

#### ASP.NET Core Integration

ASP.NET Core already has a built-in dependency injection system with the
[Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)
package. ABP uses the 
[Castle.Windsor.MsDependencyInjection](https://www.nuget.org/packages/Castle.Windsor.MsDependencyInjection)
package to integrate it's dependency injection system with ASP.NET
Core's, so you don't have to think about it.

#### Final notes

ASP.NET Boilerplate simplifies and automates dependency injection
as long as you follow the rules and use the structures above. Most of the
time you will not need more. If you do need more, you can directly use the
raw power of Castle Windsor to perform many tasks, like custom registrations,
injection hooks, interceptors and so on.
