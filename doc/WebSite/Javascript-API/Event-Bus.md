### Introduction

The **Pub/sub** event model is widely used on the client-side. ASP.NET
Boilerplate includes a **simple global event bus** to **register** to
and **trigger events**.

### Registering To Events

You can use **abp.event.on** to **register** to a **global event**. An
example registration:

    abp.event.on('itemAddedToBasket', function (item) {
        console.log(item.name + ' was added to basket!');
    });

The firt argument is the **unique name of the event**. The second one is a
**callback function** that is called when the specified event is
triggered.

You can use the **abp.event.off** method to **unregister** from an event.
Note that the same function should be provided so it can be unregistered.
So for the example above, you must set the callback function to a
variable, then use both the **on** and **off** methods.

### Trigger Events

The **abp.event.trigger** is used to **trigger** a **global event**. Example
trigger code for the event registered above:

    abp.event.trigger('itemAddedToBasket', {
        id: 42,
        name: 'Acme Light MousePad'
    });

The first argument is the **unique name of the event**. The second one is the
(optional) **event argument**. You can add any number of arguments and
get them in the callback method.
