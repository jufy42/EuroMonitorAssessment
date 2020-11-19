using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Library.DataCore;
using Microsoft.AspNetCore.Identity;

namespace Library.Repository
{
    public class SystemUserStore : IUserPasswordStore<SystemUser>,
        IUserSecurityStampStore<SystemUser>,
        IUserEmailStore<SystemUser>,
        IUserRoleStore<SystemUser>
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public SystemUserStore(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<string> GetUserIdAsync(SystemUser user, CancellationToken cancellationToken)
        {
            var userCheck = await _repositoryManager.UserRepository.GetUser(user.Id);
            if (userCheck == null) return "";
            return user.Id.ToString();
        }

        public async Task<string> GetUserNameAsync(SystemUser user, CancellationToken cancellationToken)
        {
            var userCheck = await _repositoryManager.UserRepository.GetUser(user.Id);
            if (userCheck == null) return "";
            return user.UserName;
        }

        public Task SetUserNameAsync(SystemUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(SystemUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(SystemUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(SystemUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            User aUser = _mapper.MapUser(user);

            return _repositoryManager.UserRepository.AddUserToServer(aUser);
        }

        public async Task<IdentityResult> UpdateAsync(SystemUser user, CancellationToken cancellationToken)
        {
            User aUser = await _repositoryManager.UserRepository.GetUserById(user.Id);
            aUser.Password = user.PasswordHash;

            return await _repositoryManager.UserRepository.UpdateUserAsync(aUser);
        }

        public async Task<IdentityResult> DeleteAsync(SystemUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await _repositoryManager.UserRepository.RemoveUserByUserId(user.Id);
        }

        public async Task<SystemUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            User user = await _repositoryManager.UserRepository.GetUserById(new Guid(userId));
            return _mapper.MapIdentityUser(user);
        }

        public async Task<SystemUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            User user = await _repositoryManager.UserRepository.GetUserByEmailAddress(normalizedUserName);

            return _mapper.MapIdentityUser(user);
        }

        public Task SetPasswordHashAsync(SystemUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(SystemUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return _repositoryManager.UserRepository.GetPasswordAsync(user.Id);
        }

        public Task<bool> HasPasswordAsync(SystemUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task SetSecurityStampAsync(SystemUser user, string stamp, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(SystemUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("IdentityUser");

            return Task.FromResult(user.Id.ToString());
        }

        public async Task SetEmailAsync(SystemUser user, string email, CancellationToken cancellationToken)
        {
            User aUser = await _repositoryManager.UserRepository.GetUserById(user.Id);
            aUser.EmailAddress = email;
            await _repositoryManager.UserRepository.UpdateUserAsync(aUser);
        }

        public async Task<string> GetEmailAsync(SystemUser user, CancellationToken cancellationToken)
        {
            User aUser = await _repositoryManager.UserRepository.GetUserById(user.Id);
            return aUser.EmailAddress;
        }

        public async Task<bool> GetEmailConfirmedAsync(SystemUser user, CancellationToken cancellationToken)
        {
            User aUser = await _repositoryManager.UserRepository.GetUserById(user.Id);
            return !string.IsNullOrWhiteSpace(aUser.EmailAddress);
        }

        public Task SetEmailConfirmedAsync(SystemUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<SystemUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            User user = await _repositoryManager.UserRepository.GetUserByEmailAddress(normalizedEmail);

            return _mapper.MapIdentityUser(user);
        }

        public Task<string> GetNormalizedEmailAsync(SystemUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(SystemUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(SystemUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(SystemUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(SystemUser user, CancellationToken cancellationToken)
        {
            return await _repositoryManager.UserRepository.GetUserRolesAsync(user.Id);
        }

        public async Task<bool> IsInRoleAsync(SystemUser user, string roleName, CancellationToken cancellationToken)
        {
            return await _repositoryManager.UserRepository.CheckIfUserInRoleAsync(user.Id, roleName);
        }

        public Task<IList<SystemUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
