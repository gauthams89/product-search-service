using Microsoft.EntityFrameworkCore;
using ProductCatalog.Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ProductCatalog.Catalog.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Finds all classes implementing IEntityTypeConfiguration
            // in the current project and applies them.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
