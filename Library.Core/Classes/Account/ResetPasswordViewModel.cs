using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Core
{
    public class ResetPasswordViewModel
    {
        public Guid UserID { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation new password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}
