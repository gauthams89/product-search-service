using Confluent.Kafka;
using MassTransit;
using ProductCatalog.Catalog.Consumers;
using ProductCatalog.Shared;
using ProductCatalog.Shared.Events;

namespace ProductCatalog.Api.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddApiMassTransit(this IServiceCollection services, IConfiguration config)
    {
        // 1. Bind Options
        var catalogOptions = new ProductCatalogOptions();
        config.Bind(catalogOptions);

        // 2. Configure MassTransit
        services.AddMassTransit(x =>
        {
            // A. Register Consumers (Add new consumers here as you build them)
            x.AddConsumer<ProductSavedConsumer>();

            // B. InMemory Bus (Transport)
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            // C. Kafka Rider
            x.AddRider(rider =>
            {
                // Producer
                rider.AddProducer<ProductSavedEvent>(catalogOptions.Kafka.ProductSaveTopic);

                // Kafka Host Config
                rider.UsingKafka((context, k) =>
                {
                    k.Host(catalogOptions.Kafka.Brokers, h =>
                    {
                        h.UseSasl(sasl =>
                        {
                            sasl.Username = catalogOptions.Kafka.Username;
                            sasl.Password = catalogOptions.Kafka.Password;

                            if (Enum.TryParse(catalogOptions.Kafka.SaslMechanism, true, out SaslMechanism mechanism))
                                sasl.Mechanism = mechanism;

                            if (Enum.TryParse(catalogOptions.Kafka.SecurityProtocol, true, out SecurityProtocol protocol))
                                sasl.SecurityProtocol = protocol;
                        });
                    });

                    // Consumer Endpoint Wiring
                    k.TopicEndpoint<string, ProductSavedEvent>(
                        catalogOptions.Kafka.ProductSaveTopic,
                        "product-catalog-consumer-group",
                        e =>
                        {
                            e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                            e.ConfigureConsumer<ProductSavedConsumer>(context);
                        });
                });
            });
        });

        return services;
    }
}