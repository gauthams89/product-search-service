using MassTransit;
using MediatR;
using ProductCatalog.Catalog.Data;
using ProductCatalog.Catalog.Entities;
using ProductCatalog.Shared.Abstractions;
using ProductCatalog.Shared.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Catalog.Features.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly CatalogDbContext _context;
        private readonly ITopicProducer<ProductSavedEvent> _producer; // Kafka Producer
        private readonly ICacheService _cache;

        public CreateProductHandler(CatalogDbContext context, ITopicProducer<ProductSavedEvent> producer, ICacheService cache)
        {
            _context = context;
            _producer = producer;
            _cache = cache;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Create Entity
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                SKU = request.Sku,
                CategoryId = request.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 2. Save to Postgres (Source of Truth)
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            // 3. Publish Event to Kafka (For Elastic Sync)
            // Note: We map the Category ID to a dummy name for now, or fetch it if needed.
            // Ideally, the Consumer will hydrate the category name.
            ProductSavedEvent productSavedEvent = new ProductSavedEvent(
                product.Id,
                product.Name,
                product.Price,
                product.CategoryId.ToString(), // Sending ID as category for now
                product.Description
            );

            await _producer.Produce(productSavedEvent, cancellationToken);

            // 4. Invalidate Cache (if you had a "Get All" cache)
            // We aren't caching individual products yet, but this is where it goes.
            await _cache.RemoveAsync($"product:{product.Id}");
            return product.Id;
        }
    }
}
