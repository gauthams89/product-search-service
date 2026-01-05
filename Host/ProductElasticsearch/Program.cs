using Confluent.Kafka;
using MassTransit;
using ProductCatalog.Api.Extensions;
using ProductCatalog.Catalog;
using ProductCatalog.Catalog.Consumers;
using ProductCatalog.Shared;
using ProductCatalog.Shared.Events;
using ProductCatalog.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Shared Infrastructure (Redis, Elastic)
builder.Services.AddSharedInfrastructure(builder.Configuration);

// 2. Catalog Module (EF Core, MediatR)
builder.Services.AddCatalogModule(builder.Configuration);

// 3. API-Specific MassTransit Setup
builder.Services.AddApiMassTransit(builder.Configuration);


// Load controllers from the Catalog Module Assembly
builder.Services.AddControllers()
    .AddApplicationPart(typeof(ProductCatalogExtensions).Assembly);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
