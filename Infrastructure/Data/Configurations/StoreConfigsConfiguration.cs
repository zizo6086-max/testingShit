using Domain.Models.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class StoreConfigsConfiguration: IEntityTypeConfiguration<StoreConfigs>
{
    public void Configure(EntityTypeBuilder<StoreConfigs> builder)
    {
        builder.HasKey(s => s.Id); 
        builder.Property(s => s.PriceMargin).IsRequired();
    }
}