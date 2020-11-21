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

namespace Library.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContext _dbContext;
        private readonly IPasswordHasher<SystemUser> _encryption;
        private readonly IMapper _mapper;

        public UserRepository(IPasswordHasher<SystemUser> encryption, IMapper mapper)
        {
            _dbContext = new DBContext(new DbContextOptions<DBContext>());
            _encryption = encryption;
            _mapper = mapper;
        }

        public async Task<ValidationResult> ValidateUser(string emailAddress, string password)
        {
            var validationResult = new ValidationResult();

            var user = await GetUserByEmailAddress(emailAddress);

            if (user == null)
            {
                validationResult.ValidationResourceString = "Invalid Email Address or Password";
                validationResult.IsValid = false;
            }

            if (user != null)
            {
                var passwordVerifiedResult = _encryption.VerifyHashedPassword(_mapper.MapIdentityUser(user),user.Password, password);

                if (passwordVerifiedResult != PasswordVerificationResult.Success)
                {
                    validationResult.ValidationResourceString = "Invalid Email Address or Password";
                    validationResult.IsValid = false;
                }
                else if (user.IsLocked)
                {
                    validationResult.ValidationResourceString = "User Is Locked Out";
                    validationResult.IsValid = false;
                }
                else
                {
                    validationResult.UserId = user.Id;
                    validationResult.UserName = user.ToString();
                }
            }

            return validationResult;
        }
        
        public async Task<User> GetUserById(Guid id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == id);

            return user;
        }

        public async Task<User> GetUserByEmailAddress(string emailAddress)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(p =>
                    p.EmailAddress.ToLower() == (emailAddress ?? "").ToLower());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> GetPasswordAsync(Guid identityUserId)
        {
            var user = await GetUserById(identityUserId);
            return user.Password;
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            try
            {
                var user = await GetUserById(userId);

                if (user == null) return new List<string>();

                var userRoles = await _dbContext.UserRoles.Where(p => p.UserId == user.Id).Select(p => p.RoleId).ToListAsync();
                
                var roles = await _dbContext.Roles.Where(p => userRoles.Contains(p.Id)).ToListAsync();

                return roles == null ? new List<string>() : roles.Select(p => p.Name).ToList();            
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<bool> CheckIfUserInRoleAsync(Guid userId, string roleName)
        {
            var userRoles = await GetUserRolesAsync(userId);
            return userRoles.Contains(roleName);
        }

        public async Task<List<RegisterViewModel>> GetUsers()
        {
            var users = await _dbContext.Users.ToListAsync();

            return users
                .Select(p => new RegisterViewModel
                {
                    UserID = p.Id,
                    Firstname = p.FirstName,
                    Surname = p.LastName,
                    Email = p.EmailAddress,                    
                    Administrator = _dbContext.Roles
                        .Join(_dbContext.UserRoles.Where(r => r.UserId == p.Id), r => r.Id, ur => ur.RoleId,
                            (r, ur) => new {r.Name}).Any(r => r.Name == Global.ROLE_ADMINISTRATOR)
                }).ToList();
        }

        public async Task<RegisterViewModel> GetUser(Guid userID)
        {
            return (await GetUsers()).FirstOrDefault(p => p.UserID == userID);
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
                    user.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = theRole.Id
                    });
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }                               
            }

            return true;
        }

        public async Task<IdentityResult> AddUserToServer(User newUser)
        {
            if (!string.IsNullOrWhiteSpace(newUser.Password))
                newUser.Password = _encryption.HashPassword(_mapper.MapIdentityUser(newUser), newUser.Password);
            newUser.UserName = newUser.EmailAddress;
            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveUserByUserId(Guid userId)
        {
            var user = await GetUserById(userId);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task RemoveRoleFromUser(Guid userId, string roleName)
        {
            var userRoles = await GetUserRolesAsync(userId);

            if (userRoles.Contains(roleName))
            {
                var user =  await GetUserById(userId);
                var theRole = await _dbContext.Roles.FirstOrDefaultAsync(p => p.Name == roleName);

                if (user != null)
                {
                    user.UserRoles.Remove(await _dbContext.UserRoles.FirstOrDefaultAsync(p => p.UserId == userId && p.RoleId == theRole.Id));
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
