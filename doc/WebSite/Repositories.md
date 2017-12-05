"*Mediates between the domain and data mapping layers using a
collection-like interface for accessing domain objects*" (Martin
Fowler).

Repositories, in practice, are used to perform database operations for
domain objects ([Entity](/Pages/Documents/Entities) and Value types).
Generally, a seperated repository is used for each Entity (or Aggregate
Root).

### Default Repositories

In ASP.NET Boilerplate, a repository classes implement
**IRepository&lt;TEntity, TPrimaryKey&gt;** interface. ABP can
automatically creates default repositories for each entity type. You can
directly [inject](/Pages/Documents/Dependency-Injection)
**IRepository&lt;TEntity&gt;** (or IRepository&lt;TEntity,
TPrimaryKey&gt;). An example [application
service](/Pages/Documents/Application-Services) uses a repository to
insert an entity to database:

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
uses the **Insert** method.

### Custom Repositories

You only create a repository class for an entity when you need to create
a custom repository method(s) for that entity.

#### Custom Repository Interface

A repository definition for a Person entity is shown below:

    public interface IPersonRepository : IRepository<Person>
    {

    }

IPersonRepository extends **IRepository&lt;TEntity&gt;**. It's used to
define entities which has a primary key type of int (Int32). If your
entity's primary key is not int, you can extend
**IRepository&lt;TEntity, TPrimaryKey&gt;** interface as shown below:

    public interface IPersonRepository : IRepository<Person, long>
    {

    }

#### Custom Repository Implementation

ASP.NET Boilerplate is designed to be independent from a particular ORM
(Object/Relational Mapping) framework or another technique to access to
database. Repositories are implemented in **NHibernate** and
**EntityFramework** as out-of-the-box. See documents to implement
repositories in ASP.NET Boilerplate in these frameworks:

-   [NHibernate integration](/Pages/Documents/NHibernate-Integration)
-   [EntityFramework
    integration](/Pages/Documents/EntityFramework-Integration)

### Base Repository Methods

Every repository has some common methods coming from
IRepository&lt;TEntity&gt; interface. We will investigate most of them
here.

#### Querying

##### Getting single entity

    TEntity Get(TPrimaryKey id);
    Task<TEntity> GetAsync(TPrimaryKey id);
    TEntity Single(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
    TEntity FirstOrDefault(TPrimaryKey id);
    Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    TEntity Load(TPrimaryKey id);

**Get** method is used to get an Entity with given primary key (Id). It
throws exception if there is no entity in database with given Id.
**Single** method is similar to Get but takes an expression rather than
an Id. So, you can write a lambda expression to get an Entity. Example
usages:

    var person = _personRepository.Get(42);
    var person = _personRepository.Single(p => p.Name == "John");

Notice that **Single** method throws exception if there is no entity
with given conditions or there are more than one entity.

**FirstOrDefault** is similar but returns **null** (instead of throwing
exception) if there is no entity with given Id or expression. Returns
first found entity if there are more than one entity for given
conditions.

**Load** does not retrieves entity from database but creates a proxy
object for lazy loading. If you only use Id property, Entity is not
actually retrieved. It's retrieved from database only if you access to
other properties of entity. This can be used instead of Get, for
performance reasons. It's implemented in **NHibernate**. If ORM provider
does not implements it, Load method works as identical as Get method.

##### Getting a list of entities

    List<TEntity> GetAllList();
    Task<List<TEntity>> GetAllListAsync();
    List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetAll();

**GetAllList** is used to retrieve all entities from database. Overload
of it can be used to filter entities. Examples:

    var allPeople = _personRepository.GetAllList();
    var somePeople = _personRepository.GetAllList(person => person.IsActive && person.Age > 42);

**GetAll** returns IQueryable&lt;T&gt;. So, you can add Linq methods
after it. Examples:

    //Example 1
    var query = from person in _personRepository.GetAll()
                where person.IsActive
                orderby person.Name
                select person;
    var people = query.ToList();

    //Example 2:
    List<Person> personList2 = _personRepository.GetAll().Where(p => p.Name.Contains("H")).OrderBy(p => p.Name).Skip(40).Take(20).ToList();

With using GetAll, almost all queries can be written in Linq. Even it
can be used in a join expression.

#### About IQueryable&lt;T&gt;

When you call GetAll() out of a repository method, there must be an open
database connection. This is because of deferred execution of
IQueryable&lt;T&gt;. It does not perform database query unless you call
ToList() method or use the IQueryable&lt;T&gt; in a foreach loop (or
somehow access to queried items). So, when you call ToList() method,
database connection must be alive. For a web application, you don't care
about that in most cases since MVC controller methods are unit of work
by default and database connection is available for entire request. See
**[UnitOfWork](/Pages/Documents/Unit-Of-Work)** documentation to
understand it better.

##### Custom return value

There is also an additional method to provide power of IQueryable that
can be usable out of a unit of work.

    T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

Query method accepts a lambda (or method) that recieves
IQueryable&lt;T&gt; and returns any type of object. Example:

    var people = _personRepository.Query(q => q.Where(p => p.Name.Contains("H")).OrderBy(p => p.Name).ToList());

Since given lamda (or method) is executed inside the repository method,
it's executed when database connection is available. You can return a
list of entities, a single entity, a projection or something else that
executes the query.

#### Insert

IRepository interface defines methods to insert an entity to database:

    TEntity Insert(TEntity entity);
    Task<TEntity> InsertAsync(TEntity entity);
    TPrimaryKey InsertAndGetId(TEntity entity);
    Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);
    TEntity InsertOrUpdate(TEntity entity);
    Task<TEntity> InsertOrUpdateAsync(TEntity entity);
    TPrimaryKey InsertOrUpdateAndGetId(TEntity entity);
    Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity);

**Insert** method simply inserts new entity to database and returns the
same inserted entity. **InsertAndGetId** method returns Id of new
inserted entity. This is useful if Id is auto increment and you need Id
of the new inserted entity. **InsertOrUpdate** inserts or updated given
entity by checking it's Id's value. Lastly, **InsertOrUpdateAndGetId**
returns Id of the entity after inserting or updating.

#### Update

IRepository defines methods to update an existing entity in the
database. It gets the entity to be updated and returns the same entity
object.

    TEntity Update(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);

Most of times you don't need to explicitly call Update methods since
unit of work system automatically saves all changes when unit of work
completes. See [unit of work](Unit-Of-Work.md) documentation for more.

#### Delete

IRepository defines methods to delete an existing entity from the
database

    void Delete(TEntity entity);
    Task DeleteAsync(TEntity entity);
    void Delete(TPrimaryKey id);
    Task DeleteAsync(TPrimaryKey id);
    void Delete(Expression<Func<TEntity, bool>> predicate);
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

First method accepts an existing entity, second one accepts Id of the
entity to delete. The last one accepts a condition to delete all
entities fit to given condition. Notice that all entities matches given
predicate may be retrived from database and then deleted (based on
repository implementation). So, use it carefully, it may cause
performance problems if there are too many entities with given
condition.

#### Others

IRepository also provides methods to get count of entities in a table.

    int Count();
    Task<int> CountAsync();
    int Count(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    long LongCount();
    Task<long> LongCountAsync();
    long LongCount(Expression<Func<TEntity, bool>> predicate);
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);

#### About Async Methods

ASP.NET Boilerplate supports async programming model. So, repository
methods has Async versions. Here, a sample [application
service](/Pages/Documents/Application-Services) method that uses async
model:

    public class PersonAppService : AbpWpfDemoAppServiceBase, IPersonAppService
    {
        private readonly IRepository<Person> _personRepository;

        public PersonAppService(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<GetPeopleOutput> GetAllPeople()
        {
            var people = await _personRepository.GetAllListAsync();

            return new GetPeopleOutput
            {
                People = Mapper.Map<List<PersonDto>>(people)
            };
        }
    }

GetAllPeople method is async and uses GetAllListAsync with await
keyword.

Async may not be supported by all ORM frameworks. It's supported by
EntityFramework. If not supported, Async repository methods works
synchronously. Also, for example, InsertAsync works same as Insert in
EntityFramework since EF does not write new entities to database until
unit of work completes (a.k.a. DbContext.SaveChanges).

### Managing Database Connection

A database connection is not opened or closed in a repository method.
Connection management is made automatically by ASP.NET Boilerplate.

A database connection is **opened** and a **transaction** begins while
entering a repository method automatically. When the method ends and
returns, all changes are **saved**, transaction is **commited** and
database connection is **closed** automatically by ASP.NET Boilerplate.
If your repository method throws any type of Exception, the transaction
is automatically **rolled back** and database connection is closed. This
is true for all public methods of classes those implement IRepository
interface.

If a repository method calls to another repository method (even a method
of different repository) they share same connection and transaction.
Connection is managed (opened/closed) by the first method that enters a
repository. For more information on database connection management, see
[UnitOfWork](/Pages/Documents/Unit-Of-Work) documentation.

### Lifetime of a Repository

All repository instances are **Transient**. It means, they are
instantiated per usage. See [Dependency
Injection](/Pages/Documents/Dependency-Injection) documentation for more
information.

### Repository Best Practices

-   For an entity of T, use IRepository&lt;T&gt; wherever it's possible.
    Don't create custom repositories unless it's really needed.
    Pre-defined repository methods will be enough for many cases.
-   If you are creating a custom repository (by extending
    IRepository&lt;TEntity&gt;);
    -   Repository classes should be stateless. That means, you should
        not define repository-level state objects and a repository
        method call should not effect another call.
    -   Custom repository methods should not contain business logic or
        application logic. It should just perform data-related or
        orm-specific tasks.
    -   While repositories can use dependency injection, define less or
        no dependency to other services.
