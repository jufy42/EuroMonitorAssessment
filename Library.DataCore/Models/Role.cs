using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Library.DataCore
{
    public sealed class Role : IdentityRole<Guid>
    {
        public ICollection<UserRole> UserRoles { get; set; }

        public Role()
        {
            Id = Guid.NewGuid();
        }
    }
}
