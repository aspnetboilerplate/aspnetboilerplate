using System.Linq;
using Abp.ZeroCore.SampleApp.Core;
using Abp.ZeroCore.SampleApp.EntityFramework;

namespace Abp.Zero.Users;

public class UserLoginHelper
{
    public static void CreateTestUsers(SampleAppDbContext context)
    {
        var defaultTenant = context.Tenants.Single(t => t.TenancyName == Tenant.DefaultTenantName);

        CreateTestUsers(context,
            new User
            {
                TenantId = defaultTenant.Id, // A user of tenant1
                UserName = "userOwner",
                Name = "Owner",
                Surname = "One",
                EmailAddress = "owner@aspnetboilerplate.com",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            });


        CreateTestUsers(context,
            new User
            {
                TenantId = defaultTenant.Id, // A user of tenant1
                UserName = "user1",
                Name = "User",
                Surname = "One",
                EmailAddress = "user-one@aspnetboilerplate.com",
                IsEmailConfirmed = false,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            });
    }

    private static void CreateTestUsers(SampleAppDbContext context, User user)
    {
        user.SetNormalizedNames();

        context.Users.Add(user);
    }
}