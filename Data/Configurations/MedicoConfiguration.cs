using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class MedicoConfiguration : IEntityTypeConfiguration<Medico>
    {
        public void Configure(EntityTypeBuilder<Medico> builder)
        {
            builder.ToTable("Medicos");
            builder.HasKey(m => m.MedicoId);
            builder.Property(m => m.Nombres).HasMaxLength(100).IsRequired();
            builder.Property(m => m.ApellidoPaterno).HasMaxLength(50).IsRequired();
            builder.Property(m => m.ApellidoMaterno).HasMaxLength(50);
            builder.HasIndex(m => m.CMP).IsUnique();
            builder.Property(m => m.CMP).HasMaxLength(20).IsRequired();
            builder.Property(m => m.Activo).HasDefaultValue(true);
        }
    }
}
