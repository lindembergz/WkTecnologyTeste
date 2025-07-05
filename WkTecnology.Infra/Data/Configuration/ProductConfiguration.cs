using Microsoft.EntityFrameworkCore; // Ensure this namespace is included
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portifolio.Dominio.Entidades;

namespace Portifolio.Infraestrutura.Data.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.OwnsOne(p => p.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(200)
                    .IsRequired();

                name.HasIndex(n => n.Value)
                    .HasDatabaseName("IX_Products_Name");
            });

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.Brand)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Model)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Year)
                .IsRequired();

            builder.Property(p => p.Color)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Mileage)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new { p.Brand, p.Model })
                .HasDatabaseName("IX_Products_Brand_Model");

            builder.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            builder.HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Products_IsActive");

            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Products_CreatedAt");
        }
    }
}
