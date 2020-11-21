﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Library___ASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly SystemSignInManager<SystemUser> _signInManager;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        public AccountController(
            SystemSignInManager<SystemUser> signInManager,
            IAccountService accountService,
            ILogger<AccountController> logger
        )
        {
            _signInManager = signInManager;
            _accountService = accountService;
            _logger = logger;
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
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            _accountService.SignOutUser(_signInManager);
            return RedirectToAction("Login", "Account");
        }
    }
}
