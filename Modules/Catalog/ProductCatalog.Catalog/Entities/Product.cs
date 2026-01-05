using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Catalog.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string SKU { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public Guid CategoryId { get; set; }

        // Navigation Property
        public Category Category { get; set; } = null!;
    }
}
