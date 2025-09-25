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

IChatClient client = new ChatClientBuilder(openAIClient).UseFunctionInvocation().Build();

Func<string,string, string> getWeather = (string location, string unit) =>
{
    // Here you would call a weather API to get the weather for the location
    var temperature = Random.Shared.Next(5, 20);
    var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";
    return $"The weather is {temperature} degrees C and {conditions}.";
};

var chatOptions = new ChatOptions
{
    Tools = [AIFunctionFactory.Create(getWeather, "get_current_weather", "Get the current weather in a given location")]
};

List<ChatMessage> chatHistory = [new(ChatRole.System, """
    You are a hiking enthusiast who helps people discover fun hikes in their area. 
    You are upbeat and friendly.
    """)];

// Weather conversation relevant to the registered function.
chatHistory.Add(new(ChatRole.User, """
    I live in Ahmedabad and I'm looking for a moderate intensity hike. 
    What's the current weather like? 
    """));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

ChatResponse response = await client.GetResponseAsync(chatHistory, chatOptions);

chatHistory.Add(new(ChatRole.Assistant, response.Text));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

chatHistory.Add(new(ChatRole.User, "Yeah please"));

ChatResponse response2 = await client.GetResponseAsync(chatHistory);

Console.WriteLine($"{response2.Text}");