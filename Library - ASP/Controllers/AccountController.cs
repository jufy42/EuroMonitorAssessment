using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Library___ASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly SystemSignInManager<SystemUser> _signInManager;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;
        private readonly UserManager<SystemUser> _userManager;

        public AccountController(
            SystemSignInManager<SystemUser> signInManager,
            IAccountService accountService,
            ILogger<AccountController> logger,
            UserManager<SystemUser> userManager
        )
        {
            _signInManager = signInManager;
            _accountService = accountService;
            _logger = logger;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Account");
        }        

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {           
            if (!ModelState.IsValid)
            {                
                _logger.LogInformation("Log in failed : {0}",string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage)));
                return View(model);
            }

            var validationString = await _accountService.SignInUser(_signInManager, model) ?? "";

            if (string.IsNullOrWhiteSpace(validationString))
            {                
                return RedirectToAction("Index", "Home");
            }
            
            TempData[Global.FAILURE_KEY] = validationString;            
            _logger.LogInformation("Log in failed : {0}",string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage)));
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[Global.FAILURE_KEY] = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage));
                ModelState.AddModelError("", string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage)));
                return View(model);
            }

            var user = await _accountService.GetUserbyEmail(model.Email);

            if (user == null)
            {
                TempData[Global.FAILURE_KEY] = "The email address does not exist. Please supply a different email address or contact support.";
                return View(model);
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            string scheme = Url.ActionContext.HttpContext.Request.Scheme;
            var url = Url.Action("ResetPassword", "Account", new {userID = user.Id, code}, scheme);

            return RedirectToAction("ForgotPasswordConfirmation", new {url});
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation(string url)
        {
            return View(new ResetPasswordViewModel
            {
                Token = url
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            _accountService.SignOutUser(_signInManager);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            ModelState.Remove("UserID");
            if (!ModelState.IsValid)
            {
                TempData[Global.FAILURE_KEY] = string.Join("<br/>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage));
                return View(viewModel);
            }

            try
            {
                if (await _accountService.CheckUserName(Guid.Empty, viewModel.Email))
                {
                    TempData[Global.FAILURE_KEY] = "Email Address already exists, please enter in a different one.";
                    return View(viewModel);
                }

                var success = await _accountService.RegisterUser(viewModel);

                if (success)
                {
                    var validationString = await _accountService.SignInUser(_signInManager, new LoginViewModel
                    {
                        Email = viewModel.Email,
                        Password = viewModel.Password
                    }) ?? "";

                    if (string.IsNullOrWhiteSpace(validationString))
                    {                
                        return RedirectToAction("Index", "Home");
                    }
                }

                TempData[Global.FAILURE_KEY] = "Something went wrong. Please try again or contact support";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create : {ex.Message}");
                return View("Error");
            }
        }

        public async Task<IActionResult> ResetPassword(Guid userID, string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code)) return RedirectToAction("ResetError");

                var user = await _accountService.GetUserbyID(userID);
                if (user == null) return RedirectToAction("ResetError");

                if (!await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", code)) return RedirectToAction("VerifyError");

                return View(new ResetPasswordViewModel
                {
                    UserID = userID,
                    Email = user.UserName,
                    Token = code
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"ResetPassword : {e.Message}");
                return RedirectToAction("ResetError");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[Global.FAILURE_KEY] = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage));
                ModelState.AddModelError("", string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage)));
                return View(model);
            }

            var user = await _accountService.GetUserbyEmail(model.Email);

            if (user == null)
            {
                TempData[Global.FAILURE_KEY] = "The email address does not exist. Please contact support.";
                return View(model);
            }

            try
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

                if (result == IdentityResult.Success)
                {
                    //An email is usually sent at this step
                    return RedirectToAction("ResetComplete");
                }

                TempData[Global.FAILURE_KEY] = string.Join("<br/>", result.Errors.Select(p => p.Description).ToList());
                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError($"ResetPassword : {e.Message}");
                TempData[Global.FAILURE_KEY] = "Something went wrong. Please try again or contact support.";
                return View(model);
            }
        }

        public IActionResult ResetError()
        {
            return View();
        }

        public IActionResult ResetComplete()
        {
            return View();
        }

        public IActionResult VerifyError()
        {
            return View();
        }
    }
}
