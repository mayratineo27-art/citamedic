using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");
            builder.HasKey(t => t.TicketId);

            // 1:1 con Cita (RN12)
            builder.HasIndex(t => t.CitaId).IsUnique();
            builder.HasIndex(t => t.Codigo).IsUnique();

            builder.Property(t => t.Codigo)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(t => t.Cita)
                .WithOne(c => c.Ticket)
                .HasForeignKey<Ticket>(t => t.CitaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
