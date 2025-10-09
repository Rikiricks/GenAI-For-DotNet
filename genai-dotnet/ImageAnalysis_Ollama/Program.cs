using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"), "llava");

// user prompts
//var promptDescribe = "Describe the image";
//var promptAnalyze = "How many bus are there?";

//// prompts
//string systemPrompt = "You are a useful assistant that describes images using a direct style.";
//var userPrompt = promptAnalyze;


//var messages = new List<ChatMessage>()
//{
//    new ChatMessage(ChatRole.System, systemPrompt),
//    new ChatMessage(ChatRole.User, userPrompt)
//};

//var imageFileName = "cars.png";
//string image = Path.Combine(Directory.GetCurrentDirectory(), "images", imageFileName);

//AIContent aic = new DataContent(File.ReadAllBytes(image), "image/png");
//var message = new ChatMessage(ChatRole.User, [aic]);

//messages.Add(message);
//var response = await client.GetResponseAsync(messages);
//Console.WriteLine($"Prompt: {userPrompt}");
//Console.WriteLine($"Image: {imageFileName}");
//Console.WriteLine($"Response: {response.Messages[0]}");


foreach (var item in Directory.GetFiles("TrafficCam", "*.jpg"))
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

    if (response.TryGetResult(out var result))
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