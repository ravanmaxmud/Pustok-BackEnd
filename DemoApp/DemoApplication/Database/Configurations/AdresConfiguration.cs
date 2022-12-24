using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DemoApplication.Database.Models;

namespace DemoApplication.Database.Configurations
{
    public class AdresConfiguration : IEntityTypeConfiguration<Addres>
    {
        public void Configure(EntityTypeBuilder<Addres> builder)
        {
            builder
               .ToTable("Address");
        }
    }
}
