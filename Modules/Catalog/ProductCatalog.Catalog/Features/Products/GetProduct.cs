using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Catalog.Data;

namespace ProductCatalog.Catalog.Features.Products;

public record ProductDto(Guid Id, string Name, string Description, decimal Price, string CategoryName);
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

public class GetProductHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly CatalogDbContext _context;

    public GetProductHandler(CatalogDbContext context) => _context = context;

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Category.Name))
            .FirstOrDefaultAsync(ct);
    }
}