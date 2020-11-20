using System;
using System.Threading.Tasks;
using Moq;
using Library.ADT;
using Library.Core;
using Library.DataCore;
using Library.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Library.Test
{
    public class AccountService_Test
    {
        private Mock<IRepositoryManager> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private IAccountService _accountService;

        private void BuildRepo(bool emptyuser = false)
        {
            _mockRepo = new Mock<IRepositoryManager>();
            _mockMapper = new Mock<IMapper>();

            _mockRepo.Setup(p => p.UserRepository.GetUserByEmailAddress(It.IsAny<string>())).ReturnsAsync(() => (emptyuser ? null : new User()));
            _mockRepo.Setup(p => p.UserRepository.GetUserById(It.IsAny<Guid>())).ReturnsAsync(() => (emptyuser ? null : new User()));

            _mockRepo.Setup(p => p.UserRepository.ValidateUser(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() =>
                (emptyuser ? new ValidationResult {ValidationResourceString = "Common_Error_InvalidEmailAddressOrPassword", IsValid = false} : new ValidationResult {IsValid = true}));

            _mockMapper.Setup(p => p.Map(It.IsAny<RegisterViewModel>())).Returns(() => new User());
            _mockRepo.Setup(p => p.UserRepository.AddUserToServer(It.IsAny<User>())).ReturnsAsync(() => IdentityResult.Success);

            _accountService = new AccountService(_mockRepo.Object, _mockMapper.Object, new NullLogger<AccountService>());
        }

        [Fact]
        public async Task RegisterUser_UserDoesnotExist_Registered()
        {
            BuildRepo(true);

            var response = await _accountService.RegisterUser(new RegisterViewModel{Email = ""});

            Assert.True(response);
        }

        [Fact]
        public async Task RegisterUser_UserDoesExist_NotRegistered()
        {
            BuildRepo();

            var response = await _accountService.RegisterUser(new RegisterViewModel{Email = ""});

            Assert.False(response);
        }

        [Fact]
        public async Task ConfirmUserExists_UserExists_Exists()
        {
            BuildRepo();

            var response = await _accountService.ConfirmUserExists(Guid.NewGuid());

            Assert.True(response);
        }

        [Fact]
        public async Task ConfirmUserExists_UserdoesnotExist_DoesnotExist()
        {
            BuildRepo(true);

            var response = await _accountService.ConfirmUserExists(Guid.NewGuid());

            Assert.False(response);
        }

        [Fact]
        public async Task ConfirmUserExists_EmptyGuid_DoesnotExist()
        {
            BuildRepo(true);

            var response = await _accountService.ConfirmUserExists(Guid.Empty);

            Assert.False(response);
        }
    }
}