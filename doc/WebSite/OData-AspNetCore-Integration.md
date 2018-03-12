### Introduction

**OData** is defined as "An **open protocol** to allow the creation and
consumption of **queryable** and **interoperable RESTful APIs** in a
**simple** and **standard** way" ([odata.org](http://www.odata.org/)).
You can use OData with ASP.NET Boilerplate. The
[Abp.AspNetCore.OData](https://www.nuget.org/packages/Abp.AspNetCore.OData)
NuGet package simplifies its usage.

### Setup

#### Install NuGet Package

We must first install the Abp.AspNetCore.OData NuGet package to our Web.Core
project:

    Install-Package Abp.AspNetCore.OData

#### Set Module Dependency

We need to set a dependency on AbpAspNetCoreODataModule for our module.
Example:

    [DependsOn(typeof(AbpAspNetCoreODataModule))]
    public class MyProjectWebCoreModule : AbpModule
    {
        ...
    }

See the [module system](/Pages/Documents/Module-System) to understand module
dependencies.

#### Configure Your Entities

OData requires us to declare entities which can be used as OData resources.
We must do this in the Startup class:

    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ...

            services.AddOData();

            // Workaround: https://github.com/OData/WebApi/issues/1177
            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });

            return services.AddAbp<MyProjectWebHostModule>(...);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp();

            ...

            app.UseOData(builder =>
            {
                builder.EntitySet<Person>("Persons").EntityType.Expand().Filter().OrderBy().Page();
            });

            // Return IQueryable from controllers
            app.UseUnitOfWork(options =>
            {
                options.Filter = httpContext =>
                {
                    return httpContext.Request.Path.Value.StartsWith("/odata");
                };
            });

            app.UseMvc(routes =>
            {
                routes.MapODataServiceRoute(app);

                ...
            });
        }
    }

Here, we got the ODataModelBuilder reference and set the Person entity.
You can use EntitySet to add other entities in a similar way. See the [OData
documentation](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint)
for more information on the builder.

### Create Controllers

The Abp.AspNetCore.OData NuGet package includes the **AbpODataEntityController**
base class (which extends standard ODataController) to create your
controllers easier. An example to create an OData endpoint for the Person
entity:

    public class PersonsController : AbpODataEntityController<Person>, ITransientDependency 
    {
        public PersonsController(IRepository<Person> repository)
            : base(repository)
        {
        }
    }

It's that easy! All the methods of AbpODataEntityController are **virtual**.
This means that you can override the **Get**, **Post**, **Put**, **Patch**,
**Delete** and other actions and add your own logic.

### Configuration

Abp.AspNetCore.OData calls the
IRouteBuilder.MapODataServiceRoute method with the conventional
configuration. If you need to, you can set
Configuration.Modules.AbpAspNetCoreOData().**MapAction** to map OData routes
yourself.

### Examples

Here are some requests made to the controller defined above. Assume that
the application works on *http://localhost:21021*. We will show some
basic examples. Since OData is a standard protocol, you can easily find more
advanced examples on the web.

#### Getting List of Entities

Getting all people.

##### Request

    GET http://localhost:21021/odata/Persons

##### Response

    {
      "@odata.context":"http://localhost:21021/odata/$metadata#Persons","value":[
        {
          "Name":"Douglas Adams","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":1
        },{
          "Name":"John Nash","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":2
        }
      ]
    }

#### Getting a Single Entity

Getting the person with Id = 2.

##### Request

    GET http://localhost:21021/odata/Persons(2)

##### Response

    {
      "@odata.context":"http://localhost:21021/odata/$metadata#Persons/$entity","Name":"John Nash","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":2
    }

#### Getting a Single Entity With Navigation Properties

Getting the person with Id = 1 including his/her phone numbers.

##### Request

    GET http://localhost:21021/odata/Persons(1)?$expand=Phones

##### Response

    {
      "@odata.context":"http://localhost:21021/odata/$metadata#Persons/$entity","Name":"Douglas Adams","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":1,"Phones":[
        {
          "PersonId":1,"Type":"Mobile","Number":"4242424242","CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":1
        },{
          "PersonId":1,"Type":"Mobile","Number":"2424242424","CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":2
        }
      ]
    }

#### Querying

Here's a more advanced query that includes filtering, sorting and getting the top
2 results.

##### Request

    GET http://localhost:21021/odata/Persons?$filter=Name eq 'Douglas Adams'&$orderby=CreationTime&$top=2

##### Response

    {
      "@odata.context":"http://localhost:21021/odata/$metadata#Persons","value":[
        {
          "Name":"Douglas Adams","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":1
        }
      ]
    }

OData supports paging, sorting, filtering, projections and much more.
See [its own documentation](http://www.odata.org/) for more
information.

#### Creating a New Entity

In this example, we're creating a new person.

##### Request

    POST http://localhost:21021/odata/Persons

    {
        Name: "Galileo Galilei"
    }

Here, the "Content-Type" header is "application/json".

##### Response

    {
      "@odata.context": "http://localhost:21021/odata/$metadata#Persons/$entity",
      "Name": "Galileo Galilei",
      "IsDeleted": false,
      "DeleterUserId": null,
      "DeletionTime": null,
      "LastModificationTime": null,
      "LastModifierUserId": null,
      "CreationTime": "2016-01-12T20:36:04.1628263+02:00",
      "CreatorUserId": null,
      "Id": 4
    }

If we get the list again, we can see the new person. We can also update
or delete an existing entity since OData supports it.

#### Getting MetaData

We can get the metadata of entities, as shown in this example.

##### Request

    GET http://localhost:21021/odata/$metadata

##### Response

    <?xml version="1.0" encoding="utf-8"?>
    <edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
        <edmx:DataServices>
            <Schema Namespace="AbpODataDemo.People" xmlns="http://docs.oasis-open.org/odata/ns/edm">

                <EntityType Name="Person">
                    <Key>
                        <PropertyRef Name="Id" />
                    </Key>
                    <Property Name="Name" Type="Edm.String" Nullable="false" />
                    <Property Name="IsDeleted" Type="Edm.Boolean" Nullable="false" />
                    <Property Name="DeleterUserId" Type="Edm.Int64" />
                    <Property Name="DeletionTime" Type="Edm.DateTimeOffset" />
                    <Property Name="LastModificationTime" Type="Edm.DateTimeOffset" />
                    <Property Name="LastModifierUserId" Type="Edm.Int64" />
                    <Property Name="CreationTime" Type="Edm.DateTimeOffset" Nullable="false" />
                    <Property Name="CreatorUserId" Type="Edm.Int64" />
                    <Property Name="Id" Type="Edm.Int32" Nullable="false" />
                    <NavigationProperty Name="Phones" Type="Collection(AbpODataDemo.People.Phone)" />
                </EntityType>

                <EntityType Name="Phone">
                    <Key>
                        <PropertyRef Name="Id" />
                    </Key>
                    <Property Name="PersonId" Type="Edm.Int32" />
                    <Property Name="Type" Type="AbpODataDemo.People.PhoneType" Nullable="false" />
                    <Property Name="Number" Type="Edm.String" Nullable="false" />
                    <Property Name="CreationTime" Type="Edm.DateTimeOffset" Nullable="false" />
                    <Property Name="CreatorUserId" Type="Edm.Int64" />
                    <Property Name="Id" Type="Edm.Int32" Nullable="false" />
                    <NavigationProperty Name="Person" Type="AbpODataDemo.People.Person">
                        <ReferentialConstraint Property="PersonId" ReferencedProperty="Id" />
                    </NavigationProperty>
                </EntityType>

                <EnumType Name="PhoneType">
                    <Member Name="Unknown" Value="0" />
                    <Member Name="Mobile" Value="1" />
                    <Member Name="Home" Value="2" />
                    <Member Name="Office" Value="3" />
                </EnumType>

            </Schema>
            <Schema Namespace="Default" xmlns="http://docs.oasis-open.org/odata/ns/edm">

                <EntityContainer Name="Container">
                    <EntitySet Name="Persons" EntityType="AbpODataDemo.People.Person" />
                </EntityContainer>

            </Schema>
        </edmx:DataServices>
    </edmx:Edmx>

Metadata is used to investigate the service.

### Sample Project

You can get the source code of the sample project here:
<https://github.com/aspnetboilerplate/sample-odata>
