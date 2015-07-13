using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Threading;

namespace Abp.Configuration
{
    /// <summary>
    /// This class implements <see cref="ISettingManager"/> to manage setting values in the database.
    /// </summary>
    public class SettingManager : ISettingManager, ISingletonDependency
    {
        /// <summary>
        /// Reference to the current Session.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Reference to the setting store.
        /// </summary>
        public ISettingStore SettingStore { get; set; }

        private readonly ISettingDefinitionManager _settingDefinitionManager;

        private readonly Lazy<Dictionary<string, SettingInfo>> _applicationSettings;

        private readonly AsyncThreadSafeObjectCache<Dictionary<string, SettingInfo>> _tenantSettingCache;

        private readonly AsyncThreadSafeObjectCache<Dictionary<string, SettingInfo>> _userSettingCache;

        /// <inheritdoc/>
        public SettingManager(ISettingDefinitionManager settingDefinitionManager)
        {
            _settingDefinitionManager = settingDefinitionManager;

            AbpSession = NullAbpSession.Instance;
            SettingStore = DefaultConfigSettingStore.Instance;

            _applicationSettings = new Lazy<Dictionary<string, SettingInfo>>(() => AsyncHelper.RunSync(GetApplicationSettingsFromDatabase), true); //TODO: Run async
            _tenantSettingCache = new AsyncThreadSafeObjectCache<Dictionary<string, SettingInfo>>(new MemoryCache(GetType().FullName + ".TenantSettings"), TimeSpan.FromMinutes(60)); //TODO: Get constant from somewhere else.
            _userSettingCache = new AsyncThreadSafeObjectCache<Dictionary<string, SettingInfo>>(new MemoryCache(GetType().FullName + ".UserSettings"), TimeSpan.FromMinutes(20)); //TODO: Get constant from somewhere else.
        }

        #region Public methods

        /// <inheritdoc/>
        public Task<string> GetSettingValueAsync(string name)
        {
            return GetSettingValueInternalAsync(name, AbpSession.TenantId, AbpSession.UserId);
        }

        public Task<string> GetSettingValueForApplicationAsync(string name)
        {
            return GetSettingValueInternalAsync(name);
        }

        public Task<string> GetSettingValueForTenantAsync(string name, int tenantId)
        {
            return GetSettingValueInternalAsync(name, tenantId);
        }

        public Task<string> GetSettingValueForUserAsync(string name, int? tenantId, long userId)
        {
            return GetSettingValueInternalAsync(name, tenantId, userId);
        }

        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync()
        {
            return await GetAllSettingValuesAsync(SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User);
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
                foreach (var settingValue in await GetAllSettingValuesForUserAsync(AbpSession.UserId.Value))
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
        public Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForApplicationAsync()
        {
            //TODO: Make async by removing lazy, replacing caching
            lock (_applicationSettings.Value)
            {
                return Task.FromResult<IReadOnlyList<ISettingValue>>(_applicationSettings.Value.Values
                    .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                    .ToImmutableList());
            }
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForTenantAsync(int tenantId)
        {
            return (await GetReadOnlyTenantSettings(tenantId)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForUserAsync(long userId)
        {
            return (await GetReadOnlyUserSettings(userId)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual async Task ChangeSettingForApplicationAsync(string name, string value)
        {
            var settingValue = await InsertOrUpdateOrDeleteSettingValueAsync(name, value, null, null);
            lock (_applicationSettings.Value)
            {
                if (settingValue == null)
                {
                    _applicationSettings.Value.Remove(name);
                }
                else
                {
                    _applicationSettings.Value[name] = settingValue;
                }
            }
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual async Task ChangeSettingForTenantAsync(int tenantId, string name, string value)
        {
            var settingValue = await InsertOrUpdateOrDeleteSettingValueAsync(name, value, tenantId, null);
            var cachedDictionary = await GetTenantSettingsFromCache(tenantId);
            lock (cachedDictionary)
            {
                if (settingValue == null)
                {
                    cachedDictionary.Remove(name);
                }
                else
                {
                    cachedDictionary[name] = settingValue;
                }
            }
        }

        /// <inheritdoc/>
        [UnitOfWork]
        public virtual async Task ChangeSettingForUserAsync(long userId, string name, string value)
        {
            var settingValue = await InsertOrUpdateOrDeleteSettingValueAsync(name, value, null, userId);
            var cachedDictionary = await GetUserSettingsFromCache(userId);
            lock (cachedDictionary)
            {
                if (settingValue == null)
                {
                    cachedDictionary.Remove(name);
                }
                else
                {
                    cachedDictionary[name] = settingValue;
                }
            }
        }

        #endregion

        #region Private methods

        private async Task<string> GetSettingValueInternalAsync(string name, int? tenantId = null, long? userId = null)
        {
            var settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);

            //Get for user if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.User) && userId.HasValue)
            {
                var settingValue = await GetSettingValueForUserOrNull(userId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for tenant if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Tenant) && tenantId.HasValue)
            {
                var settingValue = await GetSettingValueForTenantOrNull(tenantId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
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
            }

            //Not defined, get default value
            return settingDefinition.DefaultValue;
        }

        private async Task<SettingInfo> InsertOrUpdateOrDeleteSettingValueAsync(string name, string value, int? tenantId, long? userId)
        {
            if (tenantId.HasValue && userId.HasValue)
            {
                throw new ApplicationException("Both of tenantId and userId can not be set!");
            }

            var settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);
            var settingValue = await SettingStore.GetSettingOrNullAsync(tenantId, userId, name);

            //Determine defaultValue
            var defaultValue = settingDefinition.DefaultValue;

            if (settingDefinition.IsInherited)
            {
                //For Tenant and User, Application's value overrides Setting Definition's default value.
                if (tenantId.HasValue || userId.HasValue)
                {
                    var applicationValue = GetSettingValueForApplicationOrNull(name);
                    if (applicationValue != null)
                    {
                        defaultValue = applicationValue.Value;
                    }
                }

                //For User, Tenants's value overrides Application's default value.
                if (userId.HasValue && AbpSession.TenantId.HasValue)
                {
                    var tenantValue = await GetSettingValueForTenantOrNull(AbpSession.TenantId.Value, name);
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

        private SettingInfo GetSettingValueForApplicationOrNull(string name)
        {
            lock (_applicationSettings.Value)
            {
                return _applicationSettings.Value.GetOrDefault(name);
            }
        }

        private async Task<SettingInfo> GetSettingValueForTenantOrNull(int tenantId, string name)
        {
            return (await GetReadOnlyTenantSettings(tenantId)).GetOrDefault(name);
        }

        private async Task<SettingInfo> GetSettingValueForUserOrNull(long userId, string name)
        {
            return (await GetReadOnlyUserSettings(userId)).GetOrDefault(name);
        }

        private async Task<Dictionary<string, SettingInfo>> GetApplicationSettingsFromDatabase()
        {
            var dictionary = new Dictionary<string, SettingInfo>();

            var settingValues = await SettingStore.GetAllListAsync(null, null);
            foreach (var settingValue in settingValues)
            {
                dictionary[settingValue.Name] = settingValue;
            }

            return dictionary;
        }

        private async Task<ImmutableDictionary<string, SettingInfo>> GetReadOnlyTenantSettings(int tenantId)
        {
            var cachedDictionary = await GetTenantSettingsFromCache(tenantId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private async Task<ImmutableDictionary<string, SettingInfo>> GetReadOnlyUserSettings(long userId)
        {
            var cachedDictionary = await GetUserSettingsFromCache(userId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private async Task<Dictionary<string, SettingInfo>> GetTenantSettingsFromCache(int tenantId)
        {
            return await _tenantSettingCache.GetAsync(
                tenantId.ToString(CultureInfo.InvariantCulture),
                async () =>
                {
                    var dictionary = new Dictionary<string, SettingInfo>();

                    var settingValues = await SettingStore.GetAllListAsync(tenantId, null);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
        }

        private async Task<Dictionary<string, SettingInfo>> GetUserSettingsFromCache(long userId)
        {
            return await _userSettingCache.GetAsync(
                userId.ToString(CultureInfo.InvariantCulture),
                async () =>
                {
                    var dictionary = new Dictionary<string, SettingInfo>();

                    var settingValues = await SettingStore.GetAllListAsync(null, userId);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
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