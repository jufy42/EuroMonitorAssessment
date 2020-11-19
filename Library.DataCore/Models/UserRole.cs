using System;
using Microsoft.AspNetCore.Identity;

namespace Library.DataCore
{
    public sealed class UserRole : IdentityUserRole<Guid>
    {        
        public User User { get; set; }     
        public Role Role { get; set; }
    }
}
