using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class UPSConfiguration : IEntityTypeConfiguration<UPS>
    {
        public void Configure(EntityTypeBuilder<UPS> builder)
        {
            builder.ToTable("UPS");
            builder.HasKey(u => u.UPSId);
            builder.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
            builder.Property(u => u.Activa).HasDefaultValue(true);
        }
    }
}
