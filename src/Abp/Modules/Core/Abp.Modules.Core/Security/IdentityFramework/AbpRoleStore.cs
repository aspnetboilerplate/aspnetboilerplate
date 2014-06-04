using System.Threading.Tasks;
using Abp.Security.Roles;
using Microsoft.AspNet.Identity;

namespace Abp.Security.IdentityFramework
{
    public class AbpRoleStore : IRoleStore<AbpRole, int> 
    {
        private readonly IAbpRoleRepository _roleRepository;

        public AbpRoleStore(IAbpRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public void Dispose()
        {
            //No need to dispose since using dependency injection manager
        }

        public Task CreateAsync(AbpRole role)
        {
            return Task.Factory.StartNew(() => _roleRepository.Insert(role));
        }

        public Task UpdateAsync(AbpRole role)
        {
            return Task.Factory.StartNew(() => _roleRepository.Update(role));
        }

        public Task DeleteAsync(AbpRole role)
        {
            return Task.Factory.StartNew(() => _roleRepository.Delete(role.Id));
        }

        public Task<AbpRole> FindByIdAsync(int roleId)
        {
            return Task.Factory.StartNew(() => _roleRepository.FirstOrDefault(roleId));
        }

        public Task<AbpRole> FindByNameAsync(string roleName)
        {
            return Task.Factory.StartNew(() => _roleRepository.FirstOrDefault(role => role.Name == roleName));
        }
    }
}
