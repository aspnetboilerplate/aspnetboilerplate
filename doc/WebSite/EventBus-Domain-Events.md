In C\#, a class can define events and other classes can register with them
to be notified when something happens. This is useful for a desktop
application or standalone windows service, but for a web application
it's a bit problematic since objects are created in a web request and
are short-lived. It's hard to register some class events.
Directly registering to another class's event makes classes tightly
coupled.

Domain events can be used to decouple business logic and to react to
important domain changes in an application.

### EventBus

The EventBus is a **singleton** object that is shared by other classes
to trigger and handle events. To use the event bus, you need to get a
reference to it. You can do that in two ways.

#### Injecting IEventBus

You can use [dependency
injection](/Pages/Documents/Dependency-Injection) to get a reference to the
**IEventBus**. Here, we used the property injection pattern:

    public class TaskAppService : ApplicationService
    {
        public IEventBus EventBus { get; set; }
    
        public TaskAppService()
        {
            EventBus = NullEventBus.Instance;
        }
    }

Property injection is more proper than a constructor injection when injecting
the event bus. This way, your class can work without the event bus. NullEventBus
implements the [null object
pattern](http://en.wikipedia.org/wiki/Null_Object_pattern). When you
call its methods, it does nothing at all.

#### Getting The Default Instance

If you can not inject it, you can directly use **EventBus.Default**.
It's the global event bus and it can be used as shown:

    EventBus.Default.Trigger(...); //trigger an event

We **do not recommend** that you directly use EventBus.Default,
since it makes unit testing harder.

### Defining Events

Before you can trigger an event, you need to first define it. An event is
represented by a class that is derived from **EventData**. Assume that
we want to trigger an event when a task is completed:

    public class TaskCompletedEventData : EventData
    {
        public int TaskId { get; set; }
    }

This class contains properties that are needed by the class that handles
the event. The **EventData** class defines the **EventSource** (the object that
triggered the event) and the **EventTime** (when it's triggered) properties.

#### Predefined Events

##### Handled Exceptions

ASP.NET Boilerplate defines **AbpHandledExceptionData** and triggers
this event when it automatically handles an exception. This is
especially useful if you want to get more information about exceptions
(ASP.NET Boilerplate automatically logs all exceptions). You can
register to this event to be informed when an exception occurs.

##### Entity Changes

There are also generic event data classes for entity changes:
**EntityCreatingEventData&lt;TEntity&gt;,
EntityCreatedEventData&lt;TEntity&gt;**,
**EntityUpdatingEventData&lt;TEntity&gt;,
EntityUpdatedEventData&lt;TEntity&gt;,
EntityDeletingEventData&lt;TEntity&gt;** and
**EntityDeletedEventData&lt;TEntity&gt;**. Also, there are
**EntityChangingEventData&lt;TEntity&gt;** and
**EntityChangedEventData&lt;TEntity&gt;**. A change can be insert,
update or delete.

'ing' events (e.g. EntityUpdating) are triggered before committing a transaction.
This way, you can rollback the [unit of work](/Pages/Documents/Unit-Of-Work)
and prevent an operation by throwing an exception. 'ed'
events (e.g. EntityUpdated) are triggered after committing a transaction, for which
you cannot rollback the unit of work.

Entity change events are defined in the **Abp.Events.Bus.Entities**
namespace and are **automatically triggered** by ASP.NET Boilerplate
when an entity is inserted, updated or deleted. If you have a Person
entity, you can register to EntityCreatedEventData&lt;Person&gt; to be
informed when a new Person is created and inserted into the database. These
events also support inheritance. If the Student class is derived from the Person
class and you registered to EntityCreatedEventData&lt;Person&gt;, you
will be informed when a Person **or** Student is inserted.

### Triggering Events

Triggering an event is simple:

    public class TaskAppService : ApplicationService
    {
        public IEventBus EventBus { get; set; }
    
        public TaskAppService()
        {
            EventBus = NullEventBus.Instance;
        }
    
        public void CompleteTask(CompleteTaskInput input)
        {
            //TODO: complete the task in the database...
            EventBus.Trigger(new TaskCompletedEventData {TaskId = 42});
        }
    }

There are some overloads of the trigger method:

    EventBus.Trigger<TaskCompletedEventData>(new TaskCompletedEventData { TaskId = 42 }); //Explicitly declare generic argument
    EventBus.Trigger(this, new TaskCompletedEventData { TaskId = 42 }); //Set 'event source' as 'this'
    EventBus.Trigger(typeof(TaskCompletedEventData), this, new TaskCompletedEventData { TaskId = 42 }); //Call non-generic version (first argument is the type of the event class)

Another way of triggering events is to use the DomainEvents collection of
an AggregateRoot class (see related section in the [Entity
documentation](Entities.md)).

### Handling Events

To handle an event, you should implement the **IEventHandler&lt;T&gt;**
interface as shown below:

    public class ActivityWriter : IEventHandler<TaskCompletedEventData>, ITransientDependency
    {
        public void HandleEvent(TaskCompletedEventData eventData)
        {
            WriteActivity("A task is completed by id = " + eventData.TaskId);
        }
    }

The IEventHandler defines a HandleEvent method and implements it as shown
above.

EventBus is integrated into the dependency injection system. We implemented
ITransientDependency (above), so when a TaskCompleted event occurs, it
creates a new instance of the ActivityWriter class, calls its
HandleEvent method, and then disposes it. See [dependency
injection](/Pages/Documents/Dependency-Injection) for more info.

#### Handling Base Events

Eventbus supports the **inheritance** of events. For example, you can create
a **TaskEventData** base class with two derived classes: **TaskCompletedEventData**
and **TaskCreatedEventData**:

    public class TaskEventData : EventData
    {
        public Task Task { get; set; }
    }
    
    public class TaskCreatedEventData : TaskEventData
    {
        public User CreatorUser { get; set; }
    }
    
    public class TaskCompletedEventData : TaskEventData
    {
        public User CompletorUser { get; set; }
    }

You can then implement **IEventHandler&lt;TaskEventData&gt;** to handle
both of these events:

    public class ActivityWriter : IEventHandler<TaskEventData>, ITransientDependency
    {
        public void HandleEvent(TaskEventData eventData)
        {
            if (eventData is TaskCreatedEventData)
            {
                //...
            }
            else if (eventData is TaskCompletedEventData)
            {
                //...
            }
        }
    }

You can implement IEventHandler&lt;EventData&gt; to
handle all the events in an application. You probably don't want that, but
it's possible.

#### Exception Handlers

The EventBus **triggers all handlers** even if any of them throw
an exception. If only one of them throws an exception, then it's directly
thrown by the Trigger method. If more than one handler throws an exception,
EventBus throws a single **AggregateException** for all of them.

#### Handling Multiple Events

You can handle **multiple events** in a single handler. If so,
you should implement IEventHandler&lt;T&gt; for each event. Example:

    public class ActivityWriter :
        IEventHandler<TaskCompletedEventData>,
        IEventHandler<TaskCreatedEventData>,
        ITransientDependency
    {
        public void HandleEvent(TaskCompletedEventData eventData)
        {
            //TODO: handle the event...
        }
    
        public void HandleEvent(TaskCreatedEventData eventData)
        {
            //TODO: handle the event...
        }
    }

### Registration Of Handlers

We have to register handlers to the event bus in order to handle events.

#### Automatically

ASP.NET Boilerplate finds all the classes that implement **IEventHandler**
that are registered with **dependency injection** (for example, by implementing
ITransientDependency as the samples above). It then registers them to
the event bus **automatically**. When an event occurs, it uses
dependency injection to get a reference to the handler and, after handling the event,
releases it. This is the **suggested** way of using the
event bus in ASP.NET Boilerplate.

#### Manually

It is also possible to manually register to events, but use it with
caution! In a web application, event registration should be done at
the start of the application. It's not a good approach to register to an event in a
web request, since registered classes remain registered after the request's
completion and would be re-registered for each request. This may cause problems
for your application since a registered class can be called multiple
times. Keep in mind that manual registrations do not use the
dependency injection system.

There are some overloads of the register method in the event bus. The
simplest one uses a delegate (or a lambda):

    EventBus.Register<TaskCompletedEventData>(eventData =>
        {
            WriteActivity("A task is completed by id = " + eventData.TaskId);
        });


When a 'task completed' event occurs, this lambda method is
called. The second one waits for an object that implements
IEventHandler&lt;T&gt;:

    EventBus.Register<TaskCompletedEventData>(new ActivityWriter());

The same instance of ActivityWriter is called for events. This method
also has a non-generic overload. Another overload accepts two generic
arguments:

    EventBus.Register<TaskCompletedEventData, ActivityWriter>();

In this case, the event bus creates a new ActivityWriter for each event. It
then calls the ActivityWriter.Dispose method if it's
[disposable](http://msdn.microsoft.com/en-us/library/system.idisposable.aspx).

Lastly, you can register an **event handler factory** to handle the creation
of handlers. A handler factory has two methods: **GetHandler** and
**ReleaseHandler**. Example:

    public class ActivityWriterFactory : IEventHandlerFactory
    {
        public IEventHandler GetHandler()
        {
            return new ActivityWriter();
        }
    
        public void ReleaseHandler(IEventHandler handler)
        {
            //TODO: release/dispose the activity writer instance (handler)
        }
    }

There is also a special factory class, **IocHandlerFactory**, that
can be used with the dependency injection system to create and release
handlers. ASP.NET Boilerplate also uses this class in automatic
registrations, so if you want to use the dependency injection system,
directly use the automatic registration as defined above.

### Unregistration

When you **manually** register to an event bus, you may want to
**unregister** the event later. The simplest way of unregistering an
event is disposing the return value of the **Register** method. Example:

    //Register to an event...
    var registration = EventBus.Register<TaskCompletedEventData>(eventData => WriteActivity("A task is completed by id = " + eventData.TaskId) );
    
    //Unregister from event
    registration.Dispose();


Most likely, the unregistration will be somewhere else and at a later time. Keep the
registration object and dispose it when you want to unregister. All
overloads of the Register method return a disposable object to
unregister to the event.

The EventBus also provides the **Unregister** method. Example usage:

    //Create a handler
    var handler = new ActivityWriter();
    
    //Register to the event
    EventBus.Register<TaskCompletedEventData>(handler);
    
    //Unregister from event
    EventBus.Unregister<TaskCompletedEventData>(handler);


This also provides overloads to unregister delegates and factories.
Unregistering a handler object must be done on the same object which was registered
before.

Lastly, EventBus provides a **UnregisterAll&lt;T&gt;()** method to
unregister all the handlers of an event and a **UnregisterAll()** method to
unregister all the handlers of all the events.

### Background Events

If you don't want to execute event handlers right away, you can queue events in a background job like below;

	public class MyService
	{
		private readonly IBackgroundJobManager _backgroundJobManager;
	
		public MyService(IBackgroundJobManager backgroundJobManager)
		{
			_backgroundJobManager = backgroundJobManager;
		}
	
		public void Test()
		{
			_backgroundJobManager.EnqueueEventAsync(new MySimpleEventData());
		}
	}
Using this approach, related event will be triggered when the background job is executed.