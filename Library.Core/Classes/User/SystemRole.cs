using Microsoft.AspNetCore.Identity;
using System;

namespace Library.Core
{
    public sealed class SystemRole : IdentityRole<Guid>
    {
        public SystemRole()
        {
            Id = Guid.NewGuid();
        }

        public SystemRole(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
