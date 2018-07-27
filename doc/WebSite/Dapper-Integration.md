### Introduction

[Dapper](https://github.com/StackExchange/Dapper) is an
object-relational mapper (ORM) for .NET.
The [Abp.Dapper](https://www.nuget.org/packages/Abp.Dapper) package simply
integrates Dapper to ASP.NET Boilerplate. It works as a secondary ORM
provider along with EF 6.x, EF Core or NHibernate.

### Installation

Before you start, you need to install
[Abp.Dapper](https://www.nuget.org/packages/Abp.Dapper) and either EF
Core, EF 6.x or the NHibernate ORM NuGet packages in to the project you want
to use.

#### Module Registration

First you need to add the **DependsOn** attribute for the **AbpDapperModule** on
your module where you register it:

    [DependsOn(
         typeof(AbpEntityFrameworkCoreModule),
         typeof(AbpDapperModule)
    )]
    public class MyModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SampleApplicationModule).GetAssembly());
        }
    }

**Note** that the AbpDapperModule dependency should be added later than the EF
Core dependency.

#### Entity to Table Mapping

You can configure mappings. For example, the **Person** class maps to the
**Persons** table in the following example:

    public class PersonMapper : ClassMapper<Person>
    {
        public PersonMapper()
        {
            Table("Persons");
            Map(x => x.Roles).Ignore();
            AutoMap();
        }
    }

You should set the assemblies that contain mapper classes. Example:

    [DependsOn(
         typeof(AbpEntityFrameworkModule),
         typeof(AbpDapperModule)
    )]
    public class MyModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SampleApplicationModule).GetAssembly());
            DapperExtensions.SetMappingAssemblies(new List<Assembly> { typeof(MyModule).GetAssembly() });
        }
    }
                

### Usage

After registering **AbpDapperModule**, you can use the Generic
IDapperRepository interface (instead of standard IRepository) to inject
dapper repositories.

    public class SomeApplicationService : ITransientDependency
    {
        private readonly IDapperRepository<Person> _personDapperRepository;
        private readonly IRepository<Person> _personRepository;

        public SomeApplicationService(
            IRepository<Person> personRepository,
            IDapperRepository<Person> personDapperRepository)
        {
            _personRepository = personRepository;
            _personDapperRepository = personDapperRepository;
        }

        public void DoSomeStuff()
        {
            var people = _personDapperRepository.Query("select * from Persons");
        }
    }

You can use both EF and Dapper repositories at the same
time and in the same transaction!
