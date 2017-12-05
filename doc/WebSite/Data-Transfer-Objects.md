**Data Transfer Objects** are used to transfer data between
**Application Layer** and **Presentation Layer**.

Presentation Layer calls to an [Application
Service](/Pages/Documents/Application-Services) method with a Data
Transfer Object (**DTO**), then application service uses domain objects
to perform some specific business logic and returns a DTO back to the
presentation layer. Thus, Presentation layer is completely isolated from
Domain layer. In an ideally layered application, presentation layer does
not directly work with domain objects
([Repositories](/Pages/Documents/Repositories),
[Entities](/Pages/Documents/Entities)...).

### The need for DTOs

To create a DTO for each Application Service method can be seen as a
tedious and time-consuming work at first. But they can save your
application if you correctly use it. Why?

#### Abstraction of domain layer

DTOs provide an efficient way of abstracting domain objects from
presentation layer. Thus, your layers are correctly seperated. Even if
you want to change presentation layer completely, you can continue with
existing application and domain layers. As opposite, you can re-write
your domain layer, completely change database schema, entities and O/RM
framework without any change in presentation layer as long as contracts
(method signatures and DTOs) of your application services remain
unchanged.

#### Data hiding

Think that you have a User entity with properties Id, Name, EmailAddress
and Password fields. If GetAllUsers() method of UserAppService returns a
List&lt;User&gt;, anyone can see passwords of all users, even you do not
show it in the screen. It's not just about security, it's about data
hiding. Application service should return to presentation layer what it
needs. Not more, not less.

#### Serialization & lazy load problems

When you return a data (an object) to presentation layer, it's probably
serialized in somewhere. For example, in an MVC method that returns
JSON, your object can be serialized to JSON and sent to the client.
Returning an Entity to the presentation layer can be problematic in that
case. How?

In a real application, your entities will have references to each other.
User entity can have a reference to its Roles. So, if you want to
serialize User, its Roles are also serialized. And even Role class may
have a List&lt;Permission&gt; and Permission class can have a reference
to a PermissionGroup class and so on... Can you think that all of these
objects are serialized? You can easily serialize your whole database
accidently. Also, if your objects has circcular references, it can not
be serialized.

What's the solution? Marking properties as NonSerialized? No, you can
not know when it should be serialized and when it shouldn't be. It may
be needed in one application service method, may not be needed in other.
So, returning a safely serializable, specially designed DTOs are a good
choice in this situation.

Almost all O/RM frameworks support lazy-loading. It's a feature to load
entities from database when it's needed. Say User class has a reference
to Role class. When you get a User from database, Role property is not
filled. When you first read the Role property, it's loaded from
database. So, if you return such an Entity to presentation layer, it
will cause to retrieve additonal entities from database. If a
serialization tool reads the entity, it reads all properties recursively
and again your complete database can be retrieved (if there are suitable
relations between entities).

We can say some more problems about using Entities in presentation
layer. It's best to do not reference the assembly containing domain
(business) layer to the presentation layer at all.

### DTO conventions & validation

ASP.NET Boilerplate strongly supports DTOs. It provides some
conventional classes & interfaces and advices some naming and usage
conventions about DTOs. When you write your codes as described here,
ASP.NET Boilerplate automates some tasks easily. 

#### Example

Let's see a complete example. Say that we want to develop an application
service method that is used to **search people** with a name and returns
**list of people**. In that case, we may have a **Person**
[entity](/Pages/Documents/Entities) as shown below:

    public class Person : Entity
    {
        public virtual string Name { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string Password { get; set; }
    }

And we can define an interface for our [application
service](/Pages/Documents/Application-Services):

    public interface IPersonAppService : IApplicationService
    {
        SearchPeopleOutput SearchPeople(SearchPeopleInput input);
    }

ASP.NET Boilerplate suggest naming input/output parameters as
MethodName**Input** and MethodName**Output** and defining a seperated
input and output DTO for every application service method. Even if your
method only take/return **one** parameter, it's better to create a DTO
class. Thus, your code will be more extensible. You can add more
properties later without changing signature of your method and without
breaking existing client applications.

Surely, your method can return **void** if there is no return value. It
will not break existing applications if you add a return value later. If
your method does not get any argument, you do not have to define an
input DTO. But it maybe better to write an input DTO class if it's
probable to add parameter in the future. This is up to you.

Let's see input and output DTO classes defined for this example:

    public class SearchPeopleInput
    {
        [StringLength(40, MinimumLength = 1)]
        public string SearchedName { get; set; }
    }

    public class SearchPeopleOutput
    {
        public List<PersonDto> People { get; set; }
    }

    public class PersonDto : EntityDto
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
    }

ASP.NET Boilerplate **automatically validates** input before execution
of the method. It's similar to ASP.NET MVC's validation, but notice that
application service is not a Controller, it's a plain C\# class. ASP.NET
Boilerplate makes interception and check input automatically. There are
much more about validation. See [DTO
validation](/Pages/Documents/Validating-Data-Transfer-Objects) document.

**EntityDto** is a simple class that declares **Id property** since they
are common for entities. Has a generic version if your entity's primary
key is not int. You don't have to use it, but can be better to define an
Id property.

**PersonDto** does not include Password property as you see, since it's
not needed for presentation layer. Even it can be dangerous to send all
people's password to presentation layer. Think that a Javascript client
requested it, anyone can easily grab all passwords.

Let's implement **IPersonAppService** before go further.

    public class PersonAppService : IPersonAppService
    {
        private readonly IPersonRepository _personRepository;

        public PersonAppService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public SearchPeopleOutput SearchPeople(SearchPeopleInput input)
        {
            //Get entities
            var peopleEntityList = _personRepository.GetAllList(person => person.Name.Contains(input.SearchedName));

            //Convert to DTOs
            var peopleDtoList = peopleEntityList
                .Select(person => new PersonDto
                                    {
                                        Id = person.Id,
                                        Name = person.Name,
                                        EmailAddress = person.EmailAddress
                                    }).ToList();

            return new SearchPeopleOutput { People = peopleDtoList };
        }
    }

We get entities from database, **convert** them to DTOs and return
output. Notice that we did not validated input. ASP.NET Boilerplate
**validate**s it. It even checks **if input parameter is null** and
throws exception if so. This saves us to write guard clauses in every
method.

But, probably you did not like converting code from a Person entity to a
PersonDto object. It's really a tedious work. Person entity could have
much more properties.

### Auto mapping between DTOs and entities

Fortunately there are tools that makes this very easy.
**[AutoMapper](http://automapper.org/)** is one of them. See [AutoMapper
Integration](Object-To-Object-Mapping.html) document to know how to use
it.

### Helper interfaces and classes

ASP.NET provides some helper interfaces that can be implemented to
standardize common DTO property names.

**ILimitedResultRequest** defines **MaxResultCount** property. So, you
can implement it in your input DTOs to standardize limiting result set.

**IPagedResultRequest** extends **ILimitedResultRequest** by adding
**SkipCount**. So, we can implement this interface in SearchPeopleInput
for paging:

    public class SearchPeopleInput : IPagedResultRequest
    {
        [StringLength(40, MinimumLength = 1)]
        public string SearchedName { get; set; }

        public int MaxResultCount { get; set; }
        public int SkipCount { get; set; }
    }
                

As a result to a paged request, you can return an output DTO that
implements **IHasTotalCount**. Naming standardization helps us to create
re-usable codes and conventions. See other interfaces and classes under
**Abp.Application.Services.Dto** namespace.
