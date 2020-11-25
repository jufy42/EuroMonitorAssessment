using System;
using System.Net;
using Library.ADT;
using Library.Core;
using Library.DataCore;

namespace Library.Repository
{
    public class Mapper : IMapper
    {
        public User Map(RegisterViewModel viewModel)
        {
            return new User
            {
                FirstName = viewModel.Firstname,
                LastName = viewModel.Surname,
                EmailAddress = viewModel.Email,
                Password = viewModel.Password
            };
        }

        public User MapUser(SystemUser identityUser)
        {
            if (identityUser == null)
                return null;

            var user = new User
            {
                EmailAddress = identityUser.UserName,
                Id = identityUser.Id,
                Password = identityUser.PasswordHash,
                LastName = identityUser.LastName,
                FirstName = identityUser.FirstName,
                IsLocked = identityUser.LockoutEnabled
            };
            return user;
        }

        public SystemUser MapIdentityUser(User user)
        {
            var identityUser = new SystemUser
            {
                Id = user.Id,
                PasswordHash = user.Password,
                UserName = user.EmailAddress,
                LockoutEnabled = user.IsLocked,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return identityUser;
        }


        public SystemRole GetIdentityRole(Role role)
        {
            var identityRole = new SystemRole()
            {
                Name = role.Name
            };

            return identityRole;
        }

        public Role GetRole(SystemRole identityRole)
        {
            var role = new Role
            {
                Name = identityRole.Name,
                Id = identityRole.Id
            };
            return role;
        }

        public LibraryBook Map(Book book)
        {
            if (book == null)
                return null;

            return new LibraryBook
            {
                BookID = book.BookID,
                Name = WebUtility.HtmlDecode(book.Name),
                Text = WebUtility.HtmlDecode(book.Text),
                PurchasePrice = book.PurchasePrice,
                ImageName = WebUtility.HtmlDecode(book.ImageName),
                Author = WebUtility.HtmlDecode(book.Author)
            };
        }

        public Book Map(LibraryBook book)
        {
            if (book == null)
                return null;

            return new Book
            {
                BookID = book.BookID == Guid.Empty ? Guid.NewGuid() : book.BookID,
                Name = WebUtility.HtmlEncode(book.Name),
                Text = WebUtility.HtmlEncode(book.Text),
                PurchasePrice = book.PurchasePrice,
                ImageName = WebUtility.HtmlDecode(book.ImageName),
                Author = WebUtility.HtmlDecode(book.Author)
            };
        }
    }
}
