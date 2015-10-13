using System;
using System.Threading.Tasks;

namespace Abp.Application.Features
{
    public interface IFeatureChecker
    {
        Task<string> GetValueAsync(string name);

        Task<string> GetValueAsync(int tenantId, string name);
    }
}