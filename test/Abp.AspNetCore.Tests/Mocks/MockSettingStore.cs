using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;

namespace Abp.AspNetCore.Mocks;

public class MockSettingStore : ISettingStore
{
    private readonly List<SettingInfo> _settings;

    public MockSettingStore()
    {
        _settings = new List<SettingInfo>
        {
            new SettingInfo(null, null, "MyAppLevelSetting", "42"),
        };
    }

    public Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name)
    {
        return Task.FromResult(GetSettingOrNull(tenantId, userId, name));
    }

    public SettingInfo GetSettingOrNull(int? tenantId, long? userId, string name)
    {
        return _settings.FirstOrDefault(s => s.TenantId == tenantId && s.UserId == userId && s.Name == name);
    }

#pragma warning disable 1998
    public Task DeleteAsync(SettingInfo setting)
    {
        Delete(setting);
        return Task.CompletedTask;
    }

    public void Delete(SettingInfo setting)
    {
        _settings.RemoveAll(s =>
            s.TenantId == setting.TenantId && s.UserId == setting.UserId && s.Name == setting.Name);
    }
#pragma warning restore 1998

#pragma warning disable 1998
    public Task CreateAsync(SettingInfo setting)
    {
        Create(setting);
        return Task.CompletedTask;
    }

    public void Create(SettingInfo setting)
    {
        _settings.Add(setting);
    }
#pragma warning restore 1998

    public Task UpdateAsync(SettingInfo setting)
    {
        Update(setting);
        return Task.CompletedTask;
    }

    public void Update(SettingInfo setting)
    {
        var s = GetSettingOrNull(setting.TenantId, setting.UserId, setting.Name);
        if (s != null)
        {
            s.Value = setting.Value;
        }
    }

    public Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId)
    {
        return Task.FromResult(GetAllList(tenantId, userId));
    }

    public List<SettingInfo> GetAllList(int? tenantId, long? userId)
    {
        var allSetting = _settings.Where(s => s.TenantId == tenantId && s.UserId == userId)
            .Select(s => new SettingInfo(s.TenantId, s.UserId, s.Name, s.Value)).ToList();

        // Add some undefined settings.
        allSetting.Add(new SettingInfo(null, null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
        allSetting.Add(new SettingInfo(1, null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
        allSetting.Add(new SettingInfo(1, 1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        return allSetting;
    }
}