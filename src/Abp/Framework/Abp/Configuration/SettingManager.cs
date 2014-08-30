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

        #region Private fields

        private readonly ISettingDefinitionManager _settingDefinitionManager;

        private readonly Lazy<Dictionary<string, Setting>> _applicationSettings;
        private readonly ThreadSafeObjectCache<Dictionary<string, Setting>> _tenantSettingCache;
        private readonly ThreadSafeObjectCache<Dictionary<string, Setting>> _userSettingCache;

        #endregion

        #region Constructor

        public SettingManager(ISettingDefinitionManager settingDefinitionManager)
        {
            _settingDefinitionManager = settingDefinitionManager;

            Session = NullAbpSession.Instance;
            SettingStore = NullSettingStore.Instance;

            _applicationSettings = new Lazy<Dictionary<string, Setting>>(GetApplicationSettingsFromDatabase, true);
            _tenantSettingCache = new ThreadSafeObjectCache<Dictionary<string, Setting>>(new MemoryCache(GetType().Name + "_TenantSettings"), TimeSpan.FromMinutes(60)); //TODO: Get constant from somewhere else.
            _userSettingCache = new ThreadSafeObjectCache<Dictionary<string, Setting>>(new MemoryCache(GetType().Name + "_UserSettings"), TimeSpan.FromMinutes(30)); //TODO: Get constant from somewhere else.
        }

        #endregion

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

        private Setting InsertOrUpdateOrDeleteSettingValue(string name, string value, int? tenantId, long? userId)
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
                settingValue = new Setting
                {
                    TenantId = tenantId,
                    UserId = userId,
                    Name = name,
                    Value = value
                };

                //_settingRepository.Insert(settingValue);
                SettingStore.Add(settingValue);
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

        private Setting GetSettingValueForApplicationOrNull(string name)
        {
            lock (_applicationSettings.Value)
            {
                return _applicationSettings.Value.GetOrDefault(name);
            }
        }

        private Setting GetSettingValueForTenantOrNull(int tenantId, string name)
        {
            return GetReadOnlyTenantSettings(tenantId).GetOrDefault(name);
        }

        private Setting GetSettingValueForUserOrNull(long userId, string name)
        {
            return GetReadOnlyUserSettings(userId).GetOrDefault(name);
        }

        private Dictionary<string, Setting> GetApplicationSettingsFromDatabase()
        {
            var dictionary = new Dictionary<string, Setting>();

            var settingValues = SettingStore.GetAll(null, null);
            foreach (var settingValue in settingValues)
            {
                dictionary[settingValue.Name] = settingValue;
            }

            return dictionary;
        }


        private ImmutableDictionary<string, Setting> GetReadOnlyTenantSettings(int tenantId)
        {
            var cachedDictionary = GetTenantSettingsFromCache(tenantId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }
        private ImmutableDictionary<string, Setting> GetReadOnlyUserSettings(long userId)
        {
            var cachedDictionary = GetUserSettingsFromCache(userId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private Dictionary<string, Setting> GetTenantSettingsFromCache(int tenantId)
        {
            return _tenantSettingCache.Get(
                tenantId.ToString(),
                () =>
                {   //Getting from database
                    var dictionary = new Dictionary<string, Setting>();

                    var settingValues = SettingStore.GetAll(tenantId, null);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
        }

        private Dictionary<string, Setting> GetUserSettingsFromCache(long userId)
        {
            return _userSettingCache.Get(
                userId.ToString(),
                () =>
                {   //Getting from database
                    var dictionary = new Dictionary<string, Setting>();

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