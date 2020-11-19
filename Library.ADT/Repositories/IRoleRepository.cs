using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.DataCore;

namespace Library.ADT
{
    public interface IRoleRepository
    {
        Task<int> AddRoletoServer(Role newRole);
        Task<int> UpdateRole(Role role);
        Task<Role> GetByIdAsync(Guid roleId);
        Task<Role> FindByNameAsync(string roleName);
        Task<List<Role>> GetRoles();
    }
}
