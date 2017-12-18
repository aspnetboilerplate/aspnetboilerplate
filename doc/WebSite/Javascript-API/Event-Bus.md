### Introduction

**Pub/sub** event model is widely used in the client side. ASP.NET
Boilerplate includes a **simple global event bus** to **register** to
and **trigger** **events**.

### Register To Events

You can use **abp.event.on** to **register** to a **global event**. An
example registration:

    abp.event.on('itemAddedToBasket', function (item) {
        console.log(item.name + ' is added to basket!');
    });

Firt argument is **unique name of the event**. Second one is a
**callback function** that is called when the specified event is
triggered.

You can use **abp.event.off** method to **unregister** from an event.
Notice that; same function should be provided in order to unregister.
So, for the example above, you should set the callback function to a
variable, then use both in **on** and **off** methods.

### Trigger Events

**abp.event.trigger** is used to **trigger** a **global event**. Example
trigger code for the event registered above:

    abp.event.trigger('itemAddedToBasket', {
        id: 42,
        name: 'Acme Light MousePad'
    });

Firt argument is **unique name of the event**. Second one is the
(optional) **event argument**. You can add any number of arguments and
get them in the callback method.
