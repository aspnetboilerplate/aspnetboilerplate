using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Organizations;

public interface IOrganizationUnitManager
{
    Task CreateAsync(OrganizationUnit organizationUnit);
    void Create(OrganizationUnit organizationUnit);
    Task UpdateAsync(OrganizationUnit organizationUnit);
    void Update(OrganizationUnit organizationUnit);
    Task<string> GetNextChildCodeAsync(long? parentId);
    string GetNextChildCode(long? parentId);
    Task<OrganizationUnit> GetLastChildOrNullAsync(long? parentId);
    OrganizationUnit GetLastChildOrNull(long? parentId);
    Task<string> GetCodeAsync(long id);
    string GetCode(long id);
    Task DeleteAsync(long id);
    void Delete(long id);
    Task MoveAsync(long id, long? parentId);
    void Move(long id, long? parentId);
    Task<List<OrganizationUnit>> FindChildrenAsync(long? parentId, bool recursive = false);
    List<OrganizationUnit> FindChildren(long? parentId, bool recursive = false);
}