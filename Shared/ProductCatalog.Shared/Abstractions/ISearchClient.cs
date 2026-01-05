using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Shared.Abstractions
{
    public interface ISearchClient<T> where T:class
    {
        // Index (Upsert) a document
        Task IndexAsync(T document, string indexName);
        // Simple Search
        Task<IEnumerable<T>> SearchAsync(string query, string indexName);
    }
}
