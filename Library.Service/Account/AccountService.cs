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

        public async Task<bool> IsReseller(Guid userID)
        {
            try
            {
                var roles = await _repositoryManager.UserRepository.GetUserRolesAsync(userID);

                return roles.Contains(Global.ROLE_ADMINISTRATOR) || roles.Contains(Global.ROLE_RESELLER);
            }
            catch (Exception e)
            {
                _logger.LogError($"IsReseller : {e.Message}");
            }

            return false;
        }

        public async Task<SystemUser> GetUserbyID(Guid userID)
        {
            try
            {
                var user = await _repositoryManager.UserRepository.GetUserById(userID);
                return _mapper.MapIdentityUser(user);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserbyID : {e.Message}");
            }

            return null;
        }

        public async Task<bool> RegisterUser(RegisterViewModel viewModel)
        {
            var currentUser = await _repositoryManager.UserRepository.GetUserByEmailAddress(viewModel.Email);
            if (currentUser == null)
            {
                var user = _mapper.Map(viewModel);
                await _repositoryManager.UserRepository.AddUserToServer(user);
                
                if (viewModel.Reseller)                
                    await _repositoryManager.UserRepository.AssignUsertoRole(Global.ROLE_RESELLER, user.Id);
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
                _logger.LogError($"SignInUser : {e.Message}");
                return new ValidationResult().ValidationResourceString;
            }
        }

        public async Task<Guid?> ValidateUser(string username, string password)
        {
            try
            {
                var validationResult =  await _repositoryManager.UserRepository.ValidateUser(username, password);

                if (validationResult.IsValid)
                {
                    var user = await _repositoryManager.UserRepository.GetUserByEmailAddress(username);

                    if (user != null)
                    {
                        return user.Id;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ValidateUser : {e.Message}");
            }

            return null;
        }

        public async Task SignOutUser(SystemSignInManager<SystemUser> signInManager)
        {
            await signInManager.SignOutAsync();
        }        

        public async Task<bool> CheckUserName(Guid id, string emailAddress)
        {
            var emailMatch = await _repositoryManager.UserRepository.GetUserByEmailAddress(emailAddress);

            if (emailMatch == null) return false;

            return id != emailMatch.Id;
        }

        public async Task<SystemUser> GetUserbyEmail(string emailAddress)
        {
            try
            {
                var user = await _repositoryManager.UserRepository.GetUserByEmailAddress(emailAddress);
                return _mapper.MapIdentityUser(user);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserbyEmail : {e.Message}");
            }

            return null;
        }
    }
}
