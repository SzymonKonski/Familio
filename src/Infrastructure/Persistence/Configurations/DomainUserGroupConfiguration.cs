using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DomainUserGroupConfiguration : IEntityTypeConfiguration<DomainUserGroup>
{
    public void Configure(EntityTypeBuilder<DomainUserGroup> builder)
    {
        builder.HasKey(bc => new {bc.GroupId, bc.DomainUserId});

        builder.HasOne(bc => bc.Group)
            .WithMany(b => b.DomainUserGroups)
            .HasForeignKey(bc => bc.GroupId);

        builder.HasOne(bc => bc.DomainUser)
            .WithMany(c => c.DomainUserGroups)
            .HasForeignKey(bc => bc.DomainUserId);
    }
}