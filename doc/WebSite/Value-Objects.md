### Introduction

"*An object that represents a descriptive aspect of the domain with no
conceptual identity is called a VALUE OBJECT.*" (Eric Evans).

[Entities](Entities.md) have identities
(Id), Value Objects do not. If the identities of two
Entities are different, they are considered as different
objects/entities even if all the properties of those entities are the
same. Imagine two different people that have the same Name, Surname and Age but
are different people (their identity numbers are different). For an Address class (which 
is a classic Value Object), if the two addresses have the same Country, City, and Street number, etc,
they are considered to be the same address.

In Domain Driven Design (DDD), the Value Object is another type of domain
object which can include business logic and is an essential part of the
domain.

### Value Object Base Classes

ABP has two base classes for value objects: **ValueObject** and **ValueObject&lt;T&gt;** . Both classes override the equality operator (and other related operator and methods) to compare the two value objects and assume that they are identical if all the properties are the same. For example, all of these tests pass:

```
var address1 = new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);
var address2 = new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);

Assert.Equal(address1, address2);
Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
Assert.True(address1 == address2);
Assert.False(address1 != address2);
```

Even if they are different objects in memory, they are identical for our domain.

The difference between **ValueObject** and **ValueObject&lt;T&gt;** is the implementation of comparison. The **ValueObject&lt;T&gt;** uses reflection, while the **ValueObject** waits from you to return a list of properties.

So, the **ValueObject&lt;T&gt;** is simpler to inherit, but the **ValueObject** is more efficient.

#### ValueObject

Here's an example **Address** that inherits from the **ValueObject** class:

```csharp
public class Address : ValueObject
{
    public Guid CityId { get; }

    public string Street { get; }

    public int Number { get; }

    public Address(
        Guid cityId,
        string street,
        int number)
    {
        CityId = cityId;
        Street = street;
        Number = number;
    }

    //Requires to implement this method to return properties.
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return CityId;
        yield return Number;
    }
}
```

#### **ValueObject&lt;T&gt;**

Here's the same Address class that inherits from the **ValueObject&lt;T&gt;**.

````csharp
public class Address : ValueObject<Address>
{
    public Guid CityId { get; }

    public string Street { get; }

    public int Number { get; }

    public AddressAnother(
        Guid cityId,
        string street,
        int number)
    {
        CityId = cityId;
        Street = street;
        Number = number;
    }
}
````



### Best Practices

Here are some best practices when using Value Objects:

-   Design a value object as **immutable** (like the Address above)
    if there is not a good reason for designing it as mutable.
-   The properties that make up a Value Object should form a conceptual
    whole. For example, CityId, Street and Number shouldn't be separate
    properties of a Person entity. This also makes the Person entity
    simpler.
