using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
    {
        public void Configure(EntityTypeBuilder<Paciente> builder)
        {
            builder.ToTable("Pacientes");
            builder.HasKey(p => p.PacienteId);

            // 1:1 con Usuario
            builder.HasIndex(p => p.UsuarioId).IsUnique();

            builder.Property(p => p.DNI).HasMaxLength(8).IsRequired();
            builder.Property(p => p.Nombres).HasMaxLength(100).IsRequired();
            builder.Property(p => p.ApellidoPaterno).HasMaxLength(50).IsRequired();
            builder.Property(p => p.FechaNacimiento).HasColumnType("date");

            // Relación con Usuario
            builder.HasOne(p => p.Usuario)
                .WithOne(u => u.Paciente)
                .HasForeignKey<Paciente>(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auto-referencia Responsable → Dependientes (RN03)
            builder.HasOne(p => p.Responsable)
                .WithMany(p => p.Dependientes)
                .HasForeignKey(p => p.ResponsableId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
