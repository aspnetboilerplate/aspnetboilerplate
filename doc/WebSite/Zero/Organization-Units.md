### Introduction

Organization units (OU) can be used to **hierarchically group users and
entities**.

### OrganizationUnit Entity

An OU is represented by the **OrganizationUnit** entity. The fundamental
properties of this entity are:

-   **TenantId**: Tenant's Id of this OU. Can be null for host OUs.
-   **ParentId**: Parent OU's Id. Can be null if this is a root OU.
-   **Code**: A hierarchical string code that is unique for a tenant.
-   **DisplayName**: Shown name of the OU.

The OrganizationUnit entity's primary key (Id) is a **long** type and it derives
from the [**FullAuditedEntity**](/Pages/Documents/Entities#auditing) class
which provides audit information and implements the
[**ISoftDelete**](/Pages/Documents/Data-Filters#isoftdelete) interface
(OUs are not deleted from the database, they are just marked as deleted).

#### Organization Tree

Since an OU can have a parent, all OUs of a tenant are in a **tree**
structure. There are some rules for this tree;

-   There can be more than one root (where the ParentId is null).
-   Maximum depth of the tree is defined as a constant as
    OrganizationUnit.**MaxDepth**. The value is **16**.
-   There is a limit for the first-level children count of an OU (because of the
    fixed OU Code unit length explained below).

#### OU Code

OU code is automatically generated and maintained by the OrganizationUnit
Manager. It's a string that looks something like this:

"**00001.00042.00005**"

This code can be used to easily query the database for all the children
of an OU (recursively). There are some rules for this code:

-   It must be **unique** for a [tenant](/Pages/Documents/Multi-Tenancy).
-   All the children of the same OU have codes that **start with the parent OU's code**.
-   It's **fixed length** and based on the level of the OU in the tree, as shown in
    the sample.
-   While the OU code is unique, it can be **changeable** if you move an OU.
-   We must reference an OU by Id, not Code.

### OrganizationUnit Manager

The **OrganizationUnitManager** class can be
[injected](/Pages/Documents/Dependency-Injection) and used to manage
OUs. Common use cases are:

-   Create, Update or Delete an OU
-   Move an OU in the OU tree.
-   Getting information about the OU tree and its items.

#### Multi-Tenancy

The OrganizationUnitManager is designed to work for a **single tenant** at a
time. It works for the **current tenant** by default.

### Common Use Cases

Here, we will see some common use cases for OUs. You can find the source code of
the samples
[here](https://github.com/aspnetboilerplate/aspnetboilerplate-samples/tree/master/OrganizationUnitsDemo).

#### Creating An Entity That Belongs To An Organization Unit

The most obvious usage of OUs is to assign an entity to an OU. Let's see a
sample entity:

    public class Product : Entity, IMustHaveTenant, IMustHaveOrganizationUnit
    {
        public virtual int TenantId { get; set; }

        public virtual long OrganizationUnitId { get; set; }
        
        public virtual string Name { get; set; }

        public virtual float Price { get; set; }
    }

We simply created the **OrganizationUnitId** property to assign this entity
to an OU. The **IMustHaveOrganizationUnit** defines the OrganizationUnitId
property. We don't have to implement it, but it's recommended because it provides
standardization. There is also the IMayHaveOrganizationId interface which has a
**nullable** OrganizationUnitId property.

We can now relate a Product to an OU and query the products of a specific
OU.

**Please note**; The product entity must have a **TenantId** (which is a property
of IMustHaveTenant) to distinguish it from products of different tenants in a
multi-tenant application (see the [Multi-Tenancy
document](/Pages/Documents/Multi-Tenancy#data-filters) for more info). If your
application is not multi-tenant, you don't need this interface and
property.

#### Getting Entities In An Organization Unit

Getting the Products of an OU is simple. Let's see this sample [domain
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

As shown above, we can simply write a predicate against Product.OrganizationUnitId.

#### Get Entities In An Organization Unit Including It's Child Organization Units

We may want to get the Products of an organization unit including child
organization units. In this case, the OU **Code** can help us:

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

First, we got the **code** of the the given OU. Then we created a LINQ expression with a
**join** and a **StartsWith(code)** condition (StartsWith creates a
**LIKE** query in SQL). This way we can hierarchically get the products of an
OU.

#### Filter Entities For A User

We may want to get all products that are in the OUs of a specific user.
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

We simply found the Ids of the OUs of the user. We then used a **Contains** condition
while getting the products. We could also create a LINQ query with join
to get the same list, instead.

We may want to get products in the user's OUs including their child OUs:

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

We combined **Any** with the **StartsWith** condition into a LINQ join
statement.

There will most likely be more complex requirements, but they all can be done
with LINQ or SQL.

### Settings

You can inject and use the **IOrganizationUnitSettings** interface to get
the Organization Unit's setting values. There currently is just a single
setting that can be changed for your application needs:

-   **MaxUserMembershipCount**: Maximum allowed membership count for a
    user.  
    Default value is **int.MaxValue** which allows a user to be a member
    of unlimited OUs at the same time.  
    The Setting name is a constant defined in
    *AbpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount*.

You can change the setting values using the [setting
manager](/Pages/Documents/Setting-Management).
