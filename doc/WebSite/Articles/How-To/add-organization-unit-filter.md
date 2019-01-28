## Introduction

In this article, i will explain how to add organization unit filterto DbContext.

We will use ASP.NET Core & JQuery version of [ASP.NET Boilerplate](https://aspnetboilerplate.com).

### Create and Update Entities

#### Create an entity

Create an entity that named `Document` that inherited from `IMayHaveOrganizationUnit`.

````csharp
public class Document : Entity, IMayHaveOrganizationUnit
{
    public string Title { get; set; }

    public string Content { get; set; }

    public long? OrganizationUnitId { get; set; }
}
````

And this entity to DbContext, as well.

#### Update User class

Add `OrganizationUnitId` to `User.cs`.

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

Add migration and run `update-database` to apply changes to database.

### Create Claim for Organization Unit ID of User

We are storing organization unit id of user in session, and getting it from logged in user session to filter by it.

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

    //Override CreateAsync method to add your custom claim
    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var claim = await base.CreateAsync(user);
        claim.Identities.First().AddClaim(new Claim("Application_OrganizationUnitId", user.OrganizationUnitId.HasValue ? user.OrganizationUnitId.Value.ToString() : ""));

        return claim;
    }
}
````

### Configure DbContext

Add a field that is store organization unit of current user:

    protected virtual int? CurrentOUId => GetCurrentUsersOuIdOrNull();

This field filling from `PrincipalAccessor`.

You can enable or disable organization unit filter:

    //protected virtual bool IsOUFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled("MustHaveOU") == true;
        protected bool IsOUFilterEnabled = true;

Here is the latest DbContext look:

````csharp
public class CustomFilterSampleDbContext : AbpZeroDbContext<Tenant, Role, User, CustomFilterSampleDbContext>
{
    public DbSet<Document> Documents { get; set; }

    public IPrincipalAccessor PrincipalAccessor { get; set; }

    protected virtual int? CurrentOUId => GetCurrentUsersOuIdOrNull();

    //protected virtual bool IsOUFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled("MustHaveOU") == true;
    protected bool IsOUFilterEnabled = true;

    public CustomFilterSampleDbContext(DbContextOptions<CustomFilterSampleDbContext> options)
        : base(options)
    {
        //MyAppSession = NullAbpSession.Instance;
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
                /* This condition should normally be defined as below:
                 * !IsSoftDeleteFilterEnabled || !((ISoftDelete) e).IsDeleted
                 * But this causes a problem with EF Core (see https://github.com/aspnet/EntityFrameworkCore/issues/9502)
                 * So, we made a workaround to make it working. It works same as above.
                 */

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

### Filtering Test

Create an organization unit and set its `UserId = 2` and `TenantId = 1`. Set `OrganizationUnitId` of user and document with created one's id.

Getting data from db in HomeController:

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

When you log in as host user you should not see anything. But if you log in as tenant user, you will see the document titles:

<img src="images/document-titles-output.png" alt="Document Titles" class="img-thumbnail" />