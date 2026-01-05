using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Catalog.Data;

namespace ProductCatalog.Catalog.Features.Categories;

// Query DTO
public record CategoryDto(Guid Id, string Name, string Description);

// Get All Query
public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;

// Handler
public class GetCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly CatalogDbContext _context;

    public GetCategoriesHandler(CatalogDbContext context) => _context = context;

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
    {
        return await _context.Categories
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description))
            .ToListAsync(ct);
    }
}