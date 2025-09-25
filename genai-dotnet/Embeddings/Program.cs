using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.Numerics.Tensors;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// get credentials from user secrets
var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = 
    new OpenAIClient(credential, options).GetEmbeddingClient("openai/text-embedding-3-small").AsIEmbeddingGenerator();

// 1: Generate a single embedding
var embedding = await embeddingGenerator.GenerateVectorAsync("Hola Amigo!");
Console.WriteLine($"Embedding dimensions: {embedding.Span.Length}");
foreach(var value in embedding.Span)
{
    Console.Write("{0:0.00}, ", value);
}

var catVector = await embeddingGenerator.GenerateVectorAsync("cat");
var dogVector = await embeddingGenerator.GenerateVectorAsync("dog");
var kittenVector = await embeddingGenerator.GenerateVectorAsync("kitten");

Console.WriteLine($"cat-dog similarity: {TensorPrimitives.CosineSimilarity(catVector.Span, dogVector.Span):F2}");
Console.WriteLine($"cat-kitten similarity: {TensorPrimitives.CosineSimilarity(catVector.Span, kittenVector.Span):F2}");
Console.WriteLine($"dog-kitten similarity: {TensorPrimitives.CosineSimilarity(dogVector.Span, kittenVector.Span):F2}");