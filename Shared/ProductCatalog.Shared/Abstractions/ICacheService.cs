using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Shared.Abstractions
{
    public interface ICacheService
    {
        // Generic Get
        Task<T?> GetAsync<T>(string key);
        // Generic Set with Expiry
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        // Remove (Invalidate)
        Task RemoveAsync(string key);
    }
}
