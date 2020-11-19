using Library.DataCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataBase
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .HasColumnName("FirstName")
                .HasColumnType("nvarchar(250)")
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasColumnName("LastName")
                .HasColumnType("nvarchar(125)")
                .IsRequired();

            builder.Property(e => e.EmailAddress)
                .HasColumnName("EmailAddress")
                .HasColumnType("nvarchar(125)")
                .IsRequired();

            builder.Property(e => e.Password)
                .HasColumnName("Password")
                .HasColumnType("nvarchar(250)");

            builder.Property(e => e.IsLocked)
                .HasColumnName("IsLocked")
                .HasColumnType("bit")
                .IsRequired();

            builder.Property(e => e.MarkedForDeletion)
                .HasColumnName("MarkedForDeletion")
                .HasColumnType("bit")
                .IsRequired();

            builder.Ignore(c => c.AccessFailedCount)
                .Ignore(c => c.ConcurrencyStamp)
                .Ignore(c => c.Email)
                .Ignore(c => c.EmailConfirmed)
                .Ignore(c => c.LockoutEnabled)
                .Ignore(c => c.LockoutEnd)
                .Ignore(c => c.NormalizedEmail)
                .Ignore(c => c.NormalizedUserName)
                .Ignore(c => c.PasswordHash)
                .Ignore(c => c.PhoneNumber)
                .Ignore(c => c.PhoneNumberConfirmed)
                .Ignore(c => c.SecurityStamp)
                .Ignore(c => c.TwoFactorEnabled);
        }
    }
}
