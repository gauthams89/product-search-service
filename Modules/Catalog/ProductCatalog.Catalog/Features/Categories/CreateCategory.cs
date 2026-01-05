using MediatR;
using ProductCatalog.Catalog.Data;
using ProductCatalog.Catalog.Entities;

namespace ProductCatalog.Catalog.Features.Categories;

// Command
public record CreateCategoryCommand(string Name, string Description) : IRequest<Guid>;

// Handler
public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly CatalogDbContext _context;

    public CreateCategoryHandler(CatalogDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(ct);
        return category.Id;
    }
}