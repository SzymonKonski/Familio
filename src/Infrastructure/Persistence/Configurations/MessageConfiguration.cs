using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.Property(s => s.Content);

        builder
            .HasOne(p => p.CreatedByUser)
            .WithMany(b => b.Messages)
            .HasForeignKey(p => new {p.GroupId, p.UserId});

        builder.HasIndex(p => p.GroupId).IsClustered(false);
        builder.HasIndex(p => p.Timestamp).IsClustered(false);
    }
}