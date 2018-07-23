### Introduction

**OData** is defined as "An **open protocol** to allow for the creation and
consumption of **queryable** and **interoperable RESTful APIs** in a
**simple** and **standard** way". See [odata.org](http://www.odata.org/).
You can use OData with ASP.NET Boilerplate.

The [Abp.Web.Api.OData](https://www.nuget.org/packages/Abp.Web.Api.OData)
NuGet package simplifies its usage.

### Setup

#### Install NuGet Package

We should first install the Abp.Web.Api.OData NuGet package to our WebApi
project:

    Install-Package Abp.Web.Api.OData

#### Set Module Dependency

We should set the dependency to AbpWebApiODataModule from our module.
Example:

    [DependsOn(typeof(AbpWebApiODataModule))]
    public class MyProjectWebApiModule : AbpModule
    {
        ...
    }

See the [module system documentation](/Pages/Documents/Module-System) to understand module
dependencies.

#### Configure Your Entities

OData requires you to declare entities which can be used as OData resources.
We should do this in the
[PreInitialize](/Pages/Documents/Module-System#preinitialize) method
of our module, as shown below:

    [DependsOn(typeof(AbpWebApiODataModule))]
    public class MyProjectWebApiModule : AbpModule
    {
        public override void PreInitialize()
        {
            var builder = Configuration.Modules.AbpWebApiOData().ODataModelBuilder;

            // Configure your entities here...
            builder.EntitySet<Person>("Persons");
        }

        ...
    }

Here, we get the ODataModelBuilder reference and set the Person entity.
Similarly, you can use EntitySet to add other entities. See the [OData
documentation](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint)
for more information on the builder.

### Create Controllers

Abp.Web.Api.OData NuGet package includes the **AbpODataEntityController**
base class (which extends standard ODataController) to create your
controllers easily. 

Here's an example on how to create an OData endpoint for the Person
entity:

    public class PersonsController : AbpODataEntityController<Person>, ITransientDependency 
    {
        public PersonsController(IRepository<Person> repository)
            : base(repository)
        {
        }
    }

It's that easy. All the methods of AbpODataEntityController are **virtual**.
That means you can override **Get**, **Post**, **Put**, **Patch**,
**Delete** and other actions and add your own logic.

### Configuration

Abp.Web.Api.OData automatically calls
HttpConfiguration.MapODataServiceRoute method with the conventional
configuration. If you need to, you can set the
Configuration.Modules.AbpWebApiOData().**MapAction** to map OData routes
yourself.

### Examples

Here are some example requests to the controller defined above. Assume that
the application works on *http://localhost:61842*. We will show you some
basics. Since OData is a standard protocol, you can easily find more
advanced examples on the web.

#### Getting a List of Entities

Getting all people.

##### Request

    GET http://localhost:61842/odata/Persons

##### Response

    {
      "@odata.context":"http://localhost:61842/odata/$metadata#Persons","value":[
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

    GET http://localhost:61842/odata/Persons(2)

##### Response

    {
      "@odata.context":"http://localhost:61842/odata/$metadata#Persons/$entity","Name":"John Nash","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":2
    }

#### Getting a Single Entity With Navigation Properties

Getting the person with Id = 1 including his phone numbers.

##### Request

    GET http://localhost:61842/odata/Persons(1)?$expand=Phones

##### Response

    {
      "@odata.context":"http://localhost:61842/odata/$metadata#Persons/$entity","Name":"Douglas Adams","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":1,"Phones":[
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

    GET http://localhost:61842/odata/Persons?$filter=Name eq 'Douglas Adams'&$orderby=CreationTime&$top=2

##### Response

    {
      "@odata.context":"http://localhost:61842/odata/$metadata#Persons","value":[
        {
          "Name":"Douglas Adams","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2015-11-07T20:12:39.363+03:00","CreatorUserId":null,"Id":1
        },{
          "Name":"Douglas Adams","IsDeleted":false,"DeleterUserId":null,"DeletionTime":null,"LastModificationTime":null,"LastModifierUserId":null,"CreationTime":"2016-01-12T20:29:03+02:00","CreatorUserId":null,"Id":3
        }
      ]
    }

OData supports paging, sorting, filtering, projections and much more.
See the [OData documentation](http://www.odata.org/) for more
information.

#### Creating a New Entity

In this example, we're creating a new person.

##### Request

    POST http://localhost:61842/odata/Persons

    {
        Name: "Galileo Galilei"
    }

Here, the "Content-Type" header is "application/json".

##### Response

    {
      "@odata.context": "http://localhost:61842/odata/$metadata#Persons/$entity",
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
or delete an existing entity as OData supports it.

#### Getting MetaData

We can get the metadata of entities, as shown in this example.

##### Request

    GET http://localhost:61842/odata/$metadata

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

You can see the source code of the sample project here:
<https://github.com/aspnetboilerplate/sample-odata>
