using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using ProductCatalog.Shared.Abstractions;
using ProductCatalog.Shared.Events;
using System.ComponentModel;

namespace ProductCatalog.Shared.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // 1. Bind Options
            var options = config.Get<ProductCatalogOptions>() ?? new ProductCatalogOptions();
            services.AddSingleton(options);

            // 2. Redis Cache Setup
            services.AddStackExchangeRedisCache(redis =>
            {
                redis.Configuration = options.Redis.ConnectionString;
            });
            services.AddSingleton<ICacheService, RedisCacheService>();

            // 3. Elasticsearch Setup (v8 Client)
            services.AddSingleton<ElasticsearchClient>(sp =>
            {
                var settings = new ElasticsearchClientSettings(new Uri(options.Elasticsearch.Uri))
                    .DefaultIndex(options.Elasticsearch.DefaultIndex)
                    .Authentication(new BasicAuthentication(
                        options.Elasticsearch.Username!,
                        options.Elasticsearch.Password!
                    ));

                // Helpful for debugging: print raw queries to console in Dev
#if DEBUG
                settings.DisableDirectStreaming().OnRequestCompleted(details =>
                {
                    if (details.RequestBodyInBytes != null)
                        Console.WriteLine(System.Text.Encoding.UTF8.GetString(details.RequestBodyInBytes));
                });
#endif

                return new ElasticsearchClient(settings);
            });

            // Register our custom Search Service wrapper
            // Note: You need to implement the concrete class logic (shown later)
            // TODO: services.AddScoped(typeof(ISearchClient<>), typeof(ElasticSearchClient<>));

            

            return services;
        }
    }
}
