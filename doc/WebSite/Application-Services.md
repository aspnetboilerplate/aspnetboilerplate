Application Services are used to expose domain logic to the presentation
layer. An Application Service is called from presentation layer with a
DTO (Data Transfer Object) as parameter, uses domain objects to perform
some specific business logic and returns a DTO back to the presentation
layer. Thus, Presentation layer is completely isolated from Domain
layer. In an ideally layered application, presentation layer never
directly works with domain objects.

### IApplicationService Interface

In ASP.NET Boilerplate, an application service **should** implement
**IApplicationService** interface. It's good to create an **interface**
for each Application Service. So, we first define an interface for an
application service as shown below:

    public interface IPersonAppService : IApplicationService
    {
        void CreatePerson(CreatePersonInput input);
    }

**IPersonAppService** has only one method. It's used by presentation
layer to create a new person. **CreatePersonInput** is a DTO object as
shown below:

    public class CreatePersonInput
    {
        [Required]
        public string Name { get; set; }

        public string EmailAddress { get; set; }
    }

Then we can implement the IPersonAppService:

    public class PersonAppService : IPersonAppService
    {
        private readonly IRepository<Person> _personRepository;

        public PersonAppService(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public void CreatePerson(CreatePersonInput input)
        {
            var person = _personRepository.FirstOrDefault(p => p.EmailAddress == input.EmailAddress);
            if (person != null)
            {
                throw new UserFriendlyException("There is already a person with given email address");
            }

            person = new Person { Name = input.Name, EmailAddress = input.EmailAddress };
            _personRepository.Insert(person);
        }
    }

There are some important points here:

-   PersonAppService uses
    [IRepository&lt;Person&gt;](/Pages/Documents/Repositories#DocRepositoryImpl)
    to perform database operations. It uses **constructor injection**
    pattern. We're using [dependency
    injection](/Pages/Documents/Dependency-Injection) here.
-   PersonAppService implements **IApplicationService** (since
    IPersonAppService extends IApplicationService), it's automatically
    registered to Dependency Injection system by ASP.NET Boilerplate and
    can be injected and used by other classes. Naming convention is
    important here. See [dependency
    injection](Dependency-Injection.md) document for more.
-   **CreatePerson** method gets **CreatePersonInput**. It's an **input
    DTO** and automatically validated by ASP.NET Boilerplate. See
    [DTO](/Pages/Documents/Data-Transfer-Objects) and
    [validation](/Pages/Documents/Validating-Data-Transfer-Objects)
    documents for details.

### ApplicationService Class

An application service should implement IApplicationService interface as
declared above. Also, **optionally**, can be derived from
**ApplicationService** base class. Thus, IApplicationService is
inherently implemented. Also, ApplicationService class has some base
functionality that makes easy to **logging,** **localization** and so
on... It's suggested to create a special base class for your application
services that extends ApplicationService class. Thus, you can add some
common functionality for all your application services. A sample
application service class is shown below:

    public class TaskAppService : ApplicationService, ITaskAppService
    {
        public TaskAppService()
        {
            LocalizationSourceName = "SimpleTaskSystem";
        }

        public void CreateTask(CreateTaskInput input)
        {
            //Write some logs (Logger is defined in ApplicationService class)
            Logger.Info("Creating a new task with description: " + input.Description);

            //Get a localized text (L is a shortcut for LocalizationHelper.GetString(...), defined in ApplicationService class)
            var text = L("SampleLocalizableTextKey");

            //TODO: Add new task to database...
        }
    }

You can have a base class which defines **LocalizationSourceName** in
it's constructor. Thus, you do not repeat it for all service classes.
See [logging](/Pages/Documents/Logging) and
[localization](/Pages/Documents/Localization) documents for more
informations on this topics.

### CrudAppService and AsyncCrudAppService Classes

If you need to create an application service that will have **Create,
Update, Delete, Get, GetAll** methods for a **specific entity**, you can
inherit from **CrudAppService** (or **AsyncCrudAppService** if you want
to create async methods) class to create it easier. CrudAppService base
class is **generic** which gets related **Entity** and **DTO** types as
generic arguments and is **extensible** which allows you to override
functionality when you need to customize it.

#### Simple CRUD Application Service Example

Assume that we have a Task [entity](Entities.md) defined below:

    public class Task : Entity, IHasCreationTime
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public TaskState State { get; set; }

        public Person AssignedPerson { get; set; }
        public Guid? AssignedPersonId { get; set; }

        public Task()
        {
            CreationTime = Clock.Now;
            State = TaskState.Open;
        }
    }

And we created a [DTO](Data-Transfer-Objects.md) for this entity:

    [AutoMap(typeof(Task))]
    public class TaskDto : EntityDto, IHasCreationTime
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public TaskState State { get; set; }

        public Guid? AssignedPersonId { get; set; }

        public string AssignedPersonName { get; set; }
    }

AutoMap attribute creates auto mapping configuration between entity and
dto. Now, we can create an application service as shown below:

    public class TaskAppService : AsyncCrudAppService<Task, TaskDto>
    {
        public TaskAppService(IRepository<Task> repository) 
            : base(repository)
        {

        }
    }

We [injected](Dependency-Injection.md) the
[repository](Repositories.md) and passed it to the base class (We
could inherit from CrudAppService if we want to create sync methods
instead of async methods). **That's all!** TaskAppService now have
simple CRUD methods. If you want to define an interface for the
application service, you can create your interface as shown below:

    public interface ITaskAppService : IAsyncCrudAppService<TaskDto>
    {
            
    }

Notice that **IAsyncCrudAppService** does not get the entity (Task) as
generic argument. Because, entity is related to implementation and
should not be included in public interface. Now, we can implement
ITaskAppService interface for the TaskAppService class:

    public class TaskAppService : AsyncCrudAppService<Task, TaskDto>, ITaskAppService
    {
        public TaskAppService(IRepository<Task> repository) 
            : base(repository)
        {

        }
    }

#### Customize CRUD Application Services

##### Getting List

Crud application service gets **PagedAndSortedResultRequestDto** as
argument for **GetAll** method as default, which provides optional
sorting and paging parameters. But you may want to add another
parameters for GetAll method. For example, you may want to add some
**custom filters**. In that case, you can create a DTO for GetAll
method. Example:

    public class GetAllTasksInput : PagedAndSortedResultRequestDto
    {
        public TaskState? State { get; set; }
    }

We inherit from **PagedAndSortedResultRequestInput** (which is **not
required**, but wanted to use paging & sorting parameters form the base
class) and added an **optional State** property to filter tasks by
state. Now, we should change TaskAppService in order to apply the
**custom filter**:

    public class TaskAppService : AsyncCrudAppService<Task, TaskDto, int, GetAllTasksInput>
    {
        public TaskAppService(IRepository<Task> repository)
            : base(repository)
        {

        }

        protected override IQueryable<Task> CreateFilteredQuery(GetAllTasksInput input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.State.HasValue, t => t.State == input.State.Value);
        }
    }

First, we added **GetAllTasksInput** as 4th generic parameter to
AsyncCrudAppService class (3rd one is PK type of the entity). Then
overrided **CreateFilteredQuery** method to apply custom filters. This
method is an extension point for AsyncCrudAppService class (WhereIf is
an extension method of ABP to simplify conditional filtering. But
actually what we do is to simply filter an IQueryable).

Note that: If you have created application service interface, you need
to add same generic arguments to that interface too.

##### Create and Update

Notice that we are using same DTO (TaskDto) for getting, **creating**
and **updating** tasks which may not be good for real life applications.
So, we may want to **customize create and update DTOs**. Let's start by
creating a **CreateTaskInput** class:

    [AutoMapTo(typeof(Task))]
    public class CreateTaskInput
    {
        [Required]
        [MaxLength(Task.MaxTitleLength)]
        public string Title { get; set; }

        [MaxLength(Task.MaxDescriptionLength)]
        public string Description { get; set; }

        public Guid? AssignedPersonId { get; set; }
    }

And create an **UpdateTaskInput** DTO:

    [AutoMapTo(typeof(Task))]
    public class UpdateTaskInput : CreateTaskInput, IEntityDto
    {
        public int Id { get; set; }

        public TaskState State { get; set; }
    }

I wanted to inherit from **CreateTaskInput** to include all properties
for Update operation (but you may want different). Implementing
**IEntity** (or IEntity&lt;PrimaryKey&gt; for different PK than int) is
**required** here, because we need to know which entity is being
updated. Lastly, I added an additional property, **State**, which is not
in CreateTaskInput.

Now, we can use these DTO classes as generic arguments for
AsyncCrudAppService class, as shown below:

    public class TaskAppService : AsyncCrudAppService<Task, TaskDto, int, GetAllTasksInput, CreateTaskInput, UpdateTaskInput>
    {
        public TaskAppService(IRepository<Task> repository)
            : base(repository)
        {

        }

        protected override IQueryable<Task> CreateFilteredQuery(GetAllTasksInput input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.State.HasValue, t => t.State == input.State.Value);
        }
    }

No need to an additional code change.

##### Other Method Arguments

AsyncCrudAppService can get more generic arguments if you want to
customize input DTOs for **Get** and **Delete** methods. Also, all
methods of the base class is virtual, so you can override them to
customize the behaviour.

#### CRUD Permissions

You probably need to [authorize](Authorization.md) your CRUD methods.
There are pre-defined permission properties you can set:
GetPermissionName, GetAllPermissionName, CreatePermissionName,
UpdatePermissionName and DeletePermissionName. Base CRUD class
automatically checks permissions if you set them. You can set it in the
constructor as shown below:

    public class TaskAppService : AsyncCrudAppService<Task, TaskDto>
    {
        public TaskAppService(IRepository<Task> repository) 
            : base(repository)
        {
            CreatePermissionName = "MyTaskCreationPermission";
        }
    }

Alternatively, you can override appropriate permission checker methods
to manually check permissions: CheckGetPermission(),
CheckGetAllPermission(), CheckCreatePermission(),
CheckUpdatePermission(), CheckDeletePermission(). As default, they all
calls CheckPermission(...) method with related permission name which
simply calls IPermissionChecker.Authorize(...) method.

### Unit of Work

An application service method is a **[unit of
work](/Pages/Documents/Unit-Of-Work)** by default in ASP.NET
Boilerplate. Thus, any application service method is transactional and
automatically saves all database changes at the end of the method. 

See [unit of work](/Pages/Documents/Unit-Of-Work) documentation for
more.

### Lifetime of an Application Service

All application service instances are **Transient**. It means, they are
instantiated per usage. See [Dependency
Injection](/Pages/Documents/Dependency-Injection) documentation for more
information.
