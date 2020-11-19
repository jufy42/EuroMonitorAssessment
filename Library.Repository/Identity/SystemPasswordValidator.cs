using System;
using System.Threading.Tasks;
using Library.Core;
using Microsoft.AspNetCore.Identity;

namespace Library.Repository
{
    public class SystemPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : SystemUser
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (string.Equals(user.UserName, password, StringComparison.OrdinalIgnoreCase))
            {
                return await Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "UsernameAsPassword",
                    Description = "You cannot use your username as your password"
                }));
            }
            return await Task.FromResult(IdentityResult.Success);
        }
    }
}
