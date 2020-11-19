using Library.DataCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataBase
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(c => new {c.UserId, c.RoleId});

            builder
                .HasOne(p => p.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(pt => pt.UserId);

            builder
                .HasOne(p => p.Role)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(pt => pt.RoleId);

            builder
                .HasIndex(p => p.RoleId);

            builder
                .HasIndex(p => p.UserId);
        }
    }
}
