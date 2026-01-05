using MediatR;

namespace ProductCatalog.Catalog.Features.CreateProduct
{
    public record CreateProductCommand(
        string Name,
        string Description,
        decimal Price,
        string Sku,
        Guid CategoryId
    ) : IRequest<Guid>;
}
