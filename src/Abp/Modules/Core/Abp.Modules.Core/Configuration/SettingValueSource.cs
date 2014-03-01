using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Security.Users;
using Abp.Utils.Extensions.Collections;

namespace Abp.Configuration
{
    /// <summary>
    /// This class implements <see cref="ISettingValueSource"/> to manage setting values in the database.
    /// </summary>
    public class SettingValueSource : ISettingValueSource
    {
        private readonly ISettingValueRepository _settingValueRepository;

        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingValueSource(ISettingValueRepository settingValueRepository, ISettingManager settingManager)
        {
            _settingValueRepository = settingValueRepository;
            _settingManager = settingManager;
        }

        public string GetSettingValue(string name)
        {
            var setting = _settingManager.GetSetting(name);

            if (setting.Scopes.HasFlag(SettingScopes.User))
            {
                var userId = AbpUser.CurrentUserId;
                if (userId > 0)
                {
                    var settingValue = _settingValueRepository.FirstOrDefault(sv => sv.UserId == userId && sv.Name == name);
                    if (settingValue != null)
                    {
                        return settingValue.Value;
                    }
                }
            }

            if (setting.Scopes.HasFlag(SettingScopes.Application))
            {
                var settingValue = _settingValueRepository.FirstOrDefault(sv => sv.UserId == null && sv.Name == name);
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
            return _settingValueRepository
                .GetAllList(setting => setting.UserId == null)
                .Select(setting => new SettingValue(setting.Name, setting.Value))
                .ToImmutableList();
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(int userId)
        {
            return _settingValueRepository
                .GetAllList(setting => setting.UserId == userId)
                .Select(setting => new SettingValue(setting.Name, setting.Value))
                .ToImmutableList();
        }
    }
}