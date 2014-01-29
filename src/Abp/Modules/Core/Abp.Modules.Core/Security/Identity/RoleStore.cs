using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Security.Roles;
using Microsoft.AspNet.Identity;

namespace Abp.Security.Identity
{
    public class RoleStore<TRole, TRoleRepository> : 
        IRoleStore<TRole, int> 
        where TRole: AbpRole
        where TRoleRepository : IRoleRepository<TRole>
    {
        private readonly TRoleRepository _roleRepository;

        public RoleStore(TRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public void Dispose()
        {
            //No need to dispose since using dependency injection manager
        }

        public Task CreateAsync(TRole role)
        {
            return Task.Factory.StartNew(() => _roleRepository.Insert(role));
        }

        public Task UpdateAsync(TRole role)
        {
            return Task.Factory.StartNew(() => _roleRepository.Update(role));
        }

        public Task DeleteAsync(TRole role)
        {
            return Task.Factory.StartNew(() => _roleRepository.Delete(role.Id));
        }

        public Task<TRole> FindByIdAsync(int roleId)
        {
            return Task.Factory.StartNew(() => _roleRepository.FirstOrDefault(roleId));
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            return Task.Factory.StartNew(() => _roleRepository.FirstOrDefault(role => role.Name == roleName));
        }
    }
}
