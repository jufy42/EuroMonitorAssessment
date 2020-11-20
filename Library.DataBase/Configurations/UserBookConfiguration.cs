using Library.DataCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataBase
{
    public class UserBookConfiguration : IEntityTypeConfiguration<UserBook>
    {
        public void Configure(EntityTypeBuilder<UserBook> builder)
        {
            builder.ToTable("UserBooks");

            builder.HasKey(c => new {c.UserID, c.BookID});

            builder
                .HasOne(p => p.User)
                .WithMany(p => p.UserBooks)
                .HasForeignKey(pt => pt.UserID);

            builder
                .HasOne(p => p.Book)
                .WithMany(p => p.UserBooks)
                .HasForeignKey(pt => pt.BookID);

            builder
                .HasIndex(p => p.BookID);

            builder
                .HasIndex(p => p.UserID);
        }
    }
}
