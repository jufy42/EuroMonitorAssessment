using System;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Library.Core
{
    public class SystemUser : IdentityUser<Guid>
    {
        public bool IsDeleted { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsLocked { get; set; }

        public bool DoRememberMe { get; set; }

        public SystemUser()
        {
            Id = Guid.NewGuid();
        }

        public SystemUser(Guid id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            output.AppendFormat("IdentyUser: {0} ({1})", UserName, Id);

            return output.ToString();
        }
    }
}
