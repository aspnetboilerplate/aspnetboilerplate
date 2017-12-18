ASP.NET Boilerplate can work with any O/RM framework. It has built-in
integration with **NHibernate**. This document will explain how to use
NHibernate with ASP.NET Boilerplate. It's assumed that you're already
familar with NHibernate in a basic level.

### Nuget package

Nuget package to use NHibernate as O/RM in ASP.NET Boilerplate is
[Abp.NHibernate](http://www.nuget.org/packages/Abp.NHibernate). You
should add it to your application. It's better to implement NHibernate
in a seperated assembly (dll) in your application and depend on that
package from this assembly.

### Configuration

To start using NHibernate, you should configure it in
[PreInitialize](/Pages/Documents/Module-System) of your module.

    [DependsOn(typeof(AbpNHibernateModule))]
    public class SimpleTaskSystemDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            var connStr = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

            Configuration.Modules.AbpNHibernate().FluentConfiguration
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

**AbpNHibernateModule** module provides base functionality and adapters
to make NHibernate work with ASP.NET Boilerplate.

#### Entity mapping

In this sample configuration above, we have fluently mapped using all
mapping classes in current assembly. An example mapping class can be as
shown below:

    public class TaskMap : EntityMap<Task>
    {
        public TaskMap()
            : base("TeTasks")
        {
            References(x => x.AssignedUser).Column("AssignedUserId").LazyLoad();

            Map(x => x.Title).Not.Nullable();
            Map(x => x.Description).Nullable();
            Map(x => x.Priority).CustomType<TaskPriority>().Not.Nullable();
            Map(x => x.Privacy).CustomType<TaskPrivacy>().Not.Nullable();
            Map(x => x.State).CustomType<TaskState>().Not.Nullable();
        }
    }

**EntityMap** is a class of ASP.NET Boilerplate that extends
**ClassMap&lt;T&gt;**, automatically maps **Id** property and gets
**table name** in the constructor. So, I'm deriving from it and mapping
other properties using
[FluentNHibernate](http://www.fluentnhibernate.org/).  Surely, you can
derive directly from ClassMap, you can use full API of
**FluentNHibernate** and you can use other mapping techniques of
NHibernate (like mapping XML files).

### Repositories

Repositories are used to abstract data access from higher layers. See
[repository documentation](Repositories.md) for more.  

#### Default Implementation

[Abp.NHibernate](http://www.nuget.org/packages/Abp.NHibernate) package
implements default repositories for entities in your application. You
don't have to create repository classes for entities to just use
predefined repository methods. Example:

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
TPrimaryKey&gt;) and use predefined methods. See [repository
documentation](/Pages/Documents/Repositories) for list of all predefined
methods.

#### Custom Repositories

If you want to add some custom method, you should first add it to a
repository interface (as a best practice), then implement in a
repository class. ASP.NET Boilerplate provides a base class
**NhRepositoryBase** to implement repositories easily. To implement
IRepository interface, you can just derive your repository from this
class.

Assume that we have a Task entity that can be assigned to a Person
(entity) and a Task has a State (new, assigned, completed... and so on).
We may need to write a custom method to get list of Tasks with some
conditions and with AssisgnedPerson property pre-fetched in a single
database query. See the example code:

    public interface ITaskRepository : IRepository<Task, long>
    {
        List<Task> GetAllWithPeople(int? assignedPersonId, TaskState? state);
    }

    public class TaskRepository : NhRepositoryBase<Task, long>, ITaskRepository
    {
        public TaskRepository(ISessionProvider sessionProvider)
            : base(sessionProvider)
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
                .Fetch(task => task.AssignedPerson)
                .ToList();
        }
    }

**GetAll()** returns **IQueryable&lt;Task&gt;**, then we can add some
**Where** filters using given parameters. Finally we can call
**ToList()** to get list of Tasks.

You can also use **Session** object in repository methods to use full
API of NHibernate. 

**Note**: Define the custom repository **interface** in the
**domain/core** layer, **implement** it in the **NHibernate** project
for layered applications. Thus, you can inject the interface from any
project without referencing to NH.

##### Application Specific Base Repository Class

Although you can derive your repositories from NhRepositoryBase of
ASP.NET Boilerplate, it's a better practice to create your own base
class that **extends** NhRepositoryBase. Thus, you can add shared/common
methods to your repositories easily. Example:

    //Base class for all repositories in my application
    public abstract class MyRepositoryBase<TEntity, TPrimaryKey> : NhRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected MyRepositoryBase(ISessionProvider sessionProvider)
            : base(sessionProvider)
        {
        }

        //add common methods for all repositories
    }

    //A shortcut for entities those have integer Id.
    public abstract class MyRepositoryBase<TEntity> : MyRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected MyRepositoryBase(ISessionProvider sessionProvider)
            : base(sessionProvider)
        {
        }

        //do not add any method here, add the class above (since this inherits it)
    }

    public class TaskRepository : MyRepositoryBase<Task>, ITaskRepository
    {
        public TaskRepository(ISessionProvider sessionProvider)
            : base(sessionProvider)
        {
        }

        //Specific methods for task repository
    }
