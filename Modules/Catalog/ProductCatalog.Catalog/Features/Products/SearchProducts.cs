using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MediatR;
using ProductCatalog.Shared;
using ProductCatalog.Shared.Models;

namespace ProductCatalog.Catalog.Features.Products;

public record SearchProductsQuery(string? Term, decimal? MinPrice, decimal? MaxPrice)
    : IRequest<IEnumerable<ProductSearchDocument>>;

public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, IEnumerable<ProductSearchDocument>>
{
    private readonly ElasticsearchClient _client;
    private readonly ProductCatalogOptions _catalogOptions;

    public SearchProductsHandler(ElasticsearchClient client, ProductCatalogOptions catalogOptions) {
        _client = client;
        _catalogOptions = catalogOptions;
    }

    public async Task<IEnumerable<ProductSearchDocument>> Handle(SearchProductsQuery request, CancellationToken ct)
    {
        var response = await _client.SearchAsync<ProductSearchDocument>(_catalogOptions.Elasticsearch.ProductSearchIndex,     
            s => s.Query(q => q
                .Bool(b =>
                {
                    // Add Must clause for search term
                    if (!string.IsNullOrEmpty(request.Term))
                    {
                        b.Must(m => m
                            .MultiMatch(mm => mm
                                        .Fields(new[] { "name", "description", "categoryName" })
                                        .Query(request.Term)
                                        .Fuzziness(new Fuzziness("AUTO"))));
                    }

                    // Add Filter clause for price range
                    if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
                    {
                        b.Filter(f => f
                            .Range(r => r
                                // 2. Use .Number() instead of .NumberRange()
                                .Number(nr => nr 
                                    .Field(p => p.Price) // Use typed lambda p => p.Price
                                    .Gte((double?)request.MinPrice)
                                    .Lte((double?)request.MaxPrice))));
                    }
                })
        ), ct);

        if (!response.IsValidResponse)
        {
            // Log error here
            return Enumerable.Empty<ProductSearchDocument>();
        }

        return response.Documents;
    }
}