using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class CitaConfiguration : IEntityTypeConfiguration<Cita>
    {
        public void Configure(EntityTypeBuilder<Cita> builder)
        {
            builder.ToTable("Citas");
            builder.HasKey(c => c.CitaId);

            builder.Property(c => c.EstadoCita)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.OrigenReserva)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.FechaReserva)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.FechaUltimaActualizacion)
                .HasDefaultValueSql("GETUTCDATE()");

            // FK Paciente — RESTRICT
            builder.HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Especialidad)
                .WithMany()
                .HasForeignKey(c => c.EspecialidadId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK Slot — RESTRICT
            builder.HasOne(c => c.Slot)
                .WithMany(s => s.Citas)
                .HasForeignKey(c => c.SlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK Usuario registrador (nullable) — RESTRICT
            builder.HasOne(c => c.RegistradaPorUsuario)
                .WithMany()
                .HasForeignKey(c => c.RegistradaPorUsuarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Índice de rendimiento
            builder.HasIndex(c => c.PacienteId);
            builder.HasIndex(c => c.SlotId);
            builder.HasIndex(c => c.EstadoCita);

            builder.HasIndex(c => new { c.PacienteId, c.SlotId })
                .IsUnique()
                .HasFilter("[EstadoCita] IN (0, 1, 2)");
        }
    }
}
