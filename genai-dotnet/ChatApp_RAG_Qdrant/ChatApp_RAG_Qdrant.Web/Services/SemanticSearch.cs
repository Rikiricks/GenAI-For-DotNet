using Microsoft.Extensions.VectorData;

namespace ChatApp_RAG_Qdrant.Web.Services;

public class SemanticSearch(
    VectorStoreCollection<Guid, IngestedChunk> vectorCollection, ILogger<SemanticSearch> logger)
{
    public async Task<IReadOnlyList<IngestedChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        logger.LogInformation("Searching for top {maxResults} chunks matching text: {text} with documentIdFilter: {documentIdFilter}", maxResults, text, documentIdFilter ?? "null");
        var nearest = vectorCollection.SearchAsync(text, maxResults, new VectorSearchOptions<IngestedChunk>
        {
            Filter = documentIdFilter is { Length: > 0 } ? record => record.DocumentId == documentIdFilter : null,
        });

        return await nearest.Select(result => result.Record).ToListAsync();
    }
}
