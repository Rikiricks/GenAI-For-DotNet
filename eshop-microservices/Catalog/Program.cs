
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<CatalogDbContext>(connectionName: "catalogdb");

builder.Services.AddScoped<ProductAIService>();
builder.Services.AddScoped<ProductService>();


IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// get credentials from user secrets
var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// create a chat client
IChatClient chatClient = new OpenAIClient(credential, options).GetChatClient("openai/gpt-4o-mini").AsIChatClient();

builder.Services.AddChatClient(chatClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseMigration();

app.MapProductEndpoints();

app.Run();
