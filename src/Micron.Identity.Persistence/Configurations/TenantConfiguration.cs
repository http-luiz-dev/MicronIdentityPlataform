using Micron.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Micron.Identity.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(t => t.Name).IsUnique();

        // A tenant grants access to several applications (products). No inverse
        // navigation exists on Application, so EF maps this as an optional one-to-many.
        builder.HasMany(t => t.Products)
            .WithOne()
            .OnDelete(DeleteBehavior.SetNull);
    }
}
