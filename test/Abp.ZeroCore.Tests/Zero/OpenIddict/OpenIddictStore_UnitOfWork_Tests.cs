using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.OpenIddict.Applications;
using Abp.OpenIddict.Authorizations;
using Abp.OpenIddict.Tokens;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.Zero.OpenIddict;

// https://github.com/aspnetboilerplate/aspnetboilerplate/issues/7215
public class OpenIddictStore_UnitOfWork_Tests : AbpZeroTestBase
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly AbpOpenIddictAuthorizationStore _authorizationStore;
    private readonly AbpOpenIddictTokenStore _tokenStore;
    private readonly Guid _applicationId;

    public OpenIddictStore_UnitOfWork_Tests()
    {
        LocalIocManager.Register<AbpOpenIddictAuthorizationStore>(DependencyLifeStyle.Transient);
        LocalIocManager.Register<AbpOpenIddictTokenStore>(DependencyLifeStyle.Transient);

        _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        _authorizationStore = Resolve<AbpOpenIddictAuthorizationStore>();
        _tokenStore = Resolve<AbpOpenIddictTokenStore>();

        _applicationId = Guid.NewGuid();
        UsingDbContext(context =>
        {
            context.Applications.Add(new OpenIddictApplication(_applicationId)
            {
                ClientId = "test-client",
                DisplayName = "Test Client"
            });
        });
    }

    [Fact]
    public async Task Should_Set_Authorization_ApplicationId_Without_Ambient_UnitOfWork()
    {
        _unitOfWorkManager.Current.ShouldBeNull();

        var authorization = await _authorizationStore.InstantiateAsync(CancellationToken.None);

        await _authorizationStore.SetApplicationIdAsync(authorization, _applicationId.ToString(),
            CancellationToken.None);

        authorization.ApplicationId.ShouldBe(_applicationId);
    }

    [Fact]
    public async Task Should_Create_Ad_Hoc_Authorization_And_Token_Without_Ambient_UnitOfWork()
    {
        _unitOfWorkManager.Current.ShouldBeNull();

        var (authorizationId, tokenId) = await CreateAdHocAuthorizationAndTokenAsync();

        await ShouldBeSavedAsync(authorizationId, tokenId);
    }

    [Fact]
    public async Task Should_Create_Ad_Hoc_Authorization_And_Token_Within_Ambient_UnitOfWork()
    {
        // Simulates AbpUnitOfWorkMiddleware wrapping the OpenIddict sign-in handler
        Guid authorizationId, tokenId;

        using (var uow = _unitOfWorkManager.Begin())
        {
            (authorizationId, tokenId) = await CreateAdHocAuthorizationAndTokenAsync();
            await uow.CompleteAsync();
        }

        await ShouldBeSavedAsync(authorizationId, tokenId);
    }

    [Fact]
    public async Task Should_Rollback_Ad_Hoc_Authorization_And_Token_When_Ambient_UnitOfWork_Is_Not_Completed()
    {
        Guid authorizationId, tokenId;

        using (_unitOfWorkManager.Begin(new UnitOfWorkOptions { IsTransactional = true }))
        {
            (authorizationId, tokenId) = await CreateAdHocAuthorizationAndTokenAsync();
        }

        await UsingDbContextAsync(async context =>
        {
            (await context.Authorizations.AnyAsync(a => a.Id == authorizationId)).ShouldBeFalse();
            (await context.Tokens.AnyAsync(t => t.Id == tokenId)).ShouldBeFalse();
        });
    }

    [Fact]
    public async Task Should_Not_Create_Authorization_When_Cancellation_Is_Requested()
    {
        var authorization = await _authorizationStore.InstantiateAsync(CancellationToken.None);

        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _authorizationStore.CreateAsync(authorization, new CancellationToken(canceled: true)));

        await UsingDbContextAsync(async context =>
        {
            (await context.Authorizations.AnyAsync(a => a.Id == authorization.Id)).ShouldBeFalse();
        });
    }

    [Fact]
    public async Task Should_Not_Create_Token_When_Cancellation_Is_Requested()
    {
        var token = await _tokenStore.InstantiateAsync(CancellationToken.None);

        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _tokenStore.CreateAsync(token, new CancellationToken(canceled: true)));

        await UsingDbContextAsync(async context =>
        {
            (await context.Tokens.AnyAsync(t => t.Id == token.Id)).ShouldBeFalse();
        });
    }

    /// <summary>
    /// Mimics what OpenIddict does on connect/token with reference access tokens:
    /// it creates an ad-hoc authorization, then creates a token entry that references it.
    /// </summary>
    private async Task<(Guid authorizationId, Guid tokenId)> CreateAdHocAuthorizationAndTokenAsync()
    {
        // OpenIddictAuthorizationManager.CreateAsync
        var authorization = await _authorizationStore.InstantiateAsync(CancellationToken.None);
        await _authorizationStore.SetApplicationIdAsync(authorization, _applicationId.ToString(),
            CancellationToken.None);
        await _authorizationStore.SetSubjectAsync(authorization, "1", CancellationToken.None);
        await _authorizationStore.SetStatusAsync(authorization, "valid", CancellationToken.None);
        await _authorizationStore.SetTypeAsync(authorization, "ad-hoc", CancellationToken.None);
        await _authorizationStore.CreateAsync(authorization, CancellationToken.None);

        // OpenIddictTokenManager.CreateAsync
        var token = await _tokenStore.InstantiateAsync(CancellationToken.None);
        await _tokenStore.SetApplicationIdAsync(token, _applicationId.ToString(), CancellationToken.None);
        await _tokenStore.SetAuthorizationIdAsync(token, authorization.Id.ToString(), CancellationToken.None);
        await _tokenStore.SetSubjectAsync(token, "1", CancellationToken.None);
        await _tokenStore.SetStatusAsync(token, "valid", CancellationToken.None);
        await _tokenStore.SetTypeAsync(token, "access_token", CancellationToken.None);
        await _tokenStore.CreateAsync(token, CancellationToken.None);

        // OpenIddict attaches the payload and reference id to the token entry after creating it
        await _tokenStore.SetPayloadAsync(token, "payload", CancellationToken.None);
        await _tokenStore.SetReferenceIdAsync(token, "reference-id", CancellationToken.None);
        await _tokenStore.UpdateAsync(token, CancellationToken.None);

        token.AuthorizationId.ShouldBe(authorization.Id);

        return (authorization.Id, token.Id);
    }

    private async Task ShouldBeSavedAsync(Guid authorizationId, Guid tokenId)
    {
        await UsingDbContextAsync(async context =>
        {
            (await context.Authorizations.AnyAsync(a => a.Id == authorizationId)).ShouldBeTrue();
            (await context.Tokens.AnyAsync(t =>
                t.Id == tokenId &&
                t.AuthorizationId == authorizationId &&
                t.ReferenceId == "reference-id")).ShouldBeTrue();
        });
    }
}
