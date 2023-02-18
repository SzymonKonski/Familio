using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .HasOne(p => p.CreatedByUser)
            .WithMany(b => b.Events)
            .HasForeignKey(p => new {p.GroupId, p.UserId});
    }
}