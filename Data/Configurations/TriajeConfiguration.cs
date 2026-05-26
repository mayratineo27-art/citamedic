using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class TriajeConfiguration : IEntityTypeConfiguration<Triaje>
    {
        public void Configure(EntityTypeBuilder<Triaje> builder)
        {
            builder.ToTable("Triajes");
            builder.HasKey(t => t.TriajeId);

            builder.HasIndex(t => t.CitaId).IsUnique();

            builder.Property(t => t.Peso).HasColumnType("decimal(5,2)");
            builder.Property(t => t.Talla).HasColumnType("decimal(5,2)");
            builder.Property(t => t.Temperatura).HasColumnType("decimal(4,1)");
            builder.Property(t => t.Observacion).HasMaxLength(500);
            builder.Property(t => t.FechaRegistro).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(t => t.Cita)
                .WithOne(c => c.Triaje)
                .HasForeignKey<Triaje>(t => t.CitaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.EnfermeriaUsuario)
                .WithMany()
                .HasForeignKey(t => t.EnfermeriaUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
