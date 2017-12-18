### Introduction

It's a common to map a similar object to another object. It's also
tedious and repeating since generally both objects (classes) may have
similar/same properties mapped to each other. Think on a typical
[application service](Application-Services.md) method below:

    public class UserAppService : ApplicationService
    {
        private readonly IRepository<User> _userRepository;

        public UserAppService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public void CreateUser(CreateUserInput input)
        {
            var user = new User
            {
                Name = input.Name,
                Surname = input.Surname,
                EmailAddress = input.EmailAddress,
                Password = input.Password
            };

            _userRepository.Insert(user);
        }
    }

CreateUserInput is a simple [DTO](Data-Transfer-Objects.md) and User
is a simple [entity](Entities.md) here. We manually created a User
entity from given input. User entity will have much more properties in a
real world application and manually creating it will become tedious and
error-prone. Also, we should change the mapping code when we want to add
new properties to User and CreateUserInput.

We can use a library to make this mapping automatically.
[AutoMapper](http://automapper.org/) is one of the best libraries for
object to object mapping. ASP.NET Boilerplate defines **IObjectMapper**
interface to abstract it and implements this interface using AutoMapper
in [Abp.AutoMapper](https://www.nuget.org/packages/Abp.AutoMapper)
package.

### IObjectMapper Interface

IObjectMapper is a simple abstraction that has Map methods to map an
object to another. We can write the sample code above as like below:

    public class UserAppService : ApplicationService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IObjectMapper _objectMapper;

        public UserAppService(IRepository<User> userRepository, IObjectMapper objectMapper)
        {
            _userRepository = userRepository;
            _objectMapper = objectMapper;
        }

        public void CreateUser(CreateUserInput input)
        {
            var user = _objectMapper.Map<User>(input);
            _userRepository.Insert(user);
        }
    }

Map is a simple method gets the source object and creates a new
destination object with the type declared as the generic parameter (User
in this sample). Map method has an overload to map an object to an
**existing** object. Assume that we already have a User entity and want
to update it's properties by an object:

    public void UpdateUser(UpdateUserInput input)
    {
        var user = _userRepository.Get(input.Id);
        _objectMapper.Map(input, user);
    }

### AutoMapper Integration

**[Abp.AutoMapper](https://www.nuget.org/packages/Abp.AutoMapper)**
nuget package ([module](Module-System.md)) implements IObjectMapper
and provides additional features.

#### Installation

First, install **Abp.AutoMapper** nuget to your project:

    Install-Package Abp.AutoMapper

Then add a dependency for **AbpAutoMapperModule** to your module
definition class:

    [DependsOn(typeof(AbpAutoMapperModule))]
    public class MyModule : AbpModule
    {
        ...
    }

Then you can safely [inject](Dependency-Injection.md) and use
IObjectMapper in your code. You can also use
[AutoMapper](http://automapper.org/)'s own API when you need.

#### Creating Mappings

AutoMapper requires to define mappings between classes (by default)
before using the mapping. You can see it's own
[documentation](http://automapper.org/) for details on mapping. ASP.NET
Boilerplate makes it a bit easier and modular.

##### Auto Mapping Attributes

Most of time you only want to directly (and conventionally) map classes.
In that case, you can use **AutoMap**, **AutoMapFrom** and **AutoMapTo**
attributes. For instance, as we want to map **CreateUserInput** class to
**User** class in the sample above, we can use **AutoMapTo** attribute
as shown below:

    [AutoMapTo(typeof(User))]
    public class CreateUserInput
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }
    }

AutoMap attribute maps two classes in both direction. But in this
sample, we only need to map from CreateUserInput to User, so we used
AutoMapTo.

##### Custom Mapping

Simple mapping may not be suitable in some cases. For instance, property
names of two classes may be a little different or you may want to ignore
some properties during the mappping. In such cases you should directly
use AutoMapper's API to define the mapping. Abp.AutoMapper package
defines API to make this custom mapping stuff more modular.

Assume that we want to ignore Password on mapping and User has Email
property for email address. We can define mapping as shown below:

    [DependsOn(typeof(AbpAutoMapperModule))]
    public class MyModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                config.CreateMap<CreateUserInput, User>()
                      .ForMember(u => u.Password, options => options.Ignore())
                      .ForMember(u => u.Email, options => options.MapFrom(input => input.EmailAddress));
            });
        }
    }

AutoMapper has much more options and abilities for object to object
mapping. See it's [documentation](http://automapper.org/) for more.

#### MapTo Extension Methods

It's suggested to inject and use IObjectMapper interface as defined
before. This makes our project independent from AutoMapper as much as
possible. It also makes unit testing easier since we can replace (mock)
the mapping in unit tests.

Abp.AutoMapper module also defines MapTo extension methods which can be
used on any object to map it to another object without injecting
IObjectMapper. Example usage:

    public class UserAppService : ApplicationService
    {
        private readonly IRepository<User> _userRepository;

        public UserAppService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public void CreateUser(CreateUserInput input)
        {
            var user = input.MapTo<User>();
            _userRepository.Insert(user);
        }

        public void UpdateUser(UpdateUserInput input)
        {
            var user = _userRepository.Get(input.Id);
            input.MapTo(user);
        }
    }

MapTo extension methods are defined in Abp.AutoMapper namespace, so you
first import this namespaces into your code file.

Since MapTo extension methods are statics, they use AutoMapper's static
instance (Mapper.Instance). This is simple and fine for the application
code, but can have problems in unit tests since static configuration and
mapper is shared among different tests and they may effect each other.

#### Unit Tests

We want to isolate tests from each others. To do that, we should design
our project with the following rules:

1. Always use IObjectMapper, not use MapTo extension methods.

2. Configure Abp.AutoMapper module to use local Mapper instance
(registered as singleton to dependency injection) rather than the static
one (Abp.AutoMapper uses the static Mapper.Instance by default to allow
to use MapTo extension methods defined above):

    Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

#### Pre Defined Mappings

##### LocalizableString -&gt; string

Abp.AutoMapper module defines a mapping to convert LocalizableString (or
ILocalizableString) objects to string objects. It makes the conversion
using [ILocalizationManager](Localization.md), so localizable
properties are automatically localized during the mapping process of any
class.

#### Injecting IMapper

You may need to directly use AutoMapper's IMapper object instead of
IObjectMapper abstraction. In that case, just inject IMapper in your
classes and use it. Abp.AutoMapper package registers IMapper to
[dependency injection](Dependency-Injection.md) as singleton.
