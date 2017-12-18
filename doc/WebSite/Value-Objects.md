### Introduction

"*An object that represents a descriptive aspect of the domain with no
conceptual identity is called a VALUE OBJECT.*" (Eric Evans).

As opposite to [Entities](Entities.md), which have their identities
(Id), a Value Object has not it's identity. If identities of two
Entities are different, they are considered as different
objects/entities even if all other properties of those entities are
same. Think two different persons have same Name, Surname and Age but
they are different people if their identity numbers are different. But,
for an Address (which is a classic Value Object) class, if two addresses
has same Country, City, Street number... etc. they are considered as the
same address.

In Domain Driven Design (DDD), Value Object is another type of domain
object which can include business logic and is an essential part of the
domain.

### Value Object Base Class

ABP has a **ValueObject&lt;T&gt;** base class which can be inherited in
order to easily create Value Object types. Example **Address** Value
Object type:

    public class Address : ValueObject<Address>
    {
        public Guid CityId { get; private set; } //A reference to a City entity.

        public string Street { get; private set; }

        public int Number { get; private set; }

        public Address(Guid cityId, string street, int number)
        {
            CityId = cityId;
            Street = street;
            Number = number;
        }
    }

ValueObject base class overrides equality operator (and other related
operator and methods) to compare two value object and assumes that they
are identical if all properties are identical. So, all of these tests
pass:

    var address1 = new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);
    var address2 = new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);

    Assert.Equal(address1, address2);
    Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
    Assert.True(address1 == address2);
    Assert.False(address1 != address2);

Even they are different objects in memory, they are identical for our
domain.

### Best Practices

Here, some best practices for Value Objects:

-   Design a value object as **immutable** (as like the Address above)
    if there is not a very good reason for designing it as mutable.
-   The properties that make up a Value Object should form a conceptual
    whole. For example, CityId, Street and Number shouldn't be serarate
    properties of a Person entity. Also, this makes Person entity
    simpler.
