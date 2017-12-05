### Introduction

Connection and transaction management is one of the most important
concepts in an application that uses a database. When to open a
connection, when to start a transaction, how to dispose the connection
and so on.. ASP.NET Boilerplate manages database connection and
transactions by it's **unit of work** system.

### Connection & Transaction Management in ASP.NET Boilerplate

ASP.NET Boilerplate **opens** a database connection (it may not be
opened immediately, but opened in first database usage, based on ORM
provider implementation) and begins a **transaction** when **entering**
a **unit of work method**. So, you can use connection safely in this
method. At the end of the method, the transaction is **commited** and
the connection is **disposed**. If the method throws any **exception**,
transaction is **rolled back** and the connection is disposed. In this
way, a unit of work method is **atomic** (a **unit of work**). ASP.NET
Boilerplate does all of these automatically.

If a unit of work method calls another unit of work method, both uses
same connection & transaction. The first entering method manages
connection & transaction, others use it.

#### Conventional Unit Of Work Methods

Some methods are unit of work methods by default:

-   All [MVC](MVC-Controllers.html), [Web API](Web-API-Controllers.html)
    and [ASP.NET Core MVC](AspNet-Core.html) Controller actions.
-   All [Application Service](Application-Services.html) methods.
-   All [Repository](Repositories.html) methods.

 Assume that we have an [application
service](/Pages/Documents/Application-Services) method like below:

    public class PersonAppService : IPersonAppService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IStatisticsRepository _statisticsRepository;

        public PersonAppService(IPersonRepository personRepository, IStatisticsRepository statisticsRepository)
        {
            _personRepository = personRepository;
            _statisticsRepository = statisticsRepository;
        }

        public void CreatePerson(CreatePersonInput input)
        {
            var person = new Person { Name = input.Name, EmailAddress = input.EmailAddress };
            _personRepository.Insert(person);
            _statisticsRepository.IncrementPeopleCount();
        }
    }

In the CreatePerson method, we're inserting a person using person
repository and incrementing total people count using statistics
repository. Both of repositories **shares** same connection and
transaction in this example since application service method is unit of
work by default. ASP.NET Boilerplate opens a database connection and
starts a transaction when entering CreatePerson method and commit the
transaction at end of the method if no exception is thrown, rolls back
if any exception occurs. In that way, all database operations in
CreatePerson method becomes **atomic** (**unit of work**). 

In addition to default conventional unit of work classes, you can add
your own convention in PreInitialize method of your
[module](Module-System.html) like below:

    Configuration.UnitOfWork.ConventionalUowSelectors.Add(type => ...);

You should check type and return true if this type should be a
conventional unit of work class.

#### Controlling Unit Of Work

Unit of work **implicitly** works for the methods defined above. In most
cases you don't have to control unit of work manually for web
applications. You can **explicitly** use it if you want to control unit
of work somewhere else. There are two approaches for it. 

##### UnitOfWork Attribute

First and preferred approach is using **UnitOfWork** attribute. Example:

    [UnitOfWork]
    public void CreatePerson(CreatePersonInput input)
    {
        var person = new Person { Name = input.Name, EmailAddress = input.EmailAddress };
        _personRepository.Insert(person);
        _statisticsRepository.IncrementPeopleCount();
    }

Thus, CreatePerson methods becomes unit of work and manages database
connection and transaction, both repositories use same unit of work.
Note that no need to UnitOfWork attribute if this is an application
service method. Also see '[unit of work method
restrictions](#DocUowRestrictions)' section.

There are some options of the UnitOfWork attribute. See 'unit of work in
detail' section. UnitOfWork attribute can also be used for classes to
configure all methods of a class. Method attribute overrides the class
attribute if exists.

##### IUnitOfWorkManager

Second appropaches is using the **IUnitOfWorkManager.Begin(...)** method
as shown below:

    public class MyService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IPersonRepository _personRepository;
        private readonly IStatisticsRepository _statisticsRepository;

        public MyService(IUnitOfWorkManager unitOfWorkManager, IPersonRepository personRepository, IStatisticsRepository statisticsRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _personRepository = personRepository;
            _statisticsRepository = statisticsRepository;
        }

        public void CreatePerson(CreatePersonInput input)
        {
            var person = new Person { Name = input.Name, EmailAddress = input.EmailAddress };

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _personRepository.Insert(person);
                _statisticsRepository.IncrementPeopleCount();

                unitOfWork.Complete();
            }
        }
    }

You can inject and use IUnitOfWorkManager as shown here (Some base
classes have already **UnitOfWorkManager** injected by default: MVC
Controllers, [application services](Application-Services.html), [domain
services](Domain-Services.html)...). Thus, you can create more **limited
scope** unit of works. In this approach, you should call **Complete**
method manually. If you don't call, transaction is rolled back and
changes are not saved.

Begin method has overloads to set **unit of work options**. It's better
and shorter to use **UnitOfWork** attribute if you don't have a good
reason.

### Unit Of Work in Detail

#### Disabling Unit Of Work

You may want to disable unit of work **for conventional unit of work
methods** . To do that, use UnitOfWorkAttribute's **IsDisabled**
property. Example usage:

    [UnitOfWork(IsDisabled = true)]
    public virtual void RemoveFriendship(RemoveFriendshipInput input)
    {
        _friendshipRepository.Delete(input.Id);
    }
                

Normally, you don't want to do that, but in some situations you may want
to disable unit of work:

-   You may want to use unit of work in a limited scope with
    UnitOfWorkScope class described above.

Note that if a unit of work method calls this RemoveFriendship method,
disabling this method is ignored and it uses the same unit of work with
the caller method. So, use disabling by carefully. Also, the code above
works well since repository methods are unit of work by default.

#### Non-Transactional Unit Of Work

A unit of work is transactional as default (by it's nature). Thus,
ASP.NET Boilerplate starts/commits/rollbacks an explicit database-level
transaction. In some special cases, transaction may cause problems since
it may lock some rows or tables in the database. In this situations, you
may want to disable database-level transaction. UnitOfWork attribute can
get a boolean value in it's constructor to work as non-transactional.
Example usage:

    [UnitOfWork(isTransactional: false)]
    public GetTasksOutput GetTasks(GetTasksInput input)
    {
        var tasks = _taskRepository.GetAllWithPeople(input.AssignedPersonId, input.State);
        return new GetTasksOutput
                {
                    Tasks = Mapper.Map<List<TaskDto>>(tasks)
                };
    }

I suggest to use this attribute as **\[UnitOfWork(isTransactional:
false)\]**. I think it's more readable and explicit. But you can use as
\[UnitOfWork(false)\].

Note that ORM frameworks (like NHibernate and EntityFramework)
internally saves changes in a single command. Assume that you updated a
few Entities in a non-transactional UOW. Even in this situation all
updates are performed at end of the unit of work with a single database
command. But if you execute an SQL query directly, it's performed
immediately and not rolled back if your UOW is not transactional.

There is a restriction for non-transactional UOWs. If you're already in
a transactional unit of work scope, setting isTransactional to false is
ignored (use Transaction Scope Option to create a non-transactional unit
of work in a transactional unit of work).

Use non-transactional unit of works carefully since most of the times it
should be transactional for data integrity. If your method just reads
data, not changes it, it can be safely non-transactional.

#### A Unit Of Work Method Calls Another

Unit of work is ambient. If a unit of work method calls another unit of
work method, they share same connection and transaction. First method
manages connection, others use it.

#### Unit Of Work Scope

You can create a different and isolated transaction in another
transaction or can create a non-transactional scope in a transaction.
.NET defines
[TransactionScopeOption](https://msdn.microsoft.com/en-us/library/system.transactions.transactionscopeoption(v=vs.110).aspx)
for that. You can set Scope option of the unit of work to control it.

#### Automatically Saving Changes

If a method is unit of work, ASP.NET Boilerplate saves all changes at
the end of the method automatically. Assume that we need method to
update name of a person:

    [UnitOfWork]
    public void UpdateName(UpdateNameInput input)
    {
        var person = _personRepository.Get(input.PersonId);
        person.Name = input.NewName;
    }

That's all, name was changed! We did not even called
\_personRepository.Update method. O/RM framework keep track of all
changes of entities in a unit of work and reflects changes to the
database.

Note that no need to declare UnitOfWork for conventional unit of work
methods.

#### IRepository.GetAll() Method

When you call GetAll() out of a repository method, there must be an open
database connection since it returns IQueryable. This is needed because
of deferred execution of IQueryable. It does not perform database query
unless you call ToList() method or use the IQueryable in a foreach loop
(or somehow access to queried items). So, when you call ToList() method,
database connection must be alive.

Consider the example below:

    [UnitOfWork]
    public SearchPeopleOutput SearchPeople(SearchPeopleInput input)
    {
        //Get IQueryable<Person>
        var query = _personRepository.GetAll();

        //Add some filters if selected
        if (!string.IsNullOrEmpty(input.SearchedName))
        {
            query = query.Where(person => person.Name.StartsWith(input.SearchedName));
        }

        if (input.IsActive.HasValue)
        {
            query = query.Where(person => person.IsActive == input.IsActive.Value);
        }

        //Get paged result list
        var people = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new SearchPeopleOutput { People = Mapper.Map<List<PersonDto>>(people) };
    }

Here, SearchPeople method must be unit of work since ToList() method of
IQueryable is called in the method body, and database connection must be
open when IQueryable.ToList() is executed.

In most cases you will use GetAll method safely in a web application,
since all controller actions are unit of work by default and thus
database connection is available in entire request.

#### UnitOfWork Attribute Restrictions

You can use UnitOfWork attribute for;

-   All **public** or **public virtual** methods for classes those are
    used over interface (Like an application service used over service
    interface).
-   All **public virtual** methods for self injected classes (Like **MVC
    Controllers** and **Web API Controllers**).
-   All **protected virtual** methods.

It's suggested to always make the method **virtual**. You can **not use
it for private methods**. Because, ASP.NET Boilerplate uses dynamic
proxying for that and private methods can not be seen by derived
classes. UnitOfWork attribute (and any proxying) does not work if you
don't use [dependency injection](/Pages/Documents/Dependency-Injection)
and instantiate the class yourself.

### Options

There are some options can be used to change behaviour of a unit of
work.

First, we can change default values of all unit of works in the [startup
configuration](/Pages/Documents/Startup-Configuration). This is
generally done in PreInitialize method of our
[module](/Pages/Documents/Module-System).

    public class SimpleTaskSystemCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsolationLevel = IsolationLevel.ReadCommitted;
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
        }

        //...other module methods
    }

Second, we can override defaults for a particular unit of work. For
that, **UnitOfWork** attribute constructor and
IUnitOfWorkManager.**Begin** method have overloads to get options.

And lastly, you can use startup configuration to configure default unit
of work attributes for ASP.NET MVC, Web API and ASP.NET Core MVC
Controllers (see their documentation).

### Methods

UnitOfWork system works seamlessly and invisibly. But, in some special
cases, you need to call it's methods.

You can access to current unit of work in one of two ways:

-   You can directly use **CurrentUnitOfWork** property if your class is
    derived from some specific base classes (ApplicationService,
    DomainService, AbpController, AbpApiController... etc.)
-   You can inject IUnitOfWorkManager into any class and use
    **IUnitOfWorkManager.Current** property.

#### SaveChanges

ASP.NET Boilerplate saves all changes at end of a unit of work, you
don't have to do anything. But, sometimes, you may want to save changes
to database in middle of a unit of work operation. An example usage may
be saving changes to get Id of a new inserted
[Entity](/Pages/Documents/Entities) in
[EntityFramework](/Pages/Documents/EntityFramework-Integration).

You can use **SaveChanges** or **SaveChangesAsync** method of current
unit of work.

 Note that: if current unit of work is transactional, all changes in the
transaction are rolled back if an exception occurs, even saved changes.

### Events

A unit of work has **Completed**, **Failed** and **Disposed** events.
You can register to these events and perform needed operations. For
example,yYou may want to run some code when current unit of work
successfully completed. Example:

    public void CreateTask(CreateTaskInput input)
    {
        var task = new Task { Description = input.Description };

        if (input.AssignedPersonId.HasValue)
        {
            task.AssignedPersonId = input.AssignedPersonId.Value;
            _unitOfWorkManager.Current.Completed += (sender, args) => { /* TODO: Send email to assigned person */ };
        }

        _taskRepository.Insert(task);
    }
