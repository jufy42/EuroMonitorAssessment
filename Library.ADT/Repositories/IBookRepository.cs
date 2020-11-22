using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Core;

namespace Library.ADT
{
    public interface IBookRepository
    {
        Task<bool> Subscribe(Guid bookID, Guid userID);
        Task<bool> UnSubscribe(Guid bookID, Guid userID);
        Task<List<LibraryBook>> GetAllBooks();
        Task<bool> SaveBook(LibraryBook book);
        Task<LibraryBook> GetBookByID(Guid bookID);
        Task<List<LibraryBook>> GetUserBooks(Guid userID);
        Task<bool> IsSubscribed(Guid bookID, Guid userID);
    }
}
