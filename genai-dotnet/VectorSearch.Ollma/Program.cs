using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OllamaSharp;
using VectorSearch.Ollma.Models;

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaApiClient(new Uri("http://localhost:11434"), "all-minilm");

var vectorStore = new InMemoryVectorStore();

var movieStore = vectorStore.GetCollection<int, Movie<int>>("movies");

await movieStore.EnsureCollectionExistsAsync();

foreach (var movie in MovieData.Movies)
{
    movie.Vector = await embeddingGenerator.GenerateVectorAsync(movie.Description);
    await movieStore.UpsertAsync(movie);
}

Console.WriteLine("Query >>");

var query = Console.ReadLine();

var queryEmbedding = await embeddingGenerator!.GenerateVectorAsync(query);

var searchResults = movieStore.SearchAsync(queryEmbedding, top: 2);


await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}

