using Micron.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Micron.Identity.Persistence.Configurations;

public class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> builder)
    {
        builder.ToTable("TenantMemberships");

        builder.HasKey(tm => tm.Id);

        builder.Property(tm => tm.TenantId).IsRequired();
        builder.Property(tm => tm.UserId).IsRequired();

        // A user belongs to a given tenant at most once.
        builder.HasIndex(tm => new { tm.TenantId, tm.UserId }).IsUnique();
    }
}
