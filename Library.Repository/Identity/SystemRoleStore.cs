using System;
using System.Threading;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Library.DataCore;
using Library.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Library.Repository
{
    public class SystemRoleStore : IRoleStore<SystemRole>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public SystemRoleStore(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public void Dispose()
        {
            // Dummy implementation
        }

        public async Task<IdentityResult> CreateAsync(SystemRole role, CancellationToken cancellationToken)
        {
            if (role == null)
                return IdentityResult.Failed();

            Role aRole = _mapper.GetRole(role);

            await _repositoryManager.RoleRepository.AddRoletoServer(aRole);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(SystemRole role, CancellationToken cancellationToken)
        {
            if (role == null)
                return IdentityResult.Failed();

            Role arole = _mapper.GetRole(role);

            await _repositoryManager.RoleRepository.UpdateRole(arole);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(SystemRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(SystemRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(SystemRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(SystemRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(SystemRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(SystemRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<SystemRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            Role role = await _repositoryManager.RoleRepository.GetByIdAsync(new Guid(roleId));
            return new SystemRole().AutoMap(role);
        }

        public async Task<SystemRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            Role role = await _repositoryManager.RoleRepository.FindByNameAsync(normalizedRoleName);
            return new SystemRole().AutoMap(role);
        }
    }
}
