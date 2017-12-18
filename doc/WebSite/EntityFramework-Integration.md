ASP.NET Boilerplate can work with any O/RM framework. It has built-in
integration with **EntityFramework**. This document will explain how to
use EntityFramework with ASP.NET Boilerplate. It's assumed that you're
already familar with EntityFramework in a basic level.

### Nuget Package

Nuget package to use EntityFramework as O/RM in ASP.NET Boilerplate is
[Abp.EntityFramework](http://www.nuget.org/packages/Abp.EntityFramework).
You should add it to your application. It's better to implement
EntityFramework in a seperated assembly (dll) in your application and
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

It's a regular DbContext class except following rules:

-   It's derived from **AbpDbContext** instead of DbContext.
-   It should have constructors as the sample above (constructor
    parameter names should also be same). Explanation:
    -   **Default** consturctor passes "Default" to base bass as the
        connection string. So, it expects a "Default" named connection
        string in the web.config/app.config file. This constructor is
        not used by ABP, but used by EF command line migration tool
        commands (like update-database).
    -   The constructor gets **nameOrConnectionString** is used by ABP
        to pass the connection name or string on runtime.
    -   The constructor get **existingConnection** can be used for unit
        test and not directly used by ABP.
    -   The constructor gets **existingConnection** and
        **contextOwnsConnection** is used by ABP on single database
        multiple dbcontext scenarios to share same connection &
        transaction () when **DbContextEfTransactionStrategy** is used
        (see Transaction Management section below).

EntityFramework can map classes to database tables in a conventional
way. You even don't need to make any configuration unless you make some
custom stuff. In this example, we mapped entities to different tables.
As default, Task entity maps to **Tasks** table. But we changed it to be
**StsTasks** table. Instead of configuring it with data annotation
attributes, I prefered to use fluent configuration. You can choice the
way you like.

### Repositories

Repositories are used to abstract data access from higher layers. See
[repository documentation](Repositories.md) for more. 

#### Default Repositories

[Abp.EntityFramework](http://www.nuget.org/packages/Abp.EntityFramework)
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

ASP.NET Boilerplate provides a base class **EfRepositoryBase** to
implement repositories easily. To implement **IRepository** interface,
you can just derive your repository from this class. But it's better to
create your own base class that extens EfRepositoryBase. Thus, you can
add shared/common methods to your repositories or override existing
methods easily. An example base class all for repositories of a
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

        //do not add any method here, add to the class above (because this class inherits it)
    }

Notice that we're inheriting from
EfRepositoryBase&lt;**SimpleTaskSystemDbContext**, TEntity,
TPrimaryKey&gt;. This declares to ASP.NET Boilerplate to use
SimpleTaskSystemDbContext in our repositories.

By default, all repositories for your given DbContext
(SimpleTaskSystemDbContext in this example) is implemented using
EfRepositoryBase. You can replace it to your own repository base
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
**domain/core** layer, **implement** it in the **EntityFramework**
project for layered applications. Thus, you can inject the interface
from any project without referencing to EF.

#### Repository Best Practices

-   **Use default repositories** wherever it's possible. You can use
    default repository even you have a custom repository for an entity
    (if you will use standard repository methods).
-   Always create **repository base class** for your application for
    custom repositories, as defined above.
-   Define **interfaces** for your custom repositories in **domain
    layer** (.Core project in startup template), custom repository
    **classes** in **.EntityFramework** project if you want to abstract
    EF from your domain/application.

### Transaction Management

ASP.NET Boilerplate has a built-in [unit of work](Unit-Of-Work.md)
system to manage database connection and transactions. Entity framework
has different [transaction management
approaches](https://msdn.microsoft.com/en-us/library/dn456843(v=vs.113).aspx).
ASP.NET Boilerplate uses ambient TransactionScope approach by default,
but also has a built-in implementation for DbContext transaction API. If
you want to switch to DbContext transaction API, you can configure it in
PreInitialize method of your [module](Module-System.md) like that:

    Configuration.ReplaceService<IEfTransactionStrategy, DbContextEfTransactionStrategy>(DependencyLifeStyle.Transient);

Remember to add "*using Abp.Configuration.Startup;*" to your code file
to be able to use ReplaceService generic extension method.

In addition, your DbContext **should have constructors** as described in
DbContext section of this document.

### Data Stores

Since ASP.NET Boilerplate has built-in integration with EntityFramework,
it can work with data stores EntityFramework supports. Our free startup
templates are designed to work with Sql Server but you can modify them
to work with a different data store.

For example, if you want to work with MySql, please refer to [this
document](EF-MySql-Integration.md)
