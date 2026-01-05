using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Catalog.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation Property: One Category has many Products
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
