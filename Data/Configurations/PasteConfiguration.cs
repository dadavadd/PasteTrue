using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasteTrue.Models;

namespace PasteTrue.Data.Configurations
{
    public class PasteConfiguration : IEntityTypeConfiguration<Paste>
    {
        public void Configure(EntityTypeBuilder<Paste> builder)
        {
            builder.HasOne(p => p.User)
                .WithMany(u => u.Pastes)
                .HasForeignKey(p => p.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(e => e.Id);

            builder.HasOne(d => d.User)
                .WithMany(p => p.Pastes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.CreatedAt);
            builder.HasIndex(e => e.ExpiresAt);
        }
    }
}
