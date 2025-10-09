using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

IConfiguration config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var cred = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));

var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

IChatClient client = new OpenAIClient(cred, options).GetChatClient("openai/gpt-4o-mini").AsIChatClient();

foreach(var item in Directory.GetFiles("Images", "*.jpg"))
{
    var name = Path.GetFileNameWithoutExtension(item);

    var message = new ChatMessage(ChatRole.User, $$"""
        Extract information from this image from camera {{name}}.
        
            Respond with a JSON object in this form: {
            "Status": string // One of these values: "Clear", "Flowing", "Congested", "Blocked",
            "NumCars": number,
            "NumTrucks": number
        }
        """);

    message.Contents.Add(new DataContent(File.ReadAllBytes(item), "image/jpeg"));

    var response = await client.GetResponseAsync<TrafficCamResult>(message);

    if(response.TryGetResult(out var result))
    {
        Console.WriteLine($"{name} status: {result.Status} (cars: {result.NumCars}, trucks: {result.NumTrucks})");
    }
}


class TrafficCamResult
{
    public TrafficStatus Status { get; set; }
    public int NumCars { get; set; }
    public int NumTrucks { get; set; }

    public enum TrafficStatus { Clear, Flowing, Congested, Blocked };
}