using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Library.DataCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.DataBase
{
    public class DBContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>,UserRole,IdentityUserLogin<Guid>,IdentityRoleClaim<Guid>,UserToken>
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder.UseSqlServer(@"Server=localhost;Initial Catalog=Library;Integrated Security=True;MultipleActiveResultSets=True;");
#endif
#if TEST
                optionsBuilder.UseSqlServer(@"Server=localhost;Initial Catalog=Library_TEST;Integrated Security=True;MultipleActiveResultSets=True;");
#endif
#if RELEASE
                optionsBuilder.UseSqlServer(@"Server=localhost;Initial Catalog=Library;Integrated Security=True;MultipleActiveResultSets=True;");
#endif
            }
        }   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());            
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());    
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new UserBookConfiguration());         
            modelBuilder.ApplyConfiguration(new UserTokenConfiguration());

            modelBuilder.Ignore<IdentityUserToken<Guid>>();
            modelBuilder.Ignore<IdentityUserClaim<Guid>>();
            modelBuilder.Ignore<IdentityUserLogin<Guid>>();
            modelBuilder.Ignore<IdentityRoleClaim<Guid>>();
        }

        public override int SaveChanges()
        {
            using var scope = Database.BeginTransaction();
            int result = base.SaveChanges();
            scope.Commit();
            return result;
        }

        public async Task<int> SaveChangesAsync()
        {
            await using var scope = await Database.BeginTransactionAsync();
            var validationResults = new List<ValidationResult>();
            ChangeTracker.Entries().Where(m => m.State == EntityState.Added || m.State == EntityState.Modified)
                .ToList().ForEach(
                    m =>
                    {
                        if (!Validator.TryValidateObject(m, new ValidationContext(m), validationResults))
                        {
                            //var errors = validationResults.Select(p => string.Join(',', p.MemberNames) + ": " + p.ErrorMessage).ToList();
                            //_logger.LogError($"SaveChangesAsync : {errors}");
                        }

                        var validationContext = new ValidationContext(m);
                        Validator.ValidateObject(m, validationContext);
                    });

            if (validationResults.Any())
            {
                //var errors = validationResults.Select(p => string.Join(',', p.MemberNames) + ": " + p.ErrorMessage).ToList();
                //_logger.LogError($"SaveChangesAsync : {errors}");
            }

            int result = await base.SaveChangesAsync();
            await scope.CommitAsync();
            return result;
        }
    }
}
