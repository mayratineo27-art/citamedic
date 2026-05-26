using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class AvisoAtencionInmediataConfiguration : IEntityTypeConfiguration<AvisoAtencionInmediata>
    {
        public void Configure(EntityTypeBuilder<AvisoAtencionInmediata> builder)
        {
            builder.ToTable("AvisosAtencionInmediata");
            builder.HasKey(a => a.AvisoId);

            builder.Property(a => a.Motivo).HasMaxLength(300).IsRequired();
            builder.Property(a => a.EstadoAviso).HasConversion<int>().HasDefaultValue(EstadoAviso.Pendiente);
            builder.Property(a => a.FechaEnvio).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(a => a.Paciente)
                .WithMany()
                .HasForeignKey(a => a.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
