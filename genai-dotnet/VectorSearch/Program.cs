using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using System.ClientModel;
using VectorSearch.Models;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// get credentials from user secrets
var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
    new OpenAIClient(credential, options).GetEmbeddingClient("openai/text-embedding-3-small").AsIEmbeddingGenerator();

var vectorStore = new InMemoryVectorStore();

var movieStore = vectorStore.GetCollection<int, Movie<int>>("movies");

await movieStore.EnsureCollectionExistsAsync();

foreach (var movie in MovieData.Movies)
{
    movie.Vector = await embeddingGenerator.GenerateVectorAsync(movie.Description);
    await movieStore.UpsertAsync(movie);
}


var query = "I want to see animation carton movie";

var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);

var searchResults = movieStore.SearchAsync(queryEmbedding, top: 2);


await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}

