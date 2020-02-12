### Introduction

The [Abp.EntityFrameworkCore](https://www.nuget.org/packages/Abp.EntityFrameworkCore)
NuGet package is used to integrate the Entity Framework (EF) Core ORM
framework. After installing this package, you need to also add a
[DependsOn](Module-System.md) attribute for
**AbpEntityFrameworkCoreModule**.

### DbContext

EF Core requires you to define a class derived from DbContext. In ABP, we
need to derive from the **AbpDbContext** as shown below:

    public class MyDbContext : AbpDbContext
    {
        public DbSet<Product> Products { get; set; }
    
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }

The constructor should get a **DbContextOptions&lt;T&gt;** as shown above.
The parameter name must be **options**. It's not possible to change it
because ABP provides it as an anonymous object parameter.

### Configuration

#### The Startup Class

Use the **AddAbpDbContext** method on the service collection in
the **ConfigureServices** method as shown below:

    services.AddAbpDbContext<MyDbContext>(options =>
    {
        options.DbContextOptions.UseSqlServer(options.ConnectionString);
    });

For non web projects, we will not have a Startup class. In this case, we
can use the **Configuration.Modules.AbpEfCore().AddDbContext** method in our
[module](Module-System.md) class to configure the DbContext, shown
below:

    Configuration.Modules.AbpEfCore().AddDbContext<MyDbContext>(options =>
    {
        options.DbContextOptions.UseSqlServer(options.ConnectionString);
    });

We used the given connection string and Sql Server as the database
provider. Normally, **options.ConnectionString** is the **default connection
string** (see next section). However, ABP can use
IConnectionStringResolver to determine it. This behaviour can be
changed and the connection string can be determined dynamically. The action
passed to AddDbContext is called whenever a DbContext instance will be
created. You also have a chance to return different connection
strings, conditionally.

Where do we set the default connection string?

#### In the module PreInitialize method

You can do it in the PreInitialize method of your module as shown below:

    public class MyEfCoreAppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = GetConnectionString("Default");
            ...
        }
    }

You can define the GetConnectionString method, which simply returns the
connection string from a configuration file. This is generally in the
appsettings.json file.

### Repositories

Repositories are used to abstract data access from higher layers. See the
[repository documentation](Repositories.md) for more info. 

#### Default Repositories

[Abp.EntityFrameworkCore](http://www.nuget.org/packages/Abp.EntityFrameworkCore)
implements default repositories for all the entities defined in your
DbContext. You don't have to create repository classes to use predefined
repository methods. Example:

    public class PersonAppService : IPersonAppService
    {
        private readonly IRepository<Person> _personRepository;
    
        public PersonAppService(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }
    
        public void CreatePerson(CreatePersonInput input)
        {        
            person = new Person { Name = input.Name, EmailAddress = input.EmailAddress };
    
            _personRepository.Insert(person);
        }
    }

The PersonAppService contructor-injects **IRepository&lt;Person&gt;** and
uses the **Insert** method. In this way, you can easily inject
**IRepository&lt;TEntity&gt;** (or IRepository&lt;TEntity,
TPrimaryKey&gt;) and use the predefined methods.

#### Custom Repositories

If standard repository methods are not sufficient, you can create custom
repository classes for your entities.

##### Application Specific Base Repository Class

ASP.NET Boilerplate provides a base class **EfCoreRepositoryBase** to
implement repositories easily. To implement the **IRepository** interface,
you can simply derive your repository from this class. It's better to
create your own base class that extends EfRepositoryBase. This way, you can
easily add shared and common methods to your repositories. Here's an example base
class for all the repositories of a *SimpleTaskSystem* application:

    //Base class for all repositories in my application
    public class SimpleTaskSystemRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<SimpleTaskSystemDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public SimpleTaskSystemRepositoryBase(IDbContextProvider<SimpleTaskSystemDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    
        //add common methods for all repositories
    }
    
    //A shortcut for entities which have an integer Id
    public class SimpleTaskSystemRepositoryBase<TEntity> : SimpleTaskSystemRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        public SimpleTaskSystemRepositoryBase(IDbContextProvider<SimpleTaskSystemDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    
        //do not add a method here, add it to the class above (because this class inherits it)
    }

Note that we're inheriting from
EfCoreRepositoryBase&lt;**SimpleTaskSystemDbContext**, TEntity,
TPrimaryKey&gt;. This sets ASP.NET Boilerplate to use the
SimpleTaskSystemDbContext in our repositories.

By default, all repositories for your given DbContext
(SimpleTaskSystemDbContext in this example) are implemented using
EfCoreRepositoryBase. You can replace it with your own repository base
class by adding the **AutoRepositoryTypes** attribute to your
DbContext as shown below:

    [AutoRepositoryTypes(
        typeof(IRepository<>),
        typeof(IRepository<,>),
        typeof(SimpleTaskSystemEfRepositoryBase<>),
        typeof(SimpleTaskSystemEfRepositoryBase<,>)
    )]
    public class SimpleTaskSystemDbContext : AbpDbContext
    {
        ...
    }

##### Custom Repository Example

To implement a custom repository, just derive it from your application
specific base repository class like the one we created above.

Assume that we have a Task entity that can be assigned to a Person
(entity). We also have a Task which has a State (new, assigned, completed... and so on).
We may need to write a custom method to get the list of Tasks with some
conditions and include the AssisgnedPerson property, pre-fetched (included), in a
single database query. See the following code:

    public interface ITaskRepository : IRepository<Task, long>
    {
        List<Task> GetAllWithPeople(int? assignedPersonId, TaskState? state);
    }
    
    public class TaskRepository : SimpleTaskSystemRepositoryBase<Task, long>, ITaskRepository
    {
        public TaskRepository(IDbContextProvider<SimpleTaskSystemDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    
        public List<Task> GetAllWithPeople(int? assignedPersonId, TaskState? state)
        {
            var query = GetAll();
    
            if (assignedPersonId.HasValue)
            {
                query = query.Where(task => task.AssignedPerson.Id == assignedPersonId.Value);
            }
    
            if (state.HasValue)
            {
                query = query.Where(task => task.State == state);
            }
    
            return query
                .OrderByDescending(task => task.CreationTime)
                .Include(task => task.AssignedPerson)
                .ToList();
        }
    }

**We first defined** ITaskRepository and then implemented it.
The **GetAll()** method returns **IQueryable&lt;Task&gt;**. We then add some
**Where** filters using the given parameters. Finally, we call
**ToList()** to get the list of Tasks.

You can also use the **Context** object in the repository methods to reach 
your DbContext and directly use Entity Framework APIs. 

**Note**: Define the custom repository **interface** in the
**domain/core** layer, **implement** it in the **EntityFrameworkCore**
project for layered applications. That way, you can inject the interface
from any project without referencing EF Core.

##### Replacing the Default Repositories

Even if you created a TaskRepository as shown above, any class can
still [inject](Dependency-Injection.md) IRepository&lt;Task, long&gt;
and use it. That's not a problem in most cases. But what if you did an
**override** on a base method in your custom repository? Say that you have
overidden Delete method in your custom repository to add a custom
behaviour on delete. If a class injects IRepository&lt;Task, long&gt;
and usea the default repository to Delete a task, your custom behaviour
will not work. To overcome this issue, you can replace your custom
repository implementation with the default one like shown below:

    Configuration.ReplaceService<IRepository<Task, Guid>>(() =>
    {
        IocManager.IocContainer.Register(
            Component.For<IRepository<Task, Guid>, ITaskRepository, TaskRepository>()
                .ImplementedBy<TaskRepository>()
                .LifestyleTransient()
        );
    });

We registered the TaskRepository for IRepository&lt;Task, Guid&gt;,
ITaskRepository and TaskRepository. This way, any one of these can be injected
to use the TaskRepository.

#### Batch Operations

ASP.NET Boilerplate is integrated with [Entity Framework Plus](https://github.com/zzzprojects/EntityFramework-Plus) to provide batch delete and update operations. In order to use those methods on your repositories, you need to add [Abp.EntityFrameworkCore.EFPlus](https://www.nuget.org/packages/Abp.EntityFrameworkCore.EFPlus) NuGet package to your project. Note that `BatchDeleteAsync` method deletes entities permanently even if the entity is [**soft-delete**](/Pages/Documents/Entities#soft-delete).

#### Repository Best Practices

-   **Use the default repositories** wherever it's possible. You can use a
    default repository even if you have a custom repository for an entity
    (if you are using the standard repository methods).
-   Always create the **repository base class** for your application for
    custom repositories, as defined above.
-   Define **interfaces** for your custom repositories in the **domain
    layer** (.Core project in the startup template). Then define the custom repository
    **classes** in the **.EntityFrameworkCore** project if you want to
    abstract EF Core from your domain/application.

### Other Database Integrations

This document and the examples are based on using MS SQL Server.
The following documents can be followed for different database integrations.

- [MySQL Integration](EF-Core-MySql-Integration.md)
- [PostgreSQL Integration](EF-Core-PostgreSql-Integration.md)
- [SQLite Integration](EF-Core-Sqlite-Integration.md)
- [Oracle Integration](EF-Core-Oracle-Integration.md)
