using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MobileCodeConfiguration : IEntityTypeConfiguration<MobileCode>
{
    public void Configure(EntityTypeBuilder<MobileCode> builder)
    {
    }
}