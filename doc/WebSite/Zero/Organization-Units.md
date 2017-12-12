### Introduction

Organization units (OU) can be used to **hierarchically group users and
entities**.

### OrganizationUnit Entity

An OU is represented by the **OrganizationUnit** entity. Fundamental
properties of this entity are;

-   **TenantId**: Tenant's Id of this OU. Can be null for host OUs.
-   **ParentId**: Parent OU's Id. Can be null if this is a root OU.
-   **Code**: A hierarchical string code that is unique for a tenant.
-   **DisplayName**: Shown name of the OU.

OrganizationUnit entitiy's primary key (Id) is **long** and it derives
from [**FullAuditedEntity**](/Pages/Documents/Entities#auditing)
which provides audit information and implements
[**ISoftDelete**](/Pages/Documents/Data-Filters#isoftdelete) interface
(so, OUs are not deleted from database, they are just marked as
deleted).

#### Organization Tree

Since an OU can have a parent, all OUs of a tenant are in a **tree**
structure. There are some rules for this tree;

-   There can be more than one root (which have null ParentId).
-   Maximum deep of tree is defined as a constant as
    OrganizationUnit.**MaxDepth**, which is **16**.
-   There is a limit for first-level children count of an OU (because of
    fixed OU Code unit length explained below).

#### OU Code

OU code is automatically generated and maintained by OrganizationUnit
Manager. It's a string something like:

"**00001.00042.00005**"

This code can be used to easily query database for all children
(recursively) of an OU. There are some rules for this code;

-   It's **unique** for a [tenant](/Pages/Documents/Multi-Tenancy).
-   All children of same OU have codes **start with parent OU's code**.
-   It's **fixed length** based on level of OU in the tree, as shown in
    the sample.
-   While OU code is unique, it can be **changable** if you move an OU.
    So, we should reference an OU by Id, not Code.

### OrganizationUnit Manager

**OrganizationUnitManager** class can be
[injected](/Pages/Documents/Dependency-Injection) and used to manage
OUs. Common use cases are:

-   Create, Update or Delete an OU
-   Move an OU in OU tree.
-   Getting information about OU tree and items.

#### Multi Tenancy

OrganizationUnitManager is designed to work for a **single tenant** in a
time. It works for the **current tenant** as default.

### Common Use Cases

Here, we will see common use cases for OUs. You can find source code of
the samples
[here](https://github.com/aspnetboilerplate/aspnetboilerplate-samples/tree/master/OrganizationUnitsDemo).

#### Creating Entity Belongs To An Organization Unit

Most obvious usage of OUs is to assign an entity to an OU. Let's see a
sample entity:

    public class Product : Entity, IMustHaveTenant, IMustHaveOrganizationUnit
    {
        public virtual int TenantId { get; set; }

        public virtual long OrganizationUnitId { get; set; }
        
        public virtual string Name { get; set; }

        public virtual float Price { get; set; }
    }

We simple created **OrganizationUnitId** property to assign this entity
to an OU. **IMustHaveOrganizationUnit** defines the OrganizationUnitId
property. We don't have to implement it, but it's suggested to provide
standardization. There is also an IMayHaveOrganizationId which has a
**nullable** OrganizationUnitId property.

Now, we can relate a Product to an OU and query products of a specific
OU.

**Notice that**; Product entity have a **TenantId** (which is a property
of IMustHaveTenant) to distinguish products of different tenants in a
multi-tenant application (see [multi tenancy
document](/Pages/Documents/Multi-Tenancy#data-filters)). If your
application is not multi-tenant, you don't need this interface and
property.

#### Get Entities In An Organization Unit

Getting Products of an OU is simple. Let's see this sample [domain
service](/Pages/Documents/Domain-Services):

    public class ProductManager : IDomainService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductManager(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> GetProductsInOu(long organizationUnitId)
        {
            return _productRepository.GetAllList(p => p.OrganizationUnitId == organizationUnitId);
        }
                    
    }

We can simply write a predicate against Product.OrganizationUnitId as
shown above.

#### Get Entities In An Organization Unit Including It's Child Organization Units

We may want to get Products of an organization unit including child
organization units. In this case, OU **Code** can help us:

    public class ProductManager : IDomainService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;

        public ProductManager(
            IRepository<Product> productRepository, 
            IRepository<OrganizationUnit, long> organizationUnitRepository)
        {
            _productRepository = productRepository;
            _organizationUnitRepository = organizationUnitRepository;
        }

        [UnitOfWork]
        public virtual List<Product> GetProductsInOuIncludingChildren(long organizationUnitId)
        {
            var code = _organizationUnitRepository.Get(organizationUnitId).Code;

            var query =
                from product in _productRepository.GetAll()
                join organizationUnit in _organizationUnitRepository.GetAll() on product.OrganizationUnitId equals organizationUnit.Id
                where organizationUnit.Code.StartsWith(code)
                select product;

            return query.ToList();
        }
    }

First, we got **code** of the given OU. Then we created a LINQ with a
**join** and **StartsWith(code)** condition (StartsWith creates a
**LIKE** query in SQL). Thus, we can hierarchically get products of an
OU.

#### Filter Entities For A User

We may want to get all products those are in OUs of a specific user.
Example code:

    public class ProductManager : IDomainService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly UserManager _userManager;

        public ProductManager(
            IRepository<Product> productRepository, 
            UserManager userManager)
        {
            _productRepository = productRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _userManager = userManager;
        }

        public async Task<List<Product>> GetProductsForUserAsync(long userId)
        {
            var user = await _userManager.GetUserByIdAsync(userId);
            var organizationUnits = await _userManager.GetOrganizationUnitsAsync(user);
            var organizationUnitIds = organizationUnits.Select(ou => ou.Id);

            return await _productRepository.GetAllListAsync(p => organizationUnitIds.Contains(p.OrganizationUnitId));
        }
    }

We simply found Ids of OUs of the user. Then used **Contains** condition
while getting products. Surely, we could create a LINQ query with join
to get the same list.

We may want to get products in user's OUs including their child OUs:

    public class ProductManager : IDomainService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly UserManager _userManager;

        public ProductManager(
            IRepository<Product> productRepository, 
            IRepository<OrganizationUnit, long> organizationUnitRepository, 
            UserManager userManager)
        {
            _productRepository = productRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _userManager = userManager;
        }

        [UnitOfWork]
        public virtual async Task<List<Product>> GetProductsForUserIncludingChildOusAsync(long userId)
        {
            var user = await _userManager.GetUserByIdAsync(userId);
            var organizationUnits = await _userManager.GetOrganizationUnitsAsync(user);
            var organizationUnitCodes = organizationUnits.Select(ou => ou.Code);

            var query =
                from product in _productRepository.GetAll()
                join organizationUnit in _organizationUnitRepository.GetAll() on product.OrganizationUnitId equals organizationUnit.Id
                where organizationUnitCodes.Any(code => organizationUnit.Code.StartsWith(code))
                select product;

            return query.ToList();
        }
    }

We combined **Any** with **StartsWith** condition in a LINQ join
statement.

Surely, there may be much more complex requirements, but all can be done
with LINQ or SQL.

### Settings

You can inject and use **IOrganizationUnitSettings** interface to get
Organization Units setting values. Currently, there is just a single
setting that can be changed for your application needs:

-   **MaxUserMembershipCount**: Maximum allowed membership count for a
    user.  
    Default value is **int.MaxValue** which allows a user to be member
    of unlimited OUs in same time.  
    Setting name is a constant defined in
    *AbpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount*.

You can change setting values using [setting
manager](/Pages/Documents/Setting-Management).
