using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Microsoft.Extensions.Logging;

namespace Library.Service
{
    public class BookService : IBookService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger _logger;

        public BookService(IRepositoryManager repositoryManager, ILogger<BookService> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        public async Task<string> Subscribe(Guid bookID, Guid userID)
        {
            try
            {
                var check = await _repositoryManager.BookRepository.IsSubscribed(bookID, userID);

                if (check)
                    return "Already subscribed";

                var subscribe = await _repositoryManager.BookRepository.Subscribe(bookID, userID);

                if (!subscribe)
                    return "An error has occurred";
            }
            catch (Exception e)
            {
                _logger.LogError($"Subscribe : {e.Message}");
            }

            return "";
        }

        public async Task<string> UnSubscribe(Guid bookID, Guid userID)
        {
            try
            {
                var check = await _repositoryManager.BookRepository.IsSubscribed(bookID, userID);

                if (!check)
                    return "Not subscribed";

                var unsubscribe = await _repositoryManager.BookRepository.UnSubscribe(bookID, userID);

                if (!unsubscribe)
                    return "An error has occurred";
            }
            catch (Exception e)
            {
                _logger.LogError($"UnSubscribe : {e.Message}");
            }

            return "";
        }

        public async Task<LibraryBook> GetBookByID(Guid bookID, Guid? userID)
        {
            try
            {
                var book = await _repositoryManager.BookRepository.GetBookByID(bookID);

                if (book != null)
                {
                    if (userID != null)
                    {
                        var check = await _repositoryManager.BookRepository.IsSubscribed(bookID, userID ?? Guid.Empty);

                        book.IsSubscribed = check;
                    }

                    return book;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetBookByID : {e.Message}");
            }

            return null;
        }

        public async Task<List<LibraryBook>> GetBooks(Guid? userID)
        {
            try
            {
                var books = userID == null ? await _repositoryManager.BookRepository.GetAllBooks() : await _repositoryManager.BookRepository.GetUserBooks(userID ?? Guid.Empty);

                return books.OrderByDescending(p => p.IsSubscribed).ThenBy(p => p.Name).ThenBy(p => p.PurchasePrice).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"GetBooks : {e.Message}");
            }

            return new List<LibraryBook>();
        }

        public async Task<BookList> GetBooks(BookList bookList, Guid? userID)
        {
            var newBookList = new BookList
            {
                ItemsPerPage = bookList.ItemsPerPage == 0 ? 20 : bookList.ItemsPerPage,
                PageNo = bookList.PageNo == 0 ? 1 : bookList.PageNo,
                Search = bookList.Search ?? ""
            };

            try
            {
                var books = userID == null ? await _repositoryManager.BookRepository.GetAllBooks() : await _repositoryManager.BookRepository.GetUserBooks(userID ?? Guid.Empty);

                newBookList.Books = books.Where(p => (p.Name ?? "").ToLower().Contains((newBookList.Search ?? "").ToLower().Trim())).ToList().OrderByDescending(p => p.IsSubscribed).ThenBy(p => p.Name).ThenBy(p => p.PurchasePrice)
                    .Skip((newBookList.PageNo - 1) * newBookList.ItemsPerPage).Take(newBookList.ItemsPerPage).ToList();
                newBookList.NoPages = books.Distinct().Count() / newBookList.ItemsPerPage + (books.Distinct().Count() % newBookList.ItemsPerPage > 0 ? 1 : 0);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetBooks : {e.Message}");
            }

            return newBookList;
        }

        public async Task<bool> SaveBook(LibraryBook book)
        {
            return await _repositoryManager.BookRepository.SaveBook(book);
        }
    }
}
