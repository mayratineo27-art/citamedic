using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class HistorialEstadoCitaConfiguration : IEntityTypeConfiguration<HistorialEstadoCita>
    {
        public void Configure(EntityTypeBuilder<HistorialEstadoCita> builder)
        {
            builder.ToTable("HistorialEstadosCita");
            builder.HasKey(h => h.HistorialId);

            builder.Property(h => h.EstadoAnterior).HasConversion<int>();
            builder.Property(h => h.EstadoNuevo).HasConversion<int>();
            builder.Property(h => h.FechaCambio).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(h => h.Observacion).HasMaxLength(300);

            builder.HasOne(h => h.Cita)
                .WithMany(c => c.Historial)
                .HasForeignKey(h => h.CitaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
