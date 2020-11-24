using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Core;

namespace Library.ADT
{
    public interface IBookService
    {
        Task<string> Subscribe(Guid bookID, Guid userID);
        Task<string> UnSubscribe(Guid bookID, Guid userID);
        Task<LibraryBook> GetBookByID(Guid bookID, Guid? userID = null);
        Task<BookList> GetBooks(BookList bookList, Guid? userID = null);
        Task<List<LibraryBook>> GetBooks(Guid? userID = null);
        Task<bool> SaveBook(LibraryBook book);
        Task<bool> RemoveBook(Guid bookID);
    }
}