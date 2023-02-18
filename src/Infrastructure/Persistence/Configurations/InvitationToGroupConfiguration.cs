using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class InvitationToGroupConfiguration : IEntityTypeConfiguration<InvitationToGroup>
{
    public void Configure(EntityTypeBuilder<InvitationToGroup> builder)
    {
        builder.HasIndex(p => p.GroupId).IsClustered(false);
        builder.HasIndex(p => p.DomainUserId).IsClustered(false);
    }
}