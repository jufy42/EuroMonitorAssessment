using System.Threading;
using System.Threading.Tasks;

namespace Library.ADT
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IBookRepository BookRepository { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
