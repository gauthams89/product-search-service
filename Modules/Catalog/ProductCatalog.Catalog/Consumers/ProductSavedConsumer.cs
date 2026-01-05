using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProductCatalog.Shared.Events;
using ProductCatalog.Shared.Models;

namespace ProductCatalog.Catalog.Consumers;

public class ProductSavedConsumer : IConsumer<ProductSavedEvent>
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly ILogger<ProductSavedConsumer> _logger;

    public ProductSavedConsumer(ElasticsearchClient elasticClient, ILogger<ProductSavedConsumer> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductSavedEvent> context)
    {
        var @event = context.Message;

        // 1. Map Event to Search Document
        var document = new ProductSearchDocument
        {
            Id = @event.Id,
            Name = @event.Name,
            Description = @event.Description,
            Price = @event.Price,
            CategoryName = @event.Category, // Denormalized Category Name
            LastUpdated = DateTime.UtcNow
        };

        // 2. Index into Elasticsearch (Upsert)
        var response = await _elasticClient.IndexAsync(document, i => i
            .Index("products-search")
            .Id(document.Id.ToString())
        );

        if (response.IsValidResponse)
        {
            _logger.LogInformation("Successfully indexed product {ProductId} in Elasticsearch.", @event.Id);
        }
        else
        {
            _logger.LogError("Failed to index product {ProductId}. Error: {Error}", @event.Id, response.DebugInformation);
        }
    }
}