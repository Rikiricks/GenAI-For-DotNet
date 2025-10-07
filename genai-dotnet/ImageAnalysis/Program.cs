using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// get credentials from user secrets
var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// create a chat client
IChatClient openAIClient = new OpenAIClient(credential, options).GetChatClient("openai/gpt-4o-mini").AsIChatClient();

// user prompts
var promptDescribe = "Describe the image";
var promptAnalyze = "How many bus are there?";

// prompts
string systemPrompt = "You are a useful assistant that describes images using a direct style.";
var userPrompt = promptAnalyze;

List<ChatMessage> messages = new List<ChatMessage>()
{
    new ChatMessage(ChatRole.System, systemPrompt),
    new ChatMessage(ChatRole.User, userPrompt)
};

var imageFileName = "traffic.png";
string image = Path.Combine(Directory.GetCurrentDirectory(), "images", imageFileName);

AIContent aic = new DataContent(File.ReadAllBytes(image), "image/png");
var message = new ChatMessage(ChatRole.User, [aic]);

messages.Add(message);  

var response = await openAIClient.GetResponseAsync(messages);
Console.WriteLine($"Prompt: {userPrompt}");
Console.WriteLine($"Image: {imageFileName}");
Console.WriteLine($"Response: {response.Messages[0]}");