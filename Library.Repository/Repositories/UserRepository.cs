using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Library.DataBase;
using Library.DataCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContext _dbContext;
        private readonly IPasswordHasher<SystemUser> _encryption;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserRepository(IPasswordHasher<SystemUser> encryption, IMapper mapper, ILogger<UserRepository> logger)
        {
            _dbContext = new DBContext(new DbContextOptions<DBContext>());
            _encryption = encryption;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ValidationResult> ValidateUser(string emailAddress, string password)
        {
            var validationResult = new ValidationResult();

            try
            {
                var user = await GetUserByEmailAddress(emailAddress);

                if (user == null)
                {
                    validationResult.ValidationResourceString = "Invalid Username or Password";
                    validationResult.IsValid = false;
                }

                if (user != null)
                {
                    var passwordVerifiedResult =
                        _encryption.VerifyHashedPassword(_mapper.MapIdentityUser(user), user.Password, password);

                    if (passwordVerifiedResult != PasswordVerificationResult.Success)
                    {
                        validationResult.ValidationResourceString = "Invalid Username or Password";
                        validationResult.IsValid = false;
                    }
                    else if (user.IsLocked)
                    {
                        validationResult.ValidationResourceString = "User is locked out";
                        validationResult.IsValid = false;
                    }
                    else
                    {
                        validationResult.UserId = user.Id;
                        validationResult.UserName = user.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ValidateUser : {e.Message}");
                validationResult.ValidationResourceString = "";
                validationResult.IsValid = false;
            }

            return validationResult;
        }

        public async Task<User> GetUserById(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == id);

                return user;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserById : {e.Message}");
                return null;
            }
        }

        public async Task<User> GetUserByEmailAddress(string emailAddress)
        {
            try
            {            
                var query = _dbContext.Users;

                var allUsers = await query.ToListAsync();

                return allUsers.FirstOrDefault(m => m.EmailAddress.ToLower().Equals(emailAddress.ToLower()));
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserByEmailAddress : {e.Message}");
                return null;
            }
        }

        public async Task<string> GetPasswordAsync(Guid identityUserId)
        {
            try
            {
                var user = await GetUserById(identityUserId);
                return user.Password;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetPasswordAsync : {e.Message}");
                return null;
            }
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            try
            {
                var user = await GetUserById(userId);

                var userRoles = await _dbContext.UserRoles.Where(p => p.UserId == user.Id).Select(p => p.RoleId).ToListAsync();
                
                var roles = await _dbContext.Roles.Where(p => userRoles.Contains(p.Id)).ToListAsync();
                roles ??= new List<Role>();

                return roles.Select(p => p.Name).ToList();            
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserRolesAsync : {e.Message}");
                return new List<string>();
            }
        }

        public async Task<bool> CheckIfUserInRoleAsync(Guid userId, string roleName)
        {
            try
            {
                var userRoles = await GetUserRolesAsync(userId);
                return userRoles.Contains(roleName);
            }
            catch (Exception e)
            {
                _logger.LogError($"CheckIfUserInRoleAsync : {e.Message}");
                return false;
            }
        }

        public async Task<List<RegisterViewModel>> GetUsers()
        {
            try
            {
                var users = await _dbContext.Users
                    .ToListAsync();

                return users
                    .Select(p => new RegisterViewModel
                    {
                        UserID = p.Id,
                        Firstname = p.FirstName,
                        Surname = p.LastName,
                        Email = p.EmailAddress
                    }).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUsers : {e.Message}");
                return new List<RegisterViewModel>();
            }
        }

        public async Task<RegisterViewModel> GetUser(Guid userID)
        {
            try
            {
                return (await GetUsers()).FirstOrDefault(p => p.UserID == userID);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUser : {e.Message}");
                return null;
            }
        }

        public async Task<bool> AssignUsertoRole(string role, Guid userId)
        {
            var userRoles = await GetUserRolesAsync(userId);
            if (!userRoles.Contains(role))
            {
                var user = await GetUserById(userId);
                var theRole = await _dbContext.Roles.FirstOrDefaultAsync(p => p.Name == role);

                try
                {
                    _dbContext.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = theRole.Id
                    });
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError($"AssignUsertoRole : {e.Message}");
                    return false;
                }                               
            }

            return true;
        }

        public async Task RemoveRoleFromUser(Guid userId, string roleName)
        {
            try
            {
                var userRoles = await GetUserRolesAsync(userId);

                if (userRoles.Contains(roleName))
                {
                    var user =  await GetUserById(userId);

                    if (user != null)
                    {
                        var role = await _dbContext.Roles.FirstOrDefaultAsync(p => p.Name == roleName);

                        if (role != null)
                        {
                            var userRole = await _dbContext.UserRoles.FirstOrDefaultAsync(p => p.UserId == user.Id && p.RoleId == role.Id);

                            if (userRole != null)
                            {                            
                                _dbContext.UserRoles.Remove(userRole);
                                await _dbContext.SaveChangesAsync();
                            }
                        }    
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"RemoveRoleFromUser : {e.Message}");
            }
        }

        public async Task<IdentityResult> AddUserToServer(User newUser)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(newUser.Password))
                    newUser.Password = _encryption.HashPassword(_mapper.MapIdentityUser(newUser), newUser.Password);
                newUser.UserName = newUser.EmailAddress;
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                _logger.LogError($"AddUserToServer : {e.Message}");
                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            try
            {
                var aUser = await _dbContext.Users.FirstOrDefaultAsync(p => p.Id == user.Id);

                if (aUser != null)
                {
                    aUser.EmailAddress = user.EmailAddress;
                    aUser.Password = user.Password;
                    aUser.LastName = user.LastName;
                    aUser.FirstName = user.FirstName;
                    aUser.IsActive = true;

                    await _dbContext.SaveChangesAsync();
                }

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateUserAsync : {e.Message}");
                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> RemoveUserByUserId(Guid userId)
        {
            try
            {
                var user = await GetUserById(userId);
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                _logger.LogError($"RemoveUserByUserId : {e.Message}");
                return IdentityResult.Failed();
            }
        }
    }
}
