using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Caching;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Utils.Extensions.Collections;

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
        public IAbpSession Session { get; set; }

        /// <summary>
        /// Reference to the setting store.
        /// </summary>
        public ISettingStore SettingStore { get; set; }

        private readonly ISettingDefinitionManager _settingDefinitionManager;

        private readonly Lazy<Dictionary<string, SettingInfo>> _applicationSettings;

        private readonly ThreadSafeObjectCache<Dictionary<string, SettingInfo>> _tenantSettingCache;

        private readonly ThreadSafeObjectCache<Dictionary<string, SettingInfo>> _userSettingCache;

        public SettingManager(ISettingDefinitionManager settingDefinitionManager)
        {
            _settingDefinitionManager = settingDefinitionManager;

            Session = NullAbpSession.Instance;
            SettingStore = NullSettingStore.Instance; //Should be constructor injection? For that, ISettingStore must be registered!

            _applicationSettings = new Lazy<Dictionary<string, SettingInfo>>(GetApplicationSettingsFromDatabase, true);
            _tenantSettingCache = new ThreadSafeObjectCache<Dictionary<string, SettingInfo>>(new MemoryCache(GetType().FullName + ".TenantSettings"), TimeSpan.FromMinutes(60)); //TODO: Get constant from somewhere else.
            _userSettingCache = new ThreadSafeObjectCache<Dictionary<string, SettingInfo>>(new MemoryCache(GetType().FullName + ".UserSettings"), TimeSpan.FromMinutes(20)); //TODO: Get constant from somewhere else.
        }

        #region Public methods

        public string GetSettingValue(string name)
        {
            var settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);

            //Get for user if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.User) && Session.UserId.HasValue)
            {
                var settingValue = GetSettingValueForUserOrNull(Session.UserId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }
            }

            //Get for tenant if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Tenant) && Session.TenantId.HasValue)
            {
                var settingValue = GetSettingValueForTenantOrNull(Session.TenantId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
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

        public T GetSettingValue<T>(string name)
        {
            return (T)Convert.ChangeType(GetSettingValue(name), typeof(T));
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValues()
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
            foreach (var settingValue in GetAllSettingValuesForApplication())
            {
                var setting = settingDefinitions.GetOrDefault(settingValue.Name);
                if (setting != null && setting.Scopes.HasFlag(SettingScopes.Application))
                {
                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite tenant settings
            var tenantId = Session.TenantId;
            if (tenantId.HasValue)
            {
                foreach (var settingValue in GetAllSettingValuesForTenant(tenantId.Value))
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);
                    if (setting != null && setting.Scopes.HasFlag(SettingScopes.Tenant))
                    {
                        settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                    }
                }
            }

            //Overwrite user settings
            var userId = Session.UserId;
            if (userId.HasValue)
            {
                foreach (var settingValue in GetAllSettingValuesForUser(userId.Value))
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

        public IReadOnlyList<ISettingValue> GetAllSettingValuesForApplication()
        {
            lock (_applicationSettings.Value)
            {
                return _applicationSettings.Value.Values
                    .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                    .ToImmutableList();
            }
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValuesForTenant(int tenantId)
        {
            return GetReadOnlyTenantSettings(tenantId).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(long userId)
        {
            return GetReadOnlyUserSettings(userId).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        [UnitOfWork]
        public void ChangeSettingForApplication(string name, string value)
        {
            var settingValue = InsertOrUpdateOrDeleteSettingValue(name, value, null, null);
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

        [UnitOfWork]
        public void ChangeSettingForTenant(int tenantId, string name, string value)
        {
            var settingValue = InsertOrUpdateOrDeleteSettingValue(name, value, tenantId, null);
            var cachedDictionary = GetTenantSettingsFromCache(tenantId);
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

        [UnitOfWork]
        public void ChangeSettingForUser(long userId, string name, string value)
        {
            var settingValue = InsertOrUpdateOrDeleteSettingValue(name, value, null, userId);
            var cachedDictionary = GetUserSettingsFromCache(userId);
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

        private SettingInfo InsertOrUpdateOrDeleteSettingValue(string name, string value, int? tenantId, long? userId)
        {
            if (tenantId.HasValue && userId.HasValue)
            {
                throw new ApplicationException("Both of tenantId and userId can not be set!");
            }

            var settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);
            //var settingValue = _settingRepository.FirstOrDefault(sv => sv.TenantId == tenantId && sv.UserId == userId && sv.Name == name);
            var settingValue = SettingStore.GetSettingOrNull(tenantId, userId, name);
            

            //Determine defaultValue
            var defaultValue = settingDefinition.DefaultValue;

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
            if (userId.HasValue)
            {
                var currentTenantId = Session.TenantId;
                if (currentTenantId.HasValue)
                {
                    var tenantValue = GetSettingValueForTenantOrNull(currentTenantId.Value, name);
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
                    //_settingRepository.Delete(settingValue);
                    SettingStore.Delete(settingValue);
                }

                return null;
            }

            //It's not default value and not stored on database, so insert it
            if (settingValue == null)
            {
                settingValue = new SettingInfo
                {
                    TenantId = tenantId,
                    UserId = userId,
                    Name = name,
                    Value = value
                };

                //_settingRepository.Insert(settingValue);
                SettingStore.Create(settingValue);
                return settingValue;
            }

            //It's same value as it's, no need to update
            if (settingValue.Value == value)
            {
                return settingValue;                
            }

            //Update the setting on database.
            settingValue.Value = value;
            
            return settingValue;
        }

        private SettingInfo GetSettingValueForApplicationOrNull(string name)
        {
            lock (_applicationSettings.Value)
            {
                return _applicationSettings.Value.GetOrDefault(name);
            }
        }

        private SettingInfo GetSettingValueForTenantOrNull(int tenantId, string name)
        {
            return GetReadOnlyTenantSettings(tenantId).GetOrDefault(name);
        }

        private SettingInfo GetSettingValueForUserOrNull(long userId, string name)
        {
            return GetReadOnlyUserSettings(userId).GetOrDefault(name);
        }

        private Dictionary<string, SettingInfo> GetApplicationSettingsFromDatabase()
        {
            var dictionary = new Dictionary<string, SettingInfo>();

            var settingValues = SettingStore.GetAll(null, null);
            foreach (var settingValue in settingValues)
            {
                dictionary[settingValue.Name] = settingValue;
            }

            return dictionary;
        }


        private ImmutableDictionary<string, SettingInfo> GetReadOnlyTenantSettings(int tenantId)
        {
            var cachedDictionary = GetTenantSettingsFromCache(tenantId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }
        private ImmutableDictionary<string, SettingInfo> GetReadOnlyUserSettings(long userId)
        {
            var cachedDictionary = GetUserSettingsFromCache(userId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private Dictionary<string, SettingInfo> GetTenantSettingsFromCache(int tenantId)
        {
            return _tenantSettingCache.Get(
                tenantId.ToString(),
                () =>
                {   //Getting from database
                    var dictionary = new Dictionary<string, SettingInfo>();

                    var settingValues = SettingStore.GetAll(tenantId, null);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
        }

        private Dictionary<string, SettingInfo> GetUserSettingsFromCache(long userId)
        {
            return _userSettingCache.Get(
                userId.ToString(),
                () =>
                {   //Getting from database
                    var dictionary = new Dictionary<string, SettingInfo>();

                    var settingValues = SettingStore.GetAll(null, userId);
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