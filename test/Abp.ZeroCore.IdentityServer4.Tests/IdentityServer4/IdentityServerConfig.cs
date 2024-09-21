using System.Collections.Generic;
using IdentityServer4.Models;

namespace Abp.IdentityServer4;

public static class IdentityServerConfig
{
    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
            {
                new ApiResource("default-api", "Default (all) API")
            };
    }

    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone()
            };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>
            {
                new Client
                {
                    ClientId = "test-client",
                    ClientName = "My Test Client",
                    AllowedGrantTypes = new List<string>{ "hybrid", "client_credentials", "password" },
                    ClientSecrets = new List<Secret> {new Secret("secret".Sha256()) },
                    AllowedScopes = new List<string> { "default-api" }
                }
            };
    }
}