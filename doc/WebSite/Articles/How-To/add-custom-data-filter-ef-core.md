## Introduction

In this article, I will explain how to add a custom data filter in Entity Framework Core. 

We will create a filter for OrganizationUnit and filter entities inherited from `IMayHaveOrganizationUnit` interface automatically according to organization unit of logged in user.

We will use ASP.NET Core & JQuery template of [ASP.NET Boilerplate](https://aspnetboilerplate.com).
You can create a project on [https://aspnetboilerplate.com/Templates](https://aspnetboilerplate.com/Templates) and apply the steps below to see custom organization unit filter in action.

### Create and Update Entities

#### Create an entity

Create an entity named `Document` inherited from `IMayHaveOrganizationUnit` (`IMayHaveOrganizationUnit` interface is defined in ABP Framework).

````csharp
public class Document : Entity, IMayHaveOrganizationUnit
{
    public string Title { get; set; }

    public string Content { get; set; }

    public long? OrganizationUnitId { get; set; }
}
````

Add `Document` entity to your DbContext.

#### Update User class

Add `OrganizationUnitId` to `User.cs`. We will use `OrganizationUnitId` field of the User to filter `Document` entities.

````csharp
public class User : AbpUser<User>
{
    public const string DefaultPassword = "123qwe";

    public static string CreateRandomPassword()
    {
        return Guid.NewGuid().ToString("N").Truncate(16);
    }

    public int? OrganizationUnitId { get; set; }

    public static User CreateTenantAdminUser(int tenantId, string emailAddress)
    {
        var user = new User
        {
            TenantId = tenantId,
            UserName = AdminUserName,
            Name = AdminUserName,
            Surname = AdminUserName,
            EmailAddress = emailAddress
        };

        user.SetNormalizedNames();

        return user;
    }
}
````

#### Add migration

Add migration using `add-migration` command and run `update-database` to apply changes to your database.

### Create Claim

We need to store `OrganizationUnitId` of logged in user in claims, so we can get it in order to filter `IMayHaveOrganizationUnit` entities in our DbContext. In order to do that, override the `CreateAsync` method of `UserClaimsPrincipalFactory` class and add logged in users `OrganizationUnitId` to claims like below.

````csharp
public class UserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory<User, Role>
{
    public UserClaimsPrincipalFactory(
        UserManager userManager,
        RoleManager roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(
                userManager,
                roleManager,
                optionsAccessor)
    {
    }

    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var claim = await base.CreateAsync(user);
        claim.Identities.First().AddClaim(new Claim("Application_OrganizationUnitId", user.OrganizationUnitId.HasValue ? user.OrganizationUnitId.Value.ToString() : ""));

        return claim;
    }
}
````

### Register Filter

Before filtering entities in our DbContext, we will register our filter, so we can disable it if we want to for some cases in our code.

Register filter in PreInitialize method in `YourProjectNameEntityFrameworkModule` to get it from current unit of work manager.

````csharp
public override void PreInitialize()
{
    ...

    //register filter with default value
    Configuration.UnitOfWork.RegisterFilter("MayHaveOrganizationUnit", true);
}
````

### Configure DbContext

We need to use the value of `OrganizationUnitId` we have added to claims to filter `IMayHaveOrganizationUnit` entities in our DbContext.

In order to do that, first add a field like below to your DbContext:

````csharp
protected virtual int? CurrentOUId => GetCurrentUsersOuIdOrNull();
````

And define `GetCurrentUsersOuIdOrNull` method like below in your DbContext and also inject `IPrincipalAccessor` into your DbContext using propert injection;

````csharp
public class CustomFilterSampleDbContext : AbpZeroDbContext<Tenant, Role, User, CustomFilterSampleDbContext>
{
    public DbSet<Document> Documents { get; set; }

    public IPrincipalAccessor PrincipalAccessor { get; set; }

    protected virtual int? CurrentOUId => GetCurrentUsersOuIdOrNull();

    public CustomFilterSampleDbContext(DbContextOptions<CustomFilterSampleDbContext> options)
        : base(options)
    {
        
    }

    protected virtual int? GetCurrentUsersOuIdOrNull()
	{
		var userOuClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Application_OrganizationUnitId");
		if (string.IsNullOrEmpty(userOuClaim?.Value))
		{
			return null;
		}

		return Convert.ToInt32(userOuClaim.Value);
	}	
}
````

After that, let's add a property to our DbContext in order to get if the `MayHaveOrganizationUnit` filter is enabled or not.

````csharp
protected virtual bool IsOUFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled("MayHaveOrganizationUnit") == true;
````

AbpDbContext defines two methods related to data filters. One is `ShouldFilterEntity` and the other one is `CreateFilterExpression`. `ShouldFilterEntity` method decides to filter an entity or not. `CreateFilterExpression` method creates filter expressions for entities to filter. 

In order to filter entities inherited from `IMayHaveOrganizationUnit`, we need to override both methods.

First, override `ShouldFilterEntity` method like below;

````csharp
protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
{
    if (typeof(IMayHaveOrganizationUnit).IsAssignableFrom(typeof(TEntity)))
    {
        return true;
    }

    return base.ShouldFilterEntity<TEntity>(entityType);
}
````    

Then, override `CreateFilterExpression` method like below;

````csharp
protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
{
    var expression = base.CreateFilterExpression<TEntity>();

    if (typeof(IMayHaveOrganizationUnit).IsAssignableFrom(typeof(TEntity)))
    {
        Expression<Func<TEntity, bool>> mayHaveOUFilter = e => ((IMayHaveOrganizationUnit)e).OrganizationUnitId == CurrentOUId || (((IMayHaveOrganizationUnit)e).OrganizationUnitId == CurrentOUId) == IsOUFilterEnabled;
        expression = expression == null ? mayHaveOUFilter : CombineExpressions(expression, mayHaveOUFilter);
    }

    return expression;
}
````

Here is the final version of our DbContext:

````csharp
public class CustomFilterSampleDbContext : AbpZeroDbContext<Tenant, Role, User, CustomFilterSampleDbContext>
{
    public DbSet<Document> Documents { get; set; }
	
    public IPrincipalAccessor PrincipalAccessor { get; set; }
	
    protected virtual int? CurrentOUId => GetCurrentUsersOuIdOrNull();
	
    protected virtual bool IsOUFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled("MayHaveOrganizationUnit") == true;
	
    public CustomFilterSampleDbContext(DbContextOptions<CustomFilterSampleDbContext> options)
        : base(options)
    {
        
    }
	
    protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
    {
        if (typeof(IMayHaveOrganizationUnit).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }
         return base.ShouldFilterEntity<TEntity>(entityType);
    }
	
    protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
    {
        var expression = base.CreateFilterExpression<TEntity>();
        if (typeof(IMayHaveOrganizationUnit).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> mayHaveOUFilter = e => ((IMayHaveOrganizationUnit)e).OrganizationUnitId == CurrentOUId || (((IMayHaveOrganizationUnit)e).OrganizationUnitId == CurrentOUId) == IsOUFilterEnabled;
            expression = expression == null ? mayHaveOUFilter : CombineExpressions(expression, mayHaveOUFilter);
        }
		
        return expression;
    }
	
    protected virtual int? GetCurrentUsersOuIdOrNull()
    {
        var userOuClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Application_OrganizationUnitId");
        if (string.IsNullOrEmpty(userOuClaim?.Value))
        {
            return null;
        }
		
        return Convert.ToInt32(userOuClaim.Value);
    }
}
````

### Testing the Filter

In order to test the `MayHaveOrganizationUnit` filter, create an organization unit and set its `UserId = 2` (Id of the Default Tenant's admin user) and `TenantId = 1` (Id of the Default Tenant). Then, create document records on the database as well.
Set `OrganizationUnitId` of Default Tenant's admin user and document(s) you have created with id of the created organization unit.

Getting data from database in HomeController:

````csharp
[AbpMvcAuthorize]
public class HomeController : CustomFilterSampleControllerBase
{
    private readonly IRepository<Document> _documentRepository;

    public HomeController(IRepository<Document> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public ActionResult Index()
    {
        var documents = _documentRepository.GetAllList();
        var documentTitles = string.Join(",", documents.Select(e => e.Title).ToArray());

        return Content(documentTitles);
    }
}
````

When you log in as host user you should see an emtpy page. But if you log in as admin user of Default Tenant, you will see the document titles as below:

<img src="images/document-titles-output.png" alt="Document Titles" class="img-thumbnail" />

### Disable filter

You can disable filter like following:

````csharp
[AbpMvcAuthorize]
public class HomeController : CustomFilterSampleControllerBase
{
    private readonly IRepository<Document> _documentRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public HomeController(IRepository<Document> documentRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _documentRepository = documentRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public ActionResult Index()
    {
        using (_unitOfWorkManager.Current.DisableFilter("MayHaveOrganizationUnit"))
        {
            var documents = _documentRepository.GetAllList();
            var documentTitles = string.Join(",", documents.Select(e => e.Title).ToArray());

            return Content(documentTitles);
        }
    }
}
````

In this case, all document records will be retrieved from database regardless of the logged in users OrganizationUnitId.