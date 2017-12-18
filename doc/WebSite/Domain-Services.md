### Introduction

Domain Services (or just Service, in DDD) is used to perform domain
operations and business rules. Eric Evans describes a good Service in
three characteristics (in his DDD book):

1.  The **operation** relates to a **domain concept** that is not a
    natural part of an Entity or Value Object.
2.  The **interface** is defined in terms of other elements of **domain
    model**.
3.  The operation is **stateless**.

Unlike [Application Services](/Pages/Documents/Application-Services)
which gets/returns [Data Transfer
Objects](/Pages/Documents/Data-Transfer-Objects), a Domain Service
gets/returns **domain objects** (like
[entities](/Pages/Documents/Entities) or value types).

A Domain Service can be used by Application Services and other Domain
Services, but not directly used by presentation layer (application
service is for that).

### IDomainService Interface and DomainService Class

ASP.NET Boilerplate defines **IDomainService interface** that is
implemented by all domain services conventionally. When it's
implemented, the domain service is **automatically registered** to
[Dependency Injection](/Pages/Documents/Dependency-Injection) system as
**transient**.

Also, a domain service (optionally) can inherit from **DomainService
class**. Thus, it can use power of some inherited properties for
logging, localization and so on... Surely, even if it does not inherit,
it can inject they if needs.

### Example

Assume that we have a task management system and we have business rules
while assigning a task to a person.

#### Creating an Interface

First, we define an interface for the service (not required, but as a
good practice):

    public interface ITaskManager : IDomainService
    {
        void AssignTaskToPerson(Task task, Person person);
    }

As you can see, **TaskManager** service works with domain objects: a
**Task** and a **Person**. There are some conventions of naming domain
services. It can be TaskManager, TaskService or TaskDomainService...

#### Service Implementation

Let's see the implementation:

    public class TaskManager : DomainService, ITaskManager
    {
        public const int MaxActiveTaskCountForAPerson = 3;

        private readonly ITaskRepository _taskRepository;

        public TaskManager(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public void AssignTaskToPerson(Task task, Person person)
        {
            if (task.AssignedPersonId == person.Id)
            {
                return;
            }

            if (task.State != TaskState.Active)
            {
                throw new ApplicationException("Can not assign a task to a person when task is not active!");
            }

            if (HasPersonMaximumAssignedTask(person))
            {
                throw new UserFriendlyException(L("MaxPersonTaskLimitMessage", person.Name));
            }

            task.AssignedPersonId = person.Id;
        }

        private bool HasPersonMaximumAssignedTask(Person person)
        {
            var assignedTaskCount = _taskRepository.Count(t => t.State == TaskState.Active && t.AssignedPersonId == person.Id);
            return assignedTaskCount >= MaxActiveTaskCountForAPerson;
        }
    }

We have two business rules here:

-   A task should be in **Active state** in order to assign it to a new
    Person.
-   A person can have a **maximum of 3** active tasks.

You can wonder why I throwed an **ApplicationException** for first check
and **UserFriendlyException** for second check (see [exception
handling](/Pages/Documents/Handling-Exceptions)). This is not related to
domain services at all. I did this for just an example, it completely up
to you. I thought that user interface must check a task's state and
should not allow us to assign it to a person. I think this is an
application error and we may hide it from user. Second one is harder to
check by UI and we can show a readable error message to the user. Just
for an example.

#### Using from Application Service

Now, let's see how to use TaskManager from an application service:

    public class TaskAppService : ApplicationService, ITaskAppService
    {
        private readonly IRepository<Task, long> _taskRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly ITaskManager _taskManager;

        public TaskAppService(IRepository<Task, long> taskRepository, IRepository<Person> personRepository, ITaskManager taskManager)
        {
            _taskRepository = taskRepository;
            _personRepository = personRepository;
            _taskManager = taskManager;
        }

        public void AssignTaskToPerson(AssignTaskToPersonInput input)
        {
            var task = _taskRepository.Get(input.TaskId);
            var person = _personRepository.Get(input.PersonId);

            _taskManager.AssignTaskToPerson(task, person);
        }
    }

Task **Application Service** uses given **DTO** (input) and
**repositories** to retrieve related **task** and **person** and passes
them to the **Task Manager** (the domain service).

### Some Discussions

Based on the example above, you may have some questions.

#### Why Not Only Application Services?

You can say that why application service does not implement the logic in
the domain service?

We can simply say that it's not application service task. Because it's
not a **use-case**, instead, it's a **business operation**. We may use
same 'assign a task to a user' domain logic in a different use-case. Say
that we may have **another screen** to somehow update the task and this
updating can include assigning the task to another person. So, we can
use same domain logic there. Also, we may have **2 different UI** (one
mobile application and one web application) that shares same domain or
we may have a web API for remote clients that includes a task assign
operation.

If your domain is simple, will have only one UI and assigning a task to
a person will be done in just a single point, then you may consider to
skip domain services and implement the logic in your application
service. This will not be a best practice for DDD, but ASP.NET
Boilerplate does not force you for such a design.

#### How to Force to Use the Domain Service?

You can see that, application service simply could do that:

    public void AssignTaskToPerson(AssignTaskToPersonInput input)
    {
        var task = _taskRepository.Get(input.TaskId);
        task.AssignedPersonId = input.PersonId;
    }

The developer write the application service may not know there is a
**TaskManager** and can directly set given **PersonId** to task's
**AssignedPersonId**. So, how to **prevent** it? There are many
discussions in DDD area based on these and there are some used patterns.
We will not go very deep. But, we will provide a simple way of doing it.

We can change **Task** entity as shown below:

    public class Task : Entity<long>
    {
        public virtual int? AssignedPersonId { get; protected set; }

        //...other members and codes of Task entity

        public void AssignToPerson(Person person, ITaskPolicy taskPolicy)
        {
            taskPolicy.CheckIfCanAssignTaskToPerson(this, person);
            AssignedPersonId = person.Id;
        }
    }

We changed setter of **AssignedPersonId** as **protected**. So, it can
not be changed out of this Task entity class. Added an
**AssignToPerson** method that takes a person and a task policy.
**CheckIfCanAssignTaskToPerson** method checks if it's a valid
assignment and throws a proper exception if not (it's implementation is
not important here). Then application service method will be like that:

    public void AssignTaskToPerson(AssignTaskToPersonInput input)
    {
        var task = _taskRepository.Get(input.TaskId);
        var person = _personRepository.Get(input.PersonId);

        task.AssignToPerson(person, _taskPolicy);
    }

We injected ITaskPolicy as \_taskPolicy and passed to AssignToPerson
method. Now, there is no second way of assigning a task to a person. We
should always use AssignToPerson and can not skip business rules.
