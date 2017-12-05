In C\#, a class can define own events and other classes can register it
to be notified when something happen. This is useful for a desktop
application or standalone windows service. But, for a web application
it's a bit problematic since objects are created in a web request and
they are short-lived. It's hard to register some class's events. Also,
directly registering to another class's event makes classes tightly
coupled.

Domain events can be used to decouple business logic and react to
important domain changes in an application.

### EventBus

EventBus is a **singleton** object that is shared by all other classes
to trigger and handle events. To use the event bus, you should get a
reference to it. You can do it in two way.

#### Injecting IEventBus

You can use [dependency
injection](/Pages/Documents/Dependency-Injection) to get a reference to
**IEventBus**. Here, we used property injection pattern:

    public class TaskAppService : ApplicationService
    {
        public IEventBus EventBus { get; set; }

        public TaskAppService()
        {
            EventBus = NullEventBus.Instance;
        }
    }

Property injection is more proper than constructor injection to inject
the event bus. Thus, your class can work without event bus. NullEventBus
implements [null object
pattern](http://en.wikipedia.org/wiki/Null_Object_pattern). When you
call it's methods, it does nothing at all.

#### Getting The Default Instance

If you can not inject it, you can directly use **EventBus.Default**.
It's the global event bus and can be used as shown below:

    EventBus.Default.Trigger(...); //trigger an event

It's **not suggested** to directly use EventBus.Default wherever
possible since it makes unit testing harder.

### Defining Events

Before trigger an event, you should define the event first. An event is
represented by a class that is derived from **EventData**. Assume that
we want to trigger an event when a task is completed:

    public class TaskCompletedEventData : EventData
    {
        public int TaskId { get; set; }
    }

This class contains properties those are needed by class that handles
the event. **EventData** class defines **EventSource** (which object
triggered the event) and **EventTime** (when it's triggered) properties.

#### Predefined Events

##### Handled Exceptions

ASP.NET Boilerplate defines **AbpHandledExceptionData** and triggers
this event when it automatically handles any exception. This is
especially useful if you want to get more information about exceptions
(even ASP.NET Boilerplate automatically logs all exceptions). You can
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

'ing' events (ex: EntityUpdating) are triggered before saving changes.
So, you can rollback the [unit of work](/Pages/Documents/Unit-Of-Work)
to prevent the operation by throwing exceptions in these events. 'ed'
events (ex: EntityUpdated) are triggered after saving changes and no
chance to rollback the unit of work.

Entity change events are defined in **Abp.Events.Bus.Entities**
namespace and are **automatically triggered** by ASP.NET Boilerplate
when an entity is inserted, updated or deleted. If you have a Person
entity, can register to EntityCreatedEventData&lt;Person&gt; to be
informed when a new Person created and inserted to database. These
events also supports inheritance. If Student class derived from Person
class and you registered to EntityCreatedEventData&lt;Person&gt;, you
will be informed when a Person or Student is inserted.

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
            //TODO: complete the task on database...
            EventBus.Trigger(new TaskCompletedEventData {TaskId = 42});
        }
    }

There are some overloads of the trigger method:

    EventBus.Trigger<TaskCompletedEventData>(new TaskCompletedEventData { TaskId = 42 }); //Explicitly declare generic argument
    EventBus.Trigger(this, new TaskCompletedEventData { TaskId = 42 }); //Set 'event source' as 'this'
    EventBus.Trigger(typeof(TaskCompletedEventData), this, new TaskCompletedEventData { TaskId = 42 }); //Call non-generic version (first argument is the type of the event class)

Another way of triggering events can be using DomainEvents collection of
AggregateRoot class (see related section in the [Entity
documentation](Entities.md)).

### Handling Events

To handle an event, you should implement **IEventHandler&lt;T&gt;**
interface as shown below:

    public class ActivityWriter : IEventHandler<TaskCompletedEventData>, ITransientDependency
    {
        public void HandleEvent(TaskCompletedEventData eventData)
        {
            WriteActivity("A task is completed by id = " + eventData.TaskId);
        }
    }

IEventHandler defines HandleEvent method and we implemented it as shown
above.

EventBus is integrated to dependency injection system. As we implemented
ITransientDependency above, when a TaskCompleted event occured, it
creates a new instance of ActivityWriter class and calls it's
HandleEvent method, then disposes it. See [dependency
injection](/Pages/Documents/Dependency-Injection) for more.

#### Handling Base Events

Eventbus supports **inheritance** of events. For example, you can create
a **TaskEventData** and two derived classes: **TaskCompletedEventData**
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

Then you can implement **IEventHandler&lt;TaskEventData&gt;** to handle
both of the events:

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

That also means you can implement IEventHandler&lt;EventData&gt; to
handle all events in the application. You probably don't want that, but
it's possible.

#### Handler Exceptions

EventBus **triggers all handlers** even one/some of them throws
exception. If only one of them throws exception, then it's directly
thrown by the Trigger method. If more than one handler throws exception,
EventBus throws a single **AggregateException** for all of them.

#### Handling Multiple Events

You can handle **multiple events** in a single handler. In this time,
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

We must register the handler to the event bus in order to handle events.

#### Automatically

ASP.NET Boilerplate finds all classes those implement **IEventHandler**
and registered to **dependency injection** (for example, by implementing
ITransientDependency as the samples above). Then it registers them to
the eventbus **automatically**. When an event occures, it uses
dependency injection to get a reference to the handler and releases the
handler after handling the event. This is the **suggested** way of using
event bus in ASP.NET Boilerplate.

#### Manually

It is also possible to manually register to events but use it with
caution. In a web application, event registration should be done at
application start. It's not a good approach to register to an event in a
web request since registered classes remain registered after request
completion and re-registering for each request. This may cause problems
for your application since registered class can be called multiple
times. Also keep in mind that manual registration does not use
dependency injection system.

There are some overloads of register method of the event bus. The
simplest one waits a delegate (or a lambda):

    EventBus.Register<TaskCompletedEventData>(eventData =>
        {
            WriteActivity("A task is completed by id = " + eventData.TaskId);
        });
                

Thus, then a 'task completed' event occurs, this lambda method is
called. Second one waits an object that implements
IEventHandler&lt;T&gt;:

    EventBus.Register<TaskCompletedEventData>(new ActivityWriter());

Same instance of ActivityWriter is called for events. This method has
also a non-generic overload. Another overload accepts two generic
arguments:

    EventBus.Register<TaskCompletedEventData, ActivityWriter>();

In this time, event bus creates a new ActivityWriter for each event. It
calls ActivityWriter.Dispose method if it's
[disposable](http://msdn.microsoft.com/en-us/library/system.idisposable.aspx).

Lastly, you can register an **event handler factory** to handle creation
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

There is also a special factory class, the **IocHandlerFactory**, that
can be used to use dependency injection system to create/release
handlers. ASP.NET Boilerplate also uses this class in automatic
registration. So, if you want to use dependency injection system,
directly use automatic registration defined before.

### Unregistration

When you **manually** register to event bus, you may want to
**unregister** to the event later. Simplest way of unregistering an
event is disposing the return value of the **Register** method. Example:

    //Register to an event...
    var registration = EventBus.Register<TaskCompletedEventData>(eventData => WriteActivity("A task is completed by id = " + eventData.TaskId) );

    //Unregister from event
    registration.Dispose();
                

Surely, unregistration will be somewhere and sometime else. Keep
registration object and dispose it when you want to unregister. All
overloads of the Register method returns a disposable object to
unregister to the event.

EventBus also provides **Unregister** method. Example usage:

    //Create a handler
    var handler = new ActivityWriter();

    //Register to the event
    EventBus.Register<TaskCompletedEventData>(handler);

    //Unregister from event
    EventBus.Unregister<TaskCompletedEventData>(handler);
                

It also provides overloads to unregister delegates and factories.
Unregistering handler object must be the same object which is registered
before.

Lastly, EventBus provides a **UnregisterAll&lt;T&gt;()** method to
unregister all handlers of an event and **UnregisterAll()** method to
unregister all handlers of all events.
