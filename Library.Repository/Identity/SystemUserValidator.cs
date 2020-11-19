using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Library.Core;
using Microsoft.AspNetCore.Identity;

namespace Library.Repository
{
    public class SystemUserValidator<TUser> : IUserValidator<TUser> where TUser : SystemUser
    {
        private readonly IUserStore<TUser> _userStore;

        public SystemUserValidator(IUserStore<TUser> userStore)
        {
            _userStore = userStore;
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var errors = new List<IdentityError>();
            try
            {
                var userCheck = await _userStore.FindByNameAsync(user.UserName, CancellationToken.None);
                if (userCheck != null && userCheck.Id != user.Id)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Username is already in use."
                    });
                }
            }
            catch (Exception ex)
            {
                errors.Add(new IdentityError
                {
                    Description = ex.Message
                });
            }
            return errors.Any()
                ? IdentityResult.Failed(errors.ToArray())
                : IdentityResult.Success;
        }
    }
}
