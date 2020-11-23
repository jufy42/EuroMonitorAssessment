using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Library.DataCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Library.Repository
{
    public class SystemUserStore : BaseClass, IUserPasswordStore<SystemUser>,
        IUserSecurityStampStore<SystemUser>,
        IUserEmailStore<SystemUser>,
        IUserRoleStore<SystemUser>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SystemUserStore(IRepositoryManager repositoryManager, IMapper mapper, ILogger<SystemUserStore> logger)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _logger = logger;
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

        public async Task SetUserNameAsync(SystemUser user, string userName, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                User aUser = _mapper.MapUser(user);

                await _repositoryManager.UserRepository.UpdateUserAsync(aUser);
                await _repositoryManager.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"SetUserNameAsync : {e.Message}");
            }
        }

        public async Task<string> GetNormalizedUserNameAsync(SystemUser user, CancellationToken cancellationToken)
        {
            try
            {
                var dbUser = await _repositoryManager.UserRepository.GetUserById(user.Id);

                return dbUser == null ? "" : user.NormalizedUserName;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetNormalizedUserNameAsync : {e.Message}");
                return null;
            }
        }

        public async Task SetNormalizedUserNameAsync(SystemUser user, string normalizedName, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                User aUser = _mapper.MapUser(user);

                await _repositoryManager.UserRepository.UpdateUserAsync(aUser);
                await _repositoryManager.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"SetNormalizedUserNameAsync : {e.Message}");
            }
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
            try
            {
                var aUser = _mapper.MapUser(user);
                await _repositoryManager.UserRepository.UpdateUserAsync(aUser);
                await _repositoryManager.SaveChangesAsync(cancellationToken);

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateAsync : {e.Message}");
                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> DeleteAsync(SystemUser user, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return await _repositoryManager.UserRepository.RemoveUserByUserId(user.Id);
            }
            catch (Exception e)
            {
                _logger.LogError($"DeleteAsync : {e.Message}");
                return IdentityResult.Failed();
            }
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
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                user.PasswordHash = passwordHash;
            }
            catch (Exception e)
            {
                _logger.LogError($"SetPasswordHashAsync : {e.Message}");
            }

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

        public async Task SetEmailConfirmedAsync(SystemUser user, bool confirmed, CancellationToken cancellationToken)
        {
            try
            {
                User aUser = await _repositoryManager.UserRepository.GetUserById(user.Id);
                aUser.EmailConfirmed = confirmed;
                await _repositoryManager.UserRepository.UpdateUserAsync(aUser);
            }
            catch (Exception e)
            {
                _logger.LogError($"SetEmailConfirmedAsync : {e.Message}");
            }
        }

        public async Task<SystemUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            User user = await _repositoryManager.UserRepository.GetUserByEmailAddress(normalizedEmail);

            return _mapper.MapIdentityUser(user);
        }

        public async Task<string> GetNormalizedEmailAsync(SystemUser user, CancellationToken cancellationToken)
        {
            try
            {
                User aUser = await _repositoryManager.UserRepository.GetUserById(user.Id);
                return aUser.NormalizedEmail;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetNormalizedEmailAsync : {e.Message}");
                return null;
            }
        }

        public async Task SetNormalizedEmailAsync(SystemUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            try
            {
                User aUser = await _repositoryManager.UserRepository.GetUserById(user.Id);
                aUser.NormalizedEmail = normalizedEmail;
                await _repositoryManager.UserRepository.UpdateUserAsync(aUser);
            }
            catch (Exception e)
            {
                _logger.LogError($"SetNormalizedEmailAsync : {e.Message}");
            }
        }

        public async Task AddToRoleAsync(SystemUser user, string roleName, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));
                await _repositoryManager.UserRepository.AssignUsertoRole(roleName, user.Id);
            }
            catch (Exception e)
            {
                _logger.LogError($"AddToRoleAsync : {e.Message}");
            }
        }

        public async Task RemoveFromRoleAsync(SystemUser user, string roleName, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));
                await _repositoryManager.UserRepository.RemoveRoleFromUser(user.Id, roleName);
            }
            catch (Exception e)
            {
                _logger.LogError($"RemoveFromRoleAsync : {e.Message}");
            }
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
