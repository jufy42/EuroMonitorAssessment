﻿using Library.DataCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataBase
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(c => c.BookID);

            builder.Property(e => e.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar(250)")
                .IsRequired();

            builder.Property(e => e.Text)
                .HasColumnName("Text")
                .HasColumnType("nvarchar(4000)");

            builder.Property(e => e.PurchasePrice)
                .HasColumnName("PurchasePrice")
                .HasColumnType("float")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(e => e.ImageName)
                .HasColumnName("ImageName")
                .HasColumnType("nvarchar(1000)");

            builder.Property(e => e.Author)
                .HasColumnName("Author")
                .HasColumnType("nvarchar(250)");

            builder.Property(e => e.Active)
                .HasColumnName("Active")
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
