using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Portifolio.Dominio.Entidades;

namespace Portifolio.Infraestrutura.Data.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.OwnsOne(c => c.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt);

            // Self-referencing relationship
            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(c => c.Name.Value)
                .IsUnique()
                .HasDatabaseName("IX_Categories_Name");

            builder.HasIndex(c => c.ParentCategoryId)
                .HasDatabaseName("IX_Categories_ParentCategoryId");

            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Categories_IsActive");
        }
    }
}
