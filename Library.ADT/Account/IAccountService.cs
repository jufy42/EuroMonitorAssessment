using System;
using System.Threading.Tasks;
using Library.Core;

namespace Library.ADT.Account
{
    public interface IAccountService
    {
        Task<bool> RegisterUser(RegisterViewModel viewModel);
        Task<bool> ConfirmUserExists(Guid userId);
        Task<string> SignInUser(SystemSignInManager<SystemUser> signInManager, LoginViewModel viewModel);
        Task SignOutUser(SystemSignInManager<SystemUser> signInManager);
    }
}
