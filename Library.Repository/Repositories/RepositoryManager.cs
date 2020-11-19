using System.Threading;
using System.Threading.Tasks;
using Library.ADT;
using Library.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly DBContext _context;

        public IUserRepository UserRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IBookRepository BookRepository { get; }

        public RepositoryManager()
        {
            _context = new DBContext(new DbContextOptions<DBContext>());
        }

        public RepositoryManager(IUserRepository userRepository, IRoleRepository roleRepository, IBookRepository bookRepository)
        {
            _context = new DBContext(new DbContextOptions<DBContext>());

            UserRepository = userRepository;
            RoleRepository = roleRepository;
            BookRepository = bookRepository;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            int result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
