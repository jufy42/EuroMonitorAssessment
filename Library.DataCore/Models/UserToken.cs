using System;
using Microsoft.AspNetCore.Identity;

namespace Library.DataCore
{
    public class UserToken : IdentityUserToken<Guid>
    {
        public Guid TokenID { get; set; }
    }
}
