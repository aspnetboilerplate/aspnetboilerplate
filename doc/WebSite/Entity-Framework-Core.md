### Introduction

[Abp.EntityFrameworkCore](https://www.nuget.org/packages/Abp.EntityFrameworkCore)
nuget package is used to integrate to Entity Framework (EF) Core ORM
framework. After installing this package, we should also add a
[DependsOn](Module-System.html) attribute for
**AbpEntityFrameworkCoreModule**.

### DbContext

EF Core requires to define a class derived from DbContext. In ABP, we
should derive from **AbpDbContext** as shown below:

    public class MyDbContext : AbpDbContext
    {
        public DbSet<Product> Products { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }

Constructor should get a **DbContextOptions&lt;T&gt;** as shown above.
Parameter name must be **options**. It's not possible to change it
because ABP provides it as anonymous object parameter.

### Configuration

#### In Startup Class

Use **AddAbpDbContext** method on service collection in
**ConfigureServices** method as shown below:

    services.AddAbpDbContext<MyDbContext>(options =>
    {
        options.DbContextOptions.UseSqlServer(options.ConnectionString);
    });

For non web projects, we will not have a Startup class. In that case, we
can use **Configuration.Modules.AbpEfCore().AddDbContext** method in our
[module](Module-System.html)class to configure DbContext, as shown
below:

    Configuration.Modules.AbpEfCore().AddDbContext<MyDbContext>(options =>
    {
        options.DbContextOptions.UseSqlServer(options.ConnectionString);
    });

We used given connection string and used Sql Server as database
provider. **options.ConnectionString** is the **default connection
string** (see next section) normally. But ABP uses
IConnectionStringResolver to determine it. So, this behaviour can be
changed and connection string can be determined dynamically. The action
passed to AddDbContext is called whenever a DbContext instance will be
created. So, you also have a chance to return different connection
string conditionally.

So, where to set default connection string?

#### In Module PreInitialize

You can do it in PreInitialize of your module as shown below:

    public class MyEfCoreAppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = GetConnectionString("Default");
            ...
        }
    }

So, you can define GetConnectionString method simply returns the
connection string from a configuration file (generally from
appsettings.json).

### Repositories

Repositories are used to abstract data access from higher layers. See
[repository documentation](Repositories.html) for more. 

#### Default Repositories

[Abp.EntityFrameworkCore](http://www.nuget.org/packages/Abp.EntityFrameworkCore)
implements default repositories for all entities defined in your
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

PersonAppService contructor-injects **IRepository&lt;Person&gt;** and
uses the **Insert** method. In this way, you can easily inject
**IRepository&lt;TEntity&gt;** (or IRepository&lt;TEntity,
TPrimaryKey&gt;) and use predefined methods.

#### Custom Repositories

If standard repository methods are not sufficient, you can create custom
repository classes for your entities.

##### Application Specific Base Repository Class

ASP.NET Boilerplate provides a base class **EfCoreRepositoryBase** to
implement repositories easily. To implement **IRepository** interface,
you can just derive your repository from this class. But it's better to
create your own base class that extens EfRepositoryBase. Thus, you can
add shared/common methods to your repositories easily. An example base
class all for repositories of a *SimpleTaskSystem* application:

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

    //A shortcut for entities those have integer Id
    public class SimpleTaskSystemRepositoryBase<TEntity> : SimpleTaskSystemRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        public SimpleTaskSystemRepositoryBase(IDbContextProvider<SimpleTaskSystemDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        //do not add any method here, add to the class above (because this class inherits it)
    }

Notice that we're inheriting from
EfCoreRepositoryBase&lt;**SimpleTaskSystemDbContext**, TEntity,
TPrimaryKey&gt;. This declares to ASP.NET Boilerplate to use
SimpleTaskSystemDbContext in our repositories.

By default, all repositories for your given DbContext
(SimpleTaskSystemDbContext in this example) is implemented using
EfCoreRepositoryBase. You can replace it to your own repository base
repository class by adding **AutoRepositoryTypes** attribute to your
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

To implement a custom repository, just derive from your application
specific base repository class we created above.

Assume that we have a Task entity that can be assigned to a Person
(entity) and a Task has a State (new, assigned, completed... and so on).
We may need to write a custom method to get list of Tasks with some
conditions and with AssisgnedPerson property pre-fetched (included) in a
single database query. See the example code:

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
**GetAll()** returns **IQueryable&lt;Task&gt;**, then we can add some
**Where** filters using given parameters. Finally we can call
**ToList()** to get list of Tasks.

You can also use **Context** object in repository methods to reach to
your DbContext and directly use Entity Framework APIs. 

**Note**: Define the custom repository **interface** in the
**domain/core** layer, **implement** it in the **EntityFrameworkCore**
project for layered applications. Thus, you can inject the interface
from any project without referencing to EF Core.

##### Replacing Default Repositories

Even you have created a TaskRepository as shown above, any class can
still [inject](Dependency-Injection.html) IRepository&lt;Task, long&gt;
and use it. That's not a problem in most cases. But, what if you
**overrided** a base method in your custom repository? Say that you have
overrided Delete method in your custom repository to add a custom
behaviour on delete. If a class injects IRepository&lt;Task, long&gt;
and use the default repository to Delete a task, your custom behaviour
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

We registered TaskRepository for IRepository&lt;Task, Guid&gt;,
ITaskRepository and TaskRepository. So, any one of these can be injected
to use the TaskRepository.

#### Repository Best Practices

-   **Use default repositories** wherever it's possible. You can use
    default repository even you have a custom repository for an entity
    (if you will use standard repository methods).
-   Always create **repository base class** for your application for
    custom repositories, as defined above.
-   Define **interfaces** for your custom repositories in **domain
    layer** (.Core project in startup template), custom repository
    **classes** in **.EntityFrameworkCore** project if you want to
    abstract EF Core from your domain/application.
