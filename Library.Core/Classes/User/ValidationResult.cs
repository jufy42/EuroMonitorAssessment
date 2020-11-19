using System;

namespace Library.Core
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ValidationResourceString { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }

        public ValidationResult()
        {
            IsValid = true;
        }
    }
}
