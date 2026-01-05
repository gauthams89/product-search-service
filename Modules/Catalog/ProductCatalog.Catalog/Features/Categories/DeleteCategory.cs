using MediatR;
using ProductCatalog.Catalog.Data;

namespace ProductCatalog.Catalog.Features.Categories;

public record DeleteCategoryCommand(Guid Id) : IRequest;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly CatalogDbContext _context;

    public DeleteCategoryHandler(CatalogDbContext context) => _context = context;

    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _context.Categories.FindAsync(new object[] { request.Id }, ct);
        if (category == null) return; // Or throw exception

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(ct);
    }
}