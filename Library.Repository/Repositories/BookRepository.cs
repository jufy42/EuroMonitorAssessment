using Library.ADT;
using Library.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DBContext _dbContext; 

        public BookRepository() {
            _dbContext = new DBContext(new DbContextOptions<DBContext>());
        }
    }
}
