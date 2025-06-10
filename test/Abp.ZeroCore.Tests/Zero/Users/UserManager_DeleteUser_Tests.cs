using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.ZeroCore.SampleApp.Core;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users;

public class UserManager_DeleteUser_Tests : AbpZeroTestBase
{
    [Fact]
    public async Task DeleteUserAsync_Test()
    {
        var user = new User
        {
            TenantId = AbpSession.TenantId,
            UserName = "user1",
            Name = "John",
            Surname = "Doe",
            EmailAddress = "user1@aspnetboilerplate.com",
            IsEmailConfirmed = true,
            Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
        };

        await WithUnitOfWorkAsync(async () =>
        {
            //Add user
            var userManager = LocalIocManager.Resolve<UserManager>();
            await userManager.CreateAsync(user);
            await userManager.AddToRoleAsync(user, "ADMIN");
            user.Roles.Count.ShouldBe(1);

            //Add user login
            var userLoginRepository = Resolve<IRepository<UserLogin, long>>();
            await userLoginRepository.InsertAsync(
                new UserLogin(
                    user.TenantId,
                    user.Id,
                    "TestLoginProvider",
                    "TestLoginProviderKey"
                )
            );
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var userManager = LocalIocManager.Resolve<UserManager>();
            var userLoginRepository = Resolve<IRepository<UserLogin, long>>();

            var isLoginInserted = await userLoginRepository.GetAll().AnyAsync(userLogin =>
                userLogin.UserId == user.Id &&
                userLogin.TenantId == user.TenantId
            );
            isLoginInserted.ShouldBeTrue();

            //delete user
            await userManager.DeleteAsync(user);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var userLoginRepository = Resolve<IRepository<UserLogin, long>>();

            //user login should be deleted
            var isUserLoginExists = await userLoginRepository.GetAll().AnyAsync(userLogin =>
                userLogin.UserId == user.Id &&
                userLogin.TenantId == user.TenantId
            );
            isUserLoginExists.ShouldBeFalse();
        });
    }
}