using Library.Core;
using Library.DataCore;

namespace Library.ADT
{
    public interface IMapper
    {
        User Map(RegisterViewModel viewModel);
        User MapUser(SystemUser identityUser);
        SystemUser MapIdentityUser(User user);
        SystemRole GetIdentityRole(Role role);
        Role GetRole(SystemRole identityRole);
        LibraryBook Map(Book book);
        Book Map(LibraryBook book);
    }
}
