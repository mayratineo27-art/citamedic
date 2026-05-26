using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class ProgramacionOperativaConfiguration : IEntityTypeConfiguration<ProgramacionOperativa>
    {
        public void Configure(EntityTypeBuilder<ProgramacionOperativa> builder)
        {
            builder.ToTable("ProgramacionesOperativas");
            builder.HasKey(p => p.ProgramacionId);

            builder.Property(p => p.Turno).HasConversion<int>().IsRequired();
            builder.Property(p => p.Fecha).HasColumnType("date");
            builder.Property(p => p.Habilitada).HasDefaultValue(false);
            builder.Property(p => p.FechaCreacion).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(p => p.Especialidad)
                .WithMany(e => e.Programaciones)
                .HasForeignKey(p => p.EspecialidadId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Medico)
                .WithMany(m => m.Programaciones)
                .HasForeignKey(p => p.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.CreadaPorUsuario)
                .WithMany()
                .HasForeignKey(p => p.CreadaPorUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
