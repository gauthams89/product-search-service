using MediatR;
using ProductCatalog.Catalog.Data;

namespace ProductCatalog.Catalog.Features.Categories;

public record UpdateCategoryCommand(Guid Id, string Name, string Description) : IRequest;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly CatalogDbContext _context;

    public UpdateCategoryHandler(CatalogDbContext context) => _context = context;

    public async Task Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _context.Categories.FindAsync(new object[] { request.Id }, ct);
        if (category == null) throw new KeyNotFoundException("Category not found");

        category.Name = request.Name;
        category.Description = request.Description;

        await _context.SaveChangesAsync(ct);
    }
}