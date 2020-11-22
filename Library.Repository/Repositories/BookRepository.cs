using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Library.DataBase;
using Library.DataCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DBContext _dbContext;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public BookRepository(ILogger<BookRepository> logger, IMapper mapper) {
            _dbContext = new DBContext(new DbContextOptions<DBContext>());
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> Subscribe(Guid bookID, Guid userID)
        {
            try
            {
                var userBook = await _dbContext.UserBooks.FirstOrDefaultAsync(p => p.BookID == bookID && p.UserID == userID);

                if (userBook == null)
                {
                    userBook = new UserBook
                    {
                        BookID = bookID,
                        UserID = userID
                    };

                    await _dbContext.UserBooks.AddAsync(userBook);
                    await _dbContext.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Subscribe : {e.Message}");
            }

            return false;
        }

        public async Task<bool> UnSubscribe(Guid bookID, Guid userID)
        {
            try
            {
                var userBook = await _dbContext.UserBooks.FirstOrDefaultAsync(p => p.BookID == bookID && p.UserID == userID);

                if (userBook != null)
                {
                    _dbContext.UserBooks.Remove(userBook);
                    await _dbContext.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"UnSubscribe : {e.Message}");
            }

            return false;
        }

        public async Task<List<LibraryBook>> GetAllBooks()
        {
            try
            {
                var books = await _dbContext.Books.OrderBy(p => p.Name).ToListAsync();

                return books.Select(p => _mapper.Map(p)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"GetAllBooks : {e.Message}");
            }

            return new List<LibraryBook>();
        }

        public async Task<bool> SaveBook(LibraryBook book)
        {
            try
            {
                if (book != null)
                {
                    Book dbBook = _mapper.Map(book);

                    if (await _dbContext.Books.FirstOrDefaultAsync(p => p.BookID == dbBook.BookID) == null)
                        await _dbContext.Books.AddAsync(dbBook);
                    else
                        _dbContext.Books.Update(dbBook);

                    await _dbContext.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"SaveBook : {e.Message}");
            }

            return false;
        }

        public async Task<LibraryBook> GetBookByID(Guid bookID)
        {
            try
            {
                var book = await _dbContext.Books.FirstOrDefaultAsync(p => p.BookID == bookID);

                if (book != null)
                {
                    return _mapper.Map(book);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetBookByID : {e.Message}");
            }

            return null;
        }

        public async Task<List<LibraryBook>> GetUserBooks(Guid userID)
        {
            try
            {
                var books = await _dbContext.Books.ToListAsync();
                var subbed = await _dbContext.UserBooks.Where(p => p.UserID == userID).ToListAsync();

                return books.Select(p => _mapper.Map(p))
                    .Select(p =>
                    {
                        p.IsSubscribed = subbed.FirstOrDefault(r => r.BookID == p.BookID) != null;
                        return p;
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserBooks : {e.Message}");
            }

            return new List<LibraryBook>();
        }

        public async Task<bool> IsSubscribed(Guid bookID, Guid userID)
        {
            try
            {
                var userBook = await _dbContext.UserBooks.FirstOrDefaultAsync(p => p.BookID == bookID && p.UserID == userID);

                return userBook != null;
            }
            catch (Exception e)
            {
                _logger.LogError($"IsSubscribed : {e.Message}");
            }

            return false;
        }
    }
}
