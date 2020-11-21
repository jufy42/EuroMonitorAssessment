using System;
using System.Threading.Tasks;
using Library.Core;

namespace Library.ADT
{
    public interface IBookService
    {
        Task<string> Subscribe(Guid bookID, Guid userID);
        Task<string> UnSubscribe(Guid bookID, Guid userID);
        Task<LibraryBook> GetBookByID(Guid bookID, Guid? userID);
        Task<BookList> GetBooks(BookList bookList, Guid? userID);
        Task<bool> SaveBook(LibraryBook book);
    }
}