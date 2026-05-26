using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.HasKey(u => u.UsuarioId);

            builder.HasIndex(u => u.DNI).IsUnique();
            builder.Property(u => u.DNI).HasMaxLength(8).IsRequired();

            builder.HasIndex(u => u.NombreUsuario).IsUnique();
            builder.Property(u => u.NombreUsuario).HasMaxLength(50).IsRequired();

            builder.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
            builder.Property(u => u.Celular).HasMaxLength(15).IsRequired();

            builder.Property(u => u.Rol).HasConversion<int>().IsRequired();

            builder.Property(u => u.Activo).HasDefaultValue(false);
            builder.Property(u => u.FechaCreacion).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
