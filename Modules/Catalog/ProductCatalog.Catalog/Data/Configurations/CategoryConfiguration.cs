using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Catalog.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            // Seed some initial data (Optional, but helpful for testing)
            builder.HasData(
                new Category { Id = Guid.Parse("b0c36b6c-38e5-4a6f-8705-4089947701dc"), Name = "Electronics", Description = "Gadgets and Devices" },
                new Category { Id = Guid.Parse("d2e46b6c-38e5-4a6f-8705-4089947702df"), Name = "Books", Description = "Technical and Fiction" }
            );
        }
    }
}
