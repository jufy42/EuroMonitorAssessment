using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Core;
using Library.DataCore;
using Microsoft.AspNetCore.Identity;

namespace Library.ADT
{
    public interface IUserRepository
    {
        Task<ValidationResult> ValidateUser(string emailAddress, string password);
        Task<User> GetUserByEmailAddress(string emailAddress);
        Task<IdentityResult> AddUserToServer(User newUser);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<IdentityResult> RemoveUserByUserId(Guid userId);
        Task<string> GetPasswordAsync(Guid identityUserId);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        Task<bool> CheckIfUserInRoleAsync(Guid userId, string roleName);
        Task<User> GetUserById(Guid id);
        Task<List<RegisterViewModel>> GetUsers();
        Task<RegisterViewModel> GetUser(Guid userID);
        Task<bool> AssignUsertoRole(string role, Guid userId);        
        Task RemoveRoleFromUser(Guid userId, string roleName);
    }
}
