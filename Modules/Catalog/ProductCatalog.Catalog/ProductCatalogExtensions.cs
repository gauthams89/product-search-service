using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Catalog.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Catalog
{
    public static class ProductCatalogExtensions
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration config)
        {
            // 1. Get Connection String specifically for Postgres
            // We use the bind object for type safety, or GetConnectionString directly
            var connectionString = config.GetConnectionString("Postgres");

            // 2. Register EF Core (Postgres)
            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseNpgsql(connectionString);

                // Helpful for local debugging
#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
            });

            // 3. Register MediatR (CQRS Handlers)
            // This scans the current assembly (ProductCatalog.Catalog) for all Commands/Queries
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ProductCatalogExtensions).Assembly);
            });

            // 4. Register Validators (Optional but recommended)
            // services.AddValidatorsFromAssembly(typeof(CatalogModuleExtensions).Assembly);

            return services;
        }
    }
}
