ASP.NET Boilerplate can work with any O/RM framework. It has built-in
integration with **EntityFramework**. This document will explain how to
use EntityFramework with ASP.NET Boilerplate. It's assumed that you're
already familar with EntityFramework at a basic level.

### NuGet Package

The NuGet package to use EntityFramework as an O/RM in ASP.NET Boilerplate is
[Abp.EntityFramework](http://www.nuget.org/packages/Abp.EntityFramework).
You should add it to your application. It's better to implement
EntityFramework in a separated assembly (dll) in your application and
depend on that package from this assembly.

### DbContext

As you know, to work with EntityFramework, you should define a
**DbContext** class for your application. An example DbContext is shown
below:

    public class SimpleTaskSystemDbContext : AbpDbContext
    {
        public virtual IDbSet<Person> People { get; set; }
        public virtual IDbSet<Task> Tasks { get; set; }

        public SimpleTaskSystemDbContext()
            : base("Default")
        {

        }
        
        public SimpleTaskSystemDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public SimpleTaskSystemDbContext(DbConnection existingConnection)
            : base(existingConnection, false)
        {

        }

        public SimpleTaskSystemDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>().ToTable("StsPeople");
            modelBuilder.Entity<Task>().ToTable("StsTasks").HasOptional(t => t.AssignedPerson);
        }
    }

It's a regular DbContext class except with the following rules:

-   It's derived from **AbpDbContext** instead of DbContext.
-   It should have the constructors like the sample above (constructor
    parameter names should also be the same). Explanation:
    -   The **Default** constructor passes "Default" to the base class as the
        connection string. It expects a "Default" named connection
        string in the web.config/app.config file. This constructor is
        not used by ABP, but used by the EF command-line migration tool
        commands (like "update-database").
    -   The constructor gets the **nameOrConnectionString** which is used by ABP
        to pass the connection name or string on runtime.
    -   The constructor get the **existingConnection** which can be used for unit
        tests, and is not directly used by ABP.
    -   The constructor gets the **existingConnection** and the
        **contextOwnsConnection** is used by ABP on single database/
        multiple dbcontext scenarios to share the same connection &
        transaction () when **DbContextEfTransactionStrategy** is used
        (see Transaction Management section below).

EntityFramework can map classes to database tables in a conventional
way. You don't even need to make a configuration unless you make some
custom stuff. In this example, we mapped entities to different tables.
By default, the Task entity maps to the **Tasks** table. We changed it to be
**StsTasks** table. Instead of configuring it with data annotation
attributes, it is recommended that you use fluent configuration. You can choose 
what you like.

### Repositories

Repositories are used to abstract data access from higher layers. See the
[repository documentation](Repositories.md) for more info. 

#### Default Repositories

The [Abp.EntityFramework](http://www.nuget.org/packages/Abp.EntityFramework)
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
uses the **Insert** method. This way, you can easily inject
**IRepository&lt;TEntity&gt;** (or IRepository&lt;TEntity,
TPrimaryKey&gt;) and use the predefined methods.

#### Custom Repositories

If standard repository methods are not sufficient, you can create custom
repository classes for your entities.

##### Application Specific Base Repository Class

ASP.NET Boilerplate provides a base class, **EfRepositoryBase**, to
implement repositories easily. To implement the **IRepository** interface,
you can simply derive your repository from this class. However, it's better to
create your own base class that extends the EfRepositoryBase. Thus, you can
easily add shared/common methods to your repositories or override existing
methods. An example base class for all the repositories of a
*SimpleTaskSystem* application:

    //Base class for all repositories in my application
    public class SimpleTaskSystemRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<SimpleTaskSystemDbContext, TEntity, TPrimaryKey>
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

        //do not add methods here, add them to the class above (because this class inherits it)
    }

Notice that we're inheriting from
EfRepositoryBase&lt;**SimpleTaskSystemDbContext**, TEntity, and
TPrimaryKey&gt;? This sets ASP.NET Boilerplate to use the
SimpleTaskSystemDbContext in our repositories.

By default, all the repositories for your given DbContext
(SimpleTaskSystemDbContext in this example) is implemented using
EfRepositoryBase. You can replace it to your own base
repository class by adding the **AutoRepositoryTypes** attribute to your
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

To implement a custom repository, simply derive it from your application
specific base repository class like the one we created above.

Assume that we have a Task entity that can be assigned to a Person
(entity) and a Task State (new, assigned, completed... and so on).
We may need to write a custom method to get the list of Tasks, with some
conditions, and with a pre-fetched (included) AssisgnedPerson property; All in a
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
The **GetAll()** method returns an **IQueryable&lt;Task&gt;**, then we can added
some **Where** filters using the given parameters. Finally, we called
**ToList()** to get the list of Tasks.

You can also use the **Context** object in repository methods to reach 
your DbContext, so that you can directly use the Entity Framework APIs. 

**Note**: Define the custom repository **interface** in the
**domain/core** layer, **implement** it in the **EntityFramework**
project for layered applications. This way, you can inject the interface
from any project without actually referencing EF.

#### Repository Best Practices

-   **Use default repositories** wherever it's possible. You can use
    the default repository even if you have a custom repository for an entity
    (if you use standard repository methods).
-   Always create a **repository base class** for your application for
    custom repositories, as defined above.
-   Define **interfaces** for your custom repositories in the **domain
    layer** (.Core project in startup template), and custom repository
    **classes** in the **.EntityFramework** project, if you want to abstract
    EF from your domain/application.

### Transaction Management

ASP.NET Boilerplate has a built-in [unit of work](Unit-Of-Work.md)
system to manage database connection and transactions. Entity framework
has different [transaction management
approaches](https://msdn.microsoft.com/en-us/library/dn456843(v=vs.113).aspx).
ASP.NET Boilerplate uses the ambient TransactionScope approach by default,
but it also has a built-in implementation for the DbContext transaction API. If
you want to switch to the DbContext transaction API, you can configure it in the
PreInitialize method of your [module](Module-System.md) like this:

    Configuration.ReplaceService<IEfTransactionStrategy, DbContextEfTransactionStrategy>(DependencyLifeStyle.Transient);

Remember to add "*using Abp.Configuration.Startup;*" to your code file
to be able to use the ReplaceService generic extension method.

In addition, your DbContext **should have constructors** as described in the
DbContext section of this document.

### Data Stores

Since ASP.NET Boilerplate has built-in integration with EntityFramework,
it can work with the data stores EntityFramework supports. Our free startup
templates are designed to work with Sql Server but you can modify them
to work with a different data store.

For example, if you want to work with MySql, please refer to [this
document](EF-MySql-Integration.md)
