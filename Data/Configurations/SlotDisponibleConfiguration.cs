using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class SlotDisponibleConfiguration : IEntityTypeConfiguration<SlotDisponible>
    {
        public void Configure(EntityTypeBuilder<SlotDisponible> builder)
        {
            builder.ToTable("SlotsDisponibles");
            builder.HasKey(s => s.SlotId);

            builder.Property(s => s.HoraInicio)
                .HasColumnType("time");

            builder.Property(s => s.HoraFin)
                .HasColumnType("time");

            // CuposDisponibles >= 0 a nivel de base de datos (RN04)
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Slots_CuposDisponibles",
                "[CuposDisponibles] >= 0"));

            builder.HasOne(s => s.Programacion)
                .WithMany(p => p.Slots)
                .HasForeignKey(s => s.ProgramacionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.ProgramacionId);
            builder.HasIndex(s => s.EsSobrecupo);
        }
    }
}
