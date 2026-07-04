using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
// Aliased to avoid the clash with the 'Micron.Identity.Application' namespace.
using ApplicationEntity = Micron.Identity.Domain.Entities.Application;

namespace Micron.Identity.Persistence.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.Description)
            .HasMaxLength(1024);

        builder.HasIndex(a => a.Name).IsUnique();
    }
}
