using CatalogoDeProdutos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CatalogoDeProdutos.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(500) 
                .IsRequired();

            builder.Property(p => p.Price)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(p => p.Category)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(250);
        }
    }
}