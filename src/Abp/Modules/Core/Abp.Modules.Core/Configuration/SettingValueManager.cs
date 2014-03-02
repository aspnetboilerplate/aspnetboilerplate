using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Caching;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Security.Users;
using Abp.Utils.Extensions.Collections;

namespace Abp.Configuration
{
    /// <summary>
    /// This class implements <see cref="ISettingValueManager"/> to manage setting values in the database.
    /// </summary>
    public class SettingValueManager : ISettingValueManager
    {
        #region Private fields

        private readonly ISettingValueRepository _settingValueRepository;
        private readonly ISettingManager _settingManager;

        private readonly Lazy<Dictionary<string, SettingValueRecord>> _applicationSettings;
        private readonly ThreadSafeObjectCache<Dictionary<string, SettingValueRecord>> _userSettingCache;

        #endregion

        #region Constructor

        public SettingValueManager(ISettingValueRepository settingValueRepository, ISettingManager settingManager)
        {
            _settingValueRepository = settingValueRepository;
            _settingManager = settingManager;
            _applicationSettings = new Lazy<Dictionary<string, SettingValueRecord>>(GetApplicationSettingsFromDatabase, true);
            _userSettingCache = new ThreadSafeObjectCache<Dictionary<string, SettingValueRecord>>(new MemoryCache(GetType().Name), TimeSpan.FromMinutes(30)); //TODO: Get constant from somewhere else.
        }

        #endregion

        #region Public methods

        public string GetSettingValue(string name)
        {
            var setting = _settingManager.GetSetting(name);

            //Check if defined for current user
            if (setting.Scopes.HasFlag(SettingScopes.User))
            {
                if (AbpUser.CurrentUserId > 0)
                {
                    var settingValue = GetSettingValueForUserOrNull(AbpUser.CurrentUserId, name);
                    if (settingValue != null)
                    {
                        return settingValue.Value;
                    }
                }
            }

            //Check if defined for the application
            if (setting.Scopes.HasFlag(SettingScopes.Application))
            {
                var settingValue = GetSettingValueForApplicationOrNull(name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }
            }

            return setting.DefaultValue;
        }

        public T GetSettingValue<T>(string name)
        {
            return (T)Convert.ChangeType(GetSettingValue(name), typeof(T));
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValues()
        {
            var settings = new Dictionary<string, Setting>();
            var settingValues = new Dictionary<string, ISettingValue>();

            //Fill all setting with default values.
            foreach (var setting in _settingManager.GetAllSettings())
            {
                settings[setting.Name] = setting;
                settingValues[setting.Name] = new SettingValue(setting.Name, setting.DefaultValue);
            }

            //Overwrite application settings
            foreach (var settingValue in GetAllSettingValuesForApplication())
            {
                var setting = settings.GetOrDefault(settingValue.Name);
                if (setting != null && setting.Scopes.HasFlag(SettingScopes.Application))
                {
                    settingValues[settingValue.Name] = new SettingValue(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite user settings
            var userId = AbpUser.CurrentUserId;
            if (userId > 0)
            {
                foreach (var settingValue in GetAllSettingValuesForUser(userId))
                {
                    var setting = settings.GetOrDefault(settingValue.Name);
                    if (setting != null && setting.Scopes.HasFlag(SettingScopes.User))
                    {
                        settingValues[settingValue.Name] = new SettingValue(settingValue.Name, settingValue.Value);
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
                    .Select(setting => new SettingValue(setting.Name, setting.Value))
                    .ToImmutableList();
            }
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(int userId)
        {
            return GetReadOnlyUserSettings(userId).Values
                .Select(setting => new SettingValue(setting.Name, setting.Value))
                .ToImmutableList();
        }

        [UnitOfWork]
        public void ChangeSettingForApplication(string name, string value)
        {
            var settingValue = InsertOrUpdateOrDeleteSettingValue(name, value, null);
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
        public void ChangeSettingForUser(int userId, string name, string value)
        {
            var settingValue = InsertOrUpdateOrDeleteSettingValue(name, value, userId);
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

        private SettingValueRecord InsertOrUpdateOrDeleteSettingValue(string name, string value, int? userId)
        {
            var setting = _settingManager.GetSetting(name);
            var settingValue = _settingValueRepository.FirstOrDefault(sv => sv.UserId == userId && sv.Name == name);
            var applicationSettingValue = userId == null ? settingValue : GetSettingValueForApplicationOrNull(name);

            if ((userId == null && setting.DefaultValue == value) 
                || (userId != null && ((applicationSettingValue != null && applicationSettingValue.Value == value) || (applicationSettingValue == null && setting.DefaultValue == value))))
            {
                //No need to store a setting value since it's default value.
                if (settingValue != null)
                {
                    _settingValueRepository.Delete(settingValue);
                }

                return null;
            }

            if (settingValue != null)
            {
                //A record exists, update it
                settingValue.Value = value;
            }
            else
            {
                //No record found, create one
                settingValue = new SettingValueRecord
                {
                    UserId = userId,
                    Name = name,
                    Value = value
                };

                _settingValueRepository.Insert(settingValue);
            }

            return settingValue;
        }

        private SettingValueRecord GetSettingValueForApplicationOrNull(string name)
        {
            lock (_applicationSettings.Value)
            {
                return _applicationSettings.Value.GetOrDefault(name);
            }
        }

        private SettingValueRecord GetSettingValueForUserOrNull(int userId, string name)
        {
            return GetReadOnlyUserSettings(userId).GetOrDefault(name);
        }

        private Dictionary<string, SettingValueRecord> GetApplicationSettingsFromDatabase()
        {
            var dictionary = new Dictionary<string, SettingValueRecord>();

            var settingValues = _settingValueRepository.GetAllList(setting => setting.UserId == null);
            foreach (var settingValue in settingValues)
            {
                dictionary[settingValue.Name] = settingValue;
            }

            return dictionary;
        }

        private ImmutableDictionary<string, SettingValueRecord> GetReadOnlyUserSettings(int userId)
        {
            var cachedDictionary = GetUserSettingsFromCache(userId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private Dictionary<string, SettingValueRecord> GetUserSettingsFromCache(int userId)
        {
            return _userSettingCache.Get(
                userId.ToString(),
                () =>
                {   //Getting from database
                    var dictionary = new Dictionary<string, SettingValueRecord>();

                    var settingValues = _settingValueRepository.GetAllList(setting => setting.UserId == userId);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                });
        }

        #endregion
    }
}