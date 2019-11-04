using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;

namespace Abp.Configuration
{
    /// <summary>
    /// This class implements <see cref="ISettingManager"/> to manage setting values in the database.
    /// </summary>
    public class SettingManager : ISettingManager, ISingletonDependency
    {
        public const string ApplicationSettingsCacheKey = "ApplicationSettings";

        /// <summary>
        /// Reference to the current Session.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Reference to the setting store.
        /// </summary>
        public ISettingStore SettingStore { get; set; }

        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly ITypedCache<string, Dictionary<string, SettingInfo>> _applicationSettingCache;
        private readonly ITypedCache<int, Dictionary<string, SettingInfo>> _tenantSettingCache;
        private readonly ITypedCache<string, Dictionary<string, SettingInfo>> _userSettingCache;
        private readonly ITenantStore _tenantStore;

        /// <inheritdoc/>
        public SettingManager(
            ISettingDefinitionManager settingDefinitionManager,
            ICacheManager cacheManager,
            IMultiTenancyConfig multiTenancyConfig, ITenantStore tenantStore)
        {
            _settingDefinitionManager = settingDefinitionManager;
            _multiTenancyConfig = multiTenancyConfig;
            _tenantStore = tenantStore;

            AbpSession = NullAbpSession.Instance;
            SettingStore = DefaultConfigSettingStore.Instance;

            _applicationSettingCache = cacheManager.GetApplicationSettingsCache();
            _tenantSettingCache = cacheManager.GetTenantSettingsCache();
            _userSettingCache = cacheManager.GetUserSettingsCache();
        }

        #region Public methods

        /// <inheritdoc/>
        public Task<string> GetSettingValueAsync(string name)
        {
            return GetSettingValueInternalAsync(name, AbpSession.TenantId, AbpSession.UserId);
        }
        /// <inheritdoc/>
        public string GetSettingValue(string name)
        {
            return GetSettingValueInternal(name, AbpSession.TenantId, AbpSession.UserId);
        }

        public Task<string> GetSettingValueForApplicationAsync(string name)
        {
            return GetSettingValueInternalAsync(name);
        }

        public string GetSettingValueForApplication(string name)
        {
            return GetSettingValueInternal(name);
        }

        public Task<string> GetSettingValueForApplicationAsync(string name, bool fallbackToDefault)
        {
            return GetSettingValueInternalAsync(name, fallbackToDefault: fallbackToDefault);
        }

        public string GetSettingValueForApplication(string name, bool fallbackToDefault)
        {
            return GetSettingValueInternal(name, fallbackToDefault: fallbackToDefault);
        }

        public Task<string> GetSettingValueForTenantAsync(string name, int tenantId)
        {
            return GetSettingValueInternalAsync(name, tenantId);
        }

        public string GetSettingValueForTenant(string name, int tenantId)
        {
            return GetSettingValueInternal(name, tenantId);
        }

        public Task<string> GetSettingValueForTenantAsync(string name, int tenantId, bool fallbackToDefault)
        {
            return GetSettingValueInternalAsync(name, tenantId, fallbackToDefault: fallbackToDefault);
        }

        public string GetSettingValueForTenant(string name, int tenantId, bool fallbackToDefault)
        {
            return GetSettingValueInternal(name, tenantId, fallbackToDefault: fallbackToDefault);
        }

        public Task<string> GetSettingValueForUserAsync(string name, int? tenantId, long userId)
        {
            return GetSettingValueInternalAsync(name, tenantId, userId);
        }

        public string GetSettingValueForUser(string name, int? tenantId, long userId)
        {
            return GetSettingValueInternal(name, tenantId, userId);
        }

        public Task<string> GetSettingValueForUserAsync(string name, int? tenantId, long userId, bool fallbackToDefault)
        {
            return GetSettingValueInternalAsync(name, tenantId, userId, fallbackToDefault);
        }

        public string GetSettingValueForUser(string name, int? tenantId, long userId, bool fallbackToDefault)
        {
            return GetSettingValueInternal(name, tenantId, userId, fallbackToDefault);
        }

        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync()
        {
            return await GetAllSettingValuesAsync(SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User);
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValues()
        {
            return GetAllSettingValues(SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync(SettingScopes scopes)
        {
            var settingDefinitions = new Dictionary<string, SettingDefinition>();
            var settingValues = new Dictionary<string, ISettingValue>();

            //Fill all setting with default values.
            foreach (var setting in _settingDefinitionManager.GetAllSettingDefinitions())
            {
                settingDefinitions[setting.Name] = setting;
                settingValues[setting.Name] = new SettingValueObject(setting.Name, setting.DefaultValue);
            }

            //Overwrite application settings
            if (scopes.HasFlag(SettingScopes.Application))
            {
                foreach (var settingValue in await GetAllSettingValuesForApplicationAsync())
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Application))
                    {
                        continue;
                    }

                    if (!setting.IsInherited &&
                        ((setting.Scopes.HasFlag(SettingScopes.Tenant) && AbpSession.TenantId.HasValue) || (setting.Scopes.HasFlag(SettingScopes.User) && AbpSession.UserId.HasValue)))
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite tenant settings
            if (scopes.HasFlag(SettingScopes.Tenant) && AbpSession.TenantId.HasValue)
            {
                foreach (var settingValue in await GetAllSettingValuesForTenantAsync(AbpSession.TenantId.Value))
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Tenant))
                    {
                        continue;
                    }

                    if (!setting.IsInherited &&
                        (setting.Scopes.HasFlag(SettingScopes.User) && AbpSession.UserId.HasValue))
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite user settings
            if (scopes.HasFlag(SettingScopes.User) && AbpSession.UserId.HasValue)
            {
                foreach (var settingValue in await GetAllSettingValuesForUserAsync(AbpSession.ToUserIdentifier()))
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);
                    if (setting != null && setting.Scopes.HasFlag(SettingScopes.User))
                    {
                        settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                    }
                }
            }

            return settingValues.Values.ToImmutableList();
        }

        /// <inheritdoc/>
        public IReadOnlyList<ISettingValue> GetAllSettingValues(SettingScopes scopes)
        {
            var settingDefinitions = new Dictionary<string, SettingDefinition>();
            var settingValues = new Dictionary<string, ISettingValue>();

            //Fill all setting with default values.
            foreach (var setting in _settingDefinitionManager.GetAllSettingDefinitions())
            {
                settingDefinitions[setting.Name] = setting;
                settingValues[setting.Name] = new SettingValueObject(setting.Name, setting.DefaultValue);
            }

            //Overwrite application settings
            if (scopes.HasFlag(SettingScopes.Application))
            {
                foreach (var settingValue in GetAllSettingValuesForApplication())
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Application))
                    {
                        continue;
                    }

                    if (!setting.IsInherited &&
                        ((setting.Scopes.HasFlag(SettingScopes.Tenant) && AbpSession.TenantId.HasValue) || (setting.Scopes.HasFlag(SettingScopes.User) && AbpSession.UserId.HasValue)))
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite tenant settings
            if (scopes.HasFlag(SettingScopes.Tenant) && AbpSession.TenantId.HasValue)
            {
                foreach (var settingValue in GetAllSettingValuesForTenant(AbpSession.TenantId.Value))
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Tenant))
                    {
                        continue;
                    }

                    if (!setting.IsInherited &&
                        (setting.Scopes.HasFlag(SettingScopes.User) && AbpSession.UserId.HasValue))
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite user settings
            if (scopes.HasFlag(SettingScopes.User) && AbpSession.UserId.HasValue)
            {
                foreach (var settingValue in GetAllSettingValuesForUser(AbpSession.ToUserIdentifier()))
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);
                    if (setting != null && setting.Scopes.HasFlag(SettingScopes.User))
                    {
                        settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                    }
                }
            }

            return settingValues.Values.ToImmutableList();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForApplicationAsync()
        {
            if (!_multiTenancyConfig.IsEnabled)
            {
                return (await GetReadOnlyTenantSettingsAsync(AbpSession.GetTenantId())).Values
                    .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                    .ToImmutableList();
            }

            return (await GetApplicationSettingsAsync()).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc/>
        public IReadOnlyList<ISettingValue> GetAllSettingValuesForApplication()
        {
            if (!_multiTenancyConfig.IsEnabled)
            {
                return (GetReadOnlyTenantSettings(AbpSession.GetTenantId())).Values
                    .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                    .ToImmutableList();
            }

            return (GetApplicationSettings()).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForTenantAsync(int tenantId)
        {
            return (await GetReadOnlyTenantSettingsAsync(tenantId)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc/>
        public IReadOnlyList<ISettingValue> GetAllSettingValuesForTenant(int tenantId)
        {
            return (GetReadOnlyTenantSettings(tenantId)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForUserAsync(long userId)
        {
            return GetAllSettingValuesForUserAsync(new UserIdentifier(AbpSession.TenantId, userId));
        }

        /// <inheritdoc/>
        public IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(long userId)
        {
            return GetAllSettingValuesForUser(new UserIdentifier(AbpSession.TenantId, userId));
        }

        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForUserAsync(UserIdentifier user)
        {
            return (await GetReadOnlyUserSettingsAsync(user)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(UserIdentifier user)
        {
            return (GetReadOnlyUserSettings(user)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual async Task ChangeSettingForApplicationAsync(string name, string value)
        {
            if (_multiTenancyConfig.IsEnabled)
            {
                await InsertOrUpdateOrDeleteSettingValueAsync(name, value, null, null);
            }
            else
            {
                // If MultiTenancy is disabled, then we should change default tenant's setting
                await InsertOrUpdateOrDeleteSettingValueAsync(name, value, AbpSession.GetTenantId(), null);
                await _tenantSettingCache.RemoveAsync(AbpSession.GetTenantId());
            }

            await _applicationSettingCache.RemoveAsync(ApplicationSettingsCacheKey);
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual void ChangeSettingForApplication(string name, string value)
        {
            if (_multiTenancyConfig.IsEnabled)
            {
                InsertOrUpdateOrDeleteSettingValue(name, value, null, null);
            }
            else
            {
                // If MultiTenancy is disabled, then we should change default tenant's setting
                InsertOrUpdateOrDeleteSettingValue(name, value, AbpSession.GetTenantId(), null);
                _tenantSettingCache.Remove(AbpSession.GetTenantId());
            }

            _applicationSettingCache.Remove(ApplicationSettingsCacheKey);
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual async Task ChangeSettingForTenantAsync(int tenantId, string name, string value)
        {
            await InsertOrUpdateOrDeleteSettingValueAsync(name, value, tenantId, null);
            await _tenantSettingCache.RemoveAsync(tenantId);
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual void ChangeSettingForTenant(int tenantId, string name, string value)
        {
            InsertOrUpdateOrDeleteSettingValue(name, value, tenantId, null);
            _tenantSettingCache.Remove(tenantId);
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual Task ChangeSettingForUserAsync(long userId, string name, string value)
        {
            return ChangeSettingForUserAsync(new UserIdentifier(AbpSession.TenantId, userId), name, value);
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual void ChangeSettingForUser(long userId, string name, string value)
        {
            ChangeSettingForUser(new UserIdentifier(AbpSession.TenantId, userId), name, value);
        }

        public async Task ChangeSettingForUserAsync(UserIdentifier user, string name, string value)
        {
            await InsertOrUpdateOrDeleteSettingValueAsync(name, value, user.TenantId, user.UserId);
            await _userSettingCache.RemoveAsync(user.ToUserIdentifierString());
        }

        public void ChangeSettingForUser(UserIdentifier user, string name, string value)
        {
            InsertOrUpdateOrDeleteSettingValue(name, value, user.TenantId, user.UserId);
            _userSettingCache.Remove(user.ToUserIdentifierString());
        }

        #endregion

        #region Private methods

        private async Task<string> GetSettingValueInternalAsync(string name, int? tenantId = null, long? userId = null, bool fallbackToDefault = true)
        {
            var settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);

            //Get for user if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.User) && userId.HasValue)
            {
                var settingValue = await GetSettingValueForUserOrNullAsync(new UserIdentifier(tenantId, userId.Value), name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for tenant if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Tenant) && tenantId.HasValue)
            {
                var settingValue = await GetSettingValueForTenantOrNullAsync(tenantId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for application if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Application))
            {
                var settingValue = await GetSettingValueForApplicationOrNullAsync(name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }
            }

            //Not defined, get default value
            return settingDefinition.DefaultValue;
        }

        private string GetSettingValueInternal(string name, int? tenantId = null, long? userId = null, bool fallbackToDefault = true)
        {
            var settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);

            //Get for user if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.User) && userId.HasValue)
            {
                var settingValue = GetSettingValueForUserOrNull(new UserIdentifier(tenantId, userId.Value), name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for tenant if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Tenant) && tenantId.HasValue)
            {
                var settingValue = GetSettingValueForTenantOrNull(tenantId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for application if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Application))
            {
                var settingValue = GetSettingValueForApplicationOrNull(name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }
            }

            //Not defined, get default value
            return settingDefinition.DefaultValue;
        }

        private async Task<SettingInfo> InsertOrUpdateOrDeleteSettingValueAsync(string name, string value, int? tenantId, long? userId)
        {
            var settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);
            var settingValue = await SettingStore.GetSettingOrNullAsync(tenantId, userId, name);

            //Determine defaultValue
            var defaultValue = settingDefinition.DefaultValue;

            if (settingDefinition.IsInherited)
            {
                //For Tenant and User, Application's value overrides Setting Definition's default value when multi tenancy is enabled.
                if (_multiTenancyConfig.IsEnabled && (tenantId.HasValue || userId.HasValue))
                {
                    var applicationValue = await GetSettingValueForApplicationOrNullAsync(name);
                    if (applicationValue != null)
                    {
                        defaultValue = applicationValue.Value;
                    }
                }

                //For User, Tenants's value overrides Application's default value.
                if (userId.HasValue && tenantId.HasValue)
                {
                    var tenantValue = await GetSettingValueForTenantOrNullAsync(tenantId.Value, name);
                    if (tenantValue != null)
                    {
                        defaultValue = tenantValue.Value;
                    }
                }
            }

            //No need to store on database if the value is the default value
            if (value == defaultValue)
            {
                if (settingValue != null)
                {
                    await SettingStore.DeleteAsync(settingValue);
                }

                return null;
            }

            //If it's not default value and not stored on database, then insert it
            if (settingValue == null)
            {
                settingValue = new SettingInfo
                {
                    TenantId = tenantId,
                    UserId = userId,
                    Name = name,
                    Value = value
                };

                await SettingStore.CreateAsync(settingValue);
                return settingValue;
            }

            //It's same value in database, no need to update
            if (settingValue.Value == value)
            {
                return settingValue;
            }

            //Update the setting on database.
            settingValue.Value = value;
            await SettingStore.UpdateAsync(settingValue);

            return settingValue;
        }

        private SettingInfo InsertOrUpdateOrDeleteSettingValue(string name, string value, int? tenantId, long? userId)
        {
            var settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);
            var settingValue = SettingStore.GetSettingOrNull(tenantId, userId, name);

            //Determine defaultValue
            var defaultValue = settingDefinition.DefaultValue;

            if (settingDefinition.IsInherited)
            {
                //For Tenant and User, Application's value overrides Setting Definition's default value when multi tenancy is enabled.
                if (_multiTenancyConfig.IsEnabled && (tenantId.HasValue || userId.HasValue))
                {
                    var applicationValue = GetSettingValueForApplicationOrNull(name);
                    if (applicationValue != null)
                    {
                        defaultValue = applicationValue.Value;
                    }
                }

                //For User, Tenants's value overrides Application's default value.
                if (userId.HasValue && tenantId.HasValue)
                {
                    var tenantValue = GetSettingValueForTenantOrNull(tenantId.Value, name);
                    if (tenantValue != null)
                    {
                        defaultValue = tenantValue.Value;
                    }
                }
            }

            //No need to store on database if the value is the default value
            if (value == defaultValue)
            {
                if (settingValue != null)
                {
                    SettingStore.Delete(settingValue);
                }

                return null;
            }

            //If it's not default value and not stored on database, then insert it
            if (settingValue == null)
            {
                settingValue = new SettingInfo
                {
                    TenantId = tenantId,
                    UserId = userId,
                    Name = name,
                    Value = value
                };

                SettingStore.Create(settingValue);
                return settingValue;
            }

            //It's same value in database, no need to update
            if (settingValue.Value == value)
            {
                return settingValue;
            }

            //Update the setting on database.
            settingValue.Value = value;
            SettingStore.Update(settingValue);

            return settingValue;
        }

        private async Task<SettingInfo> GetSettingValueForApplicationOrNullAsync(string name)
        {
            if (_multiTenancyConfig.IsEnabled)
            {
                return (await GetApplicationSettingsAsync()).GetOrDefault(name);
            }

            return (await GetReadOnlyTenantSettingsAsync(AbpSession.GetTenantId())).GetOrDefault(name);
        }

        private SettingInfo GetSettingValueForApplicationOrNull(string name)
        {
            if (_multiTenancyConfig.IsEnabled)
            {
                return (GetApplicationSettings()).GetOrDefault(name);
            }

            return (GetReadOnlyTenantSettings(AbpSession.GetTenantId())).GetOrDefault(name);
        }

        private async Task<SettingInfo> GetSettingValueForTenantOrNullAsync(int tenantId, string name)
        {
            return (await GetReadOnlyTenantSettingsAsync(tenantId)).GetOrDefault(name);
        }

        private SettingInfo GetSettingValueForTenantOrNull(int tenantId, string name)
        {
            return (GetReadOnlyTenantSettings(tenantId)).GetOrDefault(name);
        }

        private async Task<SettingInfo> GetSettingValueForUserOrNullAsync(UserIdentifier user, string name)
        {
            return (await GetReadOnlyUserSettingsAsync(user)).GetOrDefault(name);
        }

        private SettingInfo GetSettingValueForUserOrNull(UserIdentifier user, string name)
        {
            return (GetReadOnlyUserSettings(user)).GetOrDefault(name);
        }

        private async Task<Dictionary<string, SettingInfo>> GetApplicationSettingsAsync()
        {
            return await _applicationSettingCache.GetAsync(ApplicationSettingsCacheKey, async () =>
            {
                var dictionary = new Dictionary<string, SettingInfo>();

                var settingValues = await SettingStore.GetAllListAsync(null, null);
                foreach (var settingValue in settingValues)
                {
                    dictionary[settingValue.Name] = settingValue;
                }

                return dictionary;
            });
        }

        private Dictionary<string, SettingInfo> GetApplicationSettings()
        {
            return _applicationSettingCache.Get(ApplicationSettingsCacheKey, () =>
            {
                var dictionary = new Dictionary<string, SettingInfo>();

                var settingValues = SettingStore.GetAllList(null, null);
                foreach (var settingValue in settingValues)
                {
                    dictionary[settingValue.Name] = settingValue;
                }

                return dictionary;
            });
        }

        private async Task<ImmutableDictionary<string, SettingInfo>> GetReadOnlyTenantSettingsAsync(int tenantId)
        {
            var cachedDictionary = await GetTenantSettingsFromCacheAsync(tenantId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private ImmutableDictionary<string, SettingInfo> GetReadOnlyTenantSettings(int tenantId)
        {
            var cachedDictionary = GetTenantSettingsFromCache(tenantId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private async Task<ImmutableDictionary<string, SettingInfo>> GetReadOnlyUserSettingsAsync(UserIdentifier user)
        {
            var cachedDictionary = await GetUserSettingsFromCacheAsync(user);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private ImmutableDictionary<string, SettingInfo> GetReadOnlyUserSettings(UserIdentifier user)
        {
            var cachedDictionary = GetUserSettingsFromCache(user);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private async Task<Dictionary<string, SettingInfo>> GetTenantSettingsFromCacheAsync(int tenantId)
        {
            return await _tenantSettingCache.GetAsync(
                tenantId,
                async () =>
                {
                    var dictionary = new Dictionary<string, SettingInfo>();

                    if (!_multiTenancyConfig.IsEnabled && _tenantStore.Find(tenantId) == null)
                    {
                        return dictionary;
                    }

                    var settingValues = await SettingStore.GetAllListAsync(tenantId, null);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
        }

        private Dictionary<string, SettingInfo> GetTenantSettingsFromCache(int tenantId)
        {
            return _tenantSettingCache.Get(
                tenantId,
                () =>
                {
                    var dictionary = new Dictionary<string, SettingInfo>();

                    if (!_multiTenancyConfig.IsEnabled && _tenantStore.Find(tenantId) == null)
                    {
                        return dictionary;
                    }

                    var settingValues = SettingStore.GetAllList(tenantId, null);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
        }

        private async Task<Dictionary<string, SettingInfo>> GetUserSettingsFromCacheAsync(UserIdentifier user)
        {
            return await _userSettingCache.GetAsync(
                user.ToUserIdentifierString(),
                async () =>
                {
                    var dictionary = new Dictionary<string, SettingInfo>();

                    var settingValues = await SettingStore.GetAllListAsync(user.TenantId, user.UserId);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
        }

        private Dictionary<string, SettingInfo> GetUserSettingsFromCache(UserIdentifier user)
        {
            return _userSettingCache.Get(
                user.ToUserIdentifierString(),
                () =>
                {
                    var dictionary = new Dictionary<string, SettingInfo>();

                    var settingValues = SettingStore.GetAllList(user.TenantId, user.UserId);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
        }

        public Task<string> GetSettingValueForUserAsync(string name, UserIdentifier user)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(user, nameof(user));

            return GetSettingValueForUserAsync(name, user.TenantId, user.UserId);
        }

        public string GetSettingValueForUser(string name, UserIdentifier user)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(user, nameof(user));

            return GetSettingValueForUser(name, user.TenantId, user.UserId);
        }

        #endregion

        #region Nested classes

        private class SettingValueObject : ISettingValue
        {
            public string Name { get; private set; }

            public string Value { get; private set; }

            public SettingValueObject(string name, string value)
            {
                Value = value;
                Name = name;
            }
        }

        #endregion
    }
}