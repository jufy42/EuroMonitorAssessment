using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.ADT;
using Library.DataBase;
using Library.DataCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DBContext _dbContext; 

        public RoleRepository() {
            _dbContext = new DBContext(new DbContextOptions<DBContext>());
        }

        public async Task<int> AddRoletoServer(Role newRole)
        {
            await _dbContext.Roles.AddAsync(newRole);
            return await _dbContext.SaveChangesAsync();
        }

        public Task<int> UpdateRole(Role role)
        {
            throw new NotImplementedException();
        }

        public async Task<Role> GetByIdAsync(Guid roleId)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(m => m.Id == roleId);
        }

        public async Task<Role> FindByNameAsync(string roleName)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(m => m.Name == roleName);
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _dbContext.Roles.ToListAsync();
        }
    }
}
