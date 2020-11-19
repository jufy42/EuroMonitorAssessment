using Microsoft.EntityFrameworkCore;
using Library.DataCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataBase
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(e => e.Id);   
            
            builder.Ignore(c => c.ConcurrencyStamp)
                .Ignore(c => c.NormalizedName);
        }
    }
}
