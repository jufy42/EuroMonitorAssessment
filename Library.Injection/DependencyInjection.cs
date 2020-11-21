using Autofac;
using Library.ADT;
using Library.Core;
using Library.Repository;
using Library.Service;
using Microsoft.AspNetCore.Identity;

namespace Library.Injection
{
    public class DependencyInjection
    {
        public void BuildDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<SystemRoleStore>().As<IRoleStore<SystemRole>>();
            builder.RegisterType<SystemUserStore>().As<IUserStore<SystemUser>>();

            builder.RegisterType<RepositoryManager>().As<IRepositoryManager>();
            builder.RegisterType<RoleRepository>().As<IRoleRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<BookRepository>().As<IBookRepository>().InstancePerLifetimeScope();
            
            builder.RegisterType<PasswordHasher<SystemUser>>().As<IPasswordHasher<SystemUser>>();

            builder.RegisterType<Mapper>().As<IMapper>();

            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterType<BookService>().As<IBookService>();
        }
    }
}
