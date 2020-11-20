using System;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Microsoft.Extensions.Logging;

namespace Library.Service
{
    public class AccountService : IAccountService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AccountService(IRepositoryManager repositoryManager, IMapper mapper, ILogger<AccountService> logger)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> RegisterUser(RegisterViewModel viewModel)
        {
            var currentUser = await _repositoryManager.UserRepository.GetUserByEmailAddress(viewModel.Email);
            if (currentUser == null)
            {
                var user = _mapper.Map(viewModel);
                await _repositoryManager.UserRepository.AddUserToServer(user);
                
                if (viewModel.Administrator)                
                    await _repositoryManager.UserRepository.AssignUsertoRole(Global.ROLE_ADMINISTRATOR, user.Id);
                return true;
            }
            return false;
        }

        public async Task<bool> ConfirmUserExists(Guid userId)
        {
            var user = await _repositoryManager.UserRepository.GetUserById(userId);
            return user != null;
        }

        public async Task<string> SignInUser(SystemSignInManager<SystemUser> signInManager, LoginViewModel viewModel)
        {
            try
            {            
                var validationResult = await _repositoryManager.UserRepository.ValidateUser(viewModel.Email, viewModel.Password);

                if (validationResult.IsValid)
                {                
                    await signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, false);
                    return string.Empty;                
                }
                return validationResult.ValidationResourceString;
            }
            catch (Exception e)
            {
                _logger.LogError("AccountService - SignInUser : {0}", e.Message);
                return new ValidationResult().ValidationResourceString;
            }
        }

        public async Task SignOutUser(SystemSignInManager<SystemUser> signInManager)
        {
            await signInManager.SignOutAsync();
        }        
    }
}
