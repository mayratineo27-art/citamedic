using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class EspecialidadConfiguration : IEntityTypeConfiguration<Especialidad>
    {
        public void Configure(EntityTypeBuilder<Especialidad> builder)
        {
            builder.ToTable("Especialidades");
            builder.HasKey(e => e.EspecialidadId);
            builder.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Activa).HasDefaultValue(true);

            builder.HasOne(e => e.UPS)
                .WithMany(u => u.Especialidades)
                .HasForeignKey(e => e.UPSId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
