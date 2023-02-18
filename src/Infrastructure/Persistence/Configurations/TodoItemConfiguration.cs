using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems");

        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .HasOne(p => p.CreatedByUser)
            .WithMany(b => b.CreatedTodoItems)
            .HasForeignKey(p => new {p.GroupId, p.UserId});

        builder
            .HasOne(p => p.AssignedUser)
            .WithMany(b => b.AssignedTodoItems)
            .HasForeignKey(p => new {p.GroupId, p.AssignedUserId});

        builder.HasIndex(p => p.GroupId).IsClustered(false);
    }
}