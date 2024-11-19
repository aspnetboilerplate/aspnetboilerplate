using Abp.Collections.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Abp.OpenIddict.Applications;

public static class OpenIddictApplicationExtensions
{
    public static OpenIddictApplication ToEntity(this OpenIddictApplicationModel model)
    {
        Check.NotNull(model, nameof(model));

        var entity = new OpenIddictApplication(model.Id)
        {
            ApplicationType = model.ApplicationType,
            ClientId = model.ClientId,
            ClientSecret = model.ClientSecret,
            ClientType = model.ClientType,
            ConsentType = model.ConsentType,
            DisplayName = model.DisplayName,
            DisplayNames = model.DisplayNames,
            JsonWebKeySet = model.JsonWebKeySet != null ? JsonSerializer.Serialize(model.JsonWebKeySet) : null,
            Permissions = model.Permissions,
            PostLogoutRedirectUris = model.PostLogoutRedirectUris,
            Properties = model.Properties,
            RedirectUris = model.RedirectUris,
            Requirements = model.Requirements,
            Settings = model.Settings,
            ClientUri = model.ClientUri,
            LogoUri = model.LogoUri
        };

        return entity;
    }

    public static OpenIddictApplication ToEntity(this OpenIddictApplicationModel model, OpenIddictApplication entity)
    {
        Check.NotNull(model, nameof(model));
        Check.NotNull(entity, nameof(entity));

        entity.ApplicationType = model.ApplicationType;
        entity.ClientId = model.ClientId;
        entity.ClientSecret = model.ClientSecret;
        entity.ConsentType = model.ConsentType;
        entity.ClientType = model.ClientType;
        entity.DisplayName = model.DisplayName;
        entity.DisplayNames = model.DisplayNames;
        entity.JsonWebKeySet = model.JsonWebKeySet != null ? JsonSerializer.Serialize(model.JsonWebKeySet) : null;
        entity.Permissions = model.Permissions;
        entity.PostLogoutRedirectUris = model.PostLogoutRedirectUris;
        entity.Properties = model.Properties;
        entity.RedirectUris = model.RedirectUris;
        entity.Requirements = model.Requirements;
        entity.Settings = model.Settings;
        entity.ClientUri = model.ClientUri;
        entity.LogoUri = model.LogoUri;

        return entity;
    }

    /// <summary>
    /// parsing the stringified JSON Web Key Set is an expensive operation, To mitigate that, the resulting object is stored in the static cache.
    /// </summary>
    private readonly static ConcurrentDictionary<string, JsonWebKeySet> JsonWebKeySetCache = new ConcurrentDictionary<string, JsonWebKeySet>();

    public static OpenIddictApplicationModel ToModel(this OpenIddictApplication entity)
    {
        if (entity == null)
        {
            return null;
        }

        var model = new OpenIddictApplicationModel
        {
            Id = entity.Id,
            ApplicationType = entity.ApplicationType,
            ClientId = entity.ClientId,
            ClientSecret = entity.ClientSecret,
            ClientType = entity.ClientType,
            ConsentType = entity.ConsentType,
            DisplayName = entity.DisplayName,
            DisplayNames = entity.DisplayNames,
            JsonWebKeySet = entity.JsonWebKeySet != null ? JsonWebKeySetCache.GetOrAdd(entity.JsonWebKeySet, () => JsonWebKeySet.Create(entity.JsonWebKeySet)) : null,
            Permissions = entity.Permissions,
            PostLogoutRedirectUris = entity.PostLogoutRedirectUris,
            Properties = entity.Properties,
            RedirectUris = entity.RedirectUris,
            Requirements = entity.Requirements,
            Settings = entity.Settings,
            ClientUri = entity.ClientUri,
            LogoUri = entity.LogoUri
        };

        return model;
    }
}