using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Shared.Events
{
    // This attribute tells MassTransit: "Send this to the 'catalog-events' topic"
    [EntityName("product-saved")]
    public record ProductSavedEvent(
        Guid Id,
        string Name,
        decimal Price,
        string Category,
        string Description
    );

    [EntityName("product-removed")]
    public record ProductDeletedEvent(Guid Id);
}
