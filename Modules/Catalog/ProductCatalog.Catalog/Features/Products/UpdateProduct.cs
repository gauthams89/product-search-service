using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Catalog.Data;
using ProductCatalog.Shared.Events;

namespace ProductCatalog.Catalog.Features.Products;

public record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price) : IRequest;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly CatalogDbContext _context;
    private readonly ITopicProducer<ProductSavedEvent> _producer;

    public UpdateProductHandler(CatalogDbContext context, ITopicProducer<ProductSavedEvent> producer)
    {
        _context = context;
        _producer = producer;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken ct)
    {
        // 1. Fetch from DB
        var product = await _context.Products
            .Include(p => p.Category) // Include Category for the event payload
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (product == null) throw new KeyNotFoundException("Product not found");

        // 2. Update SQL
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;

        await _context.SaveChangesAsync(ct);

        // 3. Publish to Kafka (Elastic Sync)
        await _producer.Produce(new ProductSavedEvent(
            product.Id,
            product.Name,
            product.Price,
            product.Category.Name, // Send the resolved category name
            product.Description
        ), ct);
    }
}