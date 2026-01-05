using MassTransit;
using MediatR;
using ProductCatalog.Catalog.Data;
using ProductCatalog.Shared.Events;

namespace ProductCatalog.Catalog.Features.Products;

public record DeleteProductCommand(Guid Id) : IRequest;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly CatalogDbContext _context;
    private readonly ITopicProducer<ProductDeletedEvent> _producer;

    public DeleteProductHandler(CatalogDbContext context, ITopicProducer<ProductDeletedEvent> producer)
    {
        _context = context;
        _producer = producer;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        // 1. Find
        var product = await _context.Products.FindAsync(new object[] { request.Id }, ct);
        if (product == null) return;

        // 2. Delete from SQL
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(ct);

        // 3. Publish to Kafka (Tell Elastic to delete)
        await _producer.Produce(new ProductDeletedEvent(request.Id), ct);
    }
}