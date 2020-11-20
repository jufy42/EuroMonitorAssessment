using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Library.DataCore
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public bool MarkedForDeletion { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserBook> UserBooks { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
            IsLocked = false;
            IsActive = false;
            MarkedForDeletion = false;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
