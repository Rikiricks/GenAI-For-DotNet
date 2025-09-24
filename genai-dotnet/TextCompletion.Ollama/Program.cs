using Microsoft.Extensions.AI;
using OllamaSharp;
using TextCompletion.Ollama.Models;

IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2");

#region Basic Completion

//// send prompt and get response
//string prompt = "What is AI ? explain max 20 word";
//Console.WriteLine($"user >>> {prompt}");

//ChatResponse response = await client.GetResponseAsync(prompt);

//Console.WriteLine($"assistant >>> {response}");
//Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");

#endregion

#region Streaming

//string prompt = "What is AI ? explain max 200 word";
//Console.WriteLine($"user >>> {prompt}");

//var responseStream = client.GetStreamingResponseAsync(prompt);
//await foreach (var message in responseStream)
//{
//    Console.Write(message.Text);
//}

#endregion

#region Classification
//var classificationPrompt = """
//Please classify the following sentences into categories: 
//- 'complaint' 
//- 'suggestion' 
//- 'praise' 
//- 'other'.

//1) "I love the new layout!"
//2) "You should add a night mode."
//3) "When I try to log in, it keeps failing."
//4) "This app is decent."
//""";

//Console.WriteLine($"user >>> {classificationPrompt}");

//ChatResponse classificationResponse = await client.GetResponseAsync(classificationPrompt);
//Console.WriteLine($"assistant >>>\n{classificationResponse}");

#endregion

#region Summarization
//var summaryPrompt = """
//Summarize the following blog in 1 concise sentences:

//"Microservices architecture is increasingly popular for building complex applications, but it comes with additional overhead. It's crucial to ensure each service is as small and focused as possible, and that the team invests in robust CI/CD pipelines to manage deployments and updates. Proper monitoring is also essential to maintain reliability as the system grows."
//""";

//Console.WriteLine($"user >>> {summaryPrompt}");

//ChatResponse summaryResponse = await client.GetResponseAsync(summaryPrompt);

//Console.WriteLine($"assistant >>> \n{summaryResponse}");
#endregion

#region Sentiment Analysis

//var analysisPrompt = """
//        You will analyze the sentiment of the following product reviews. 
//        Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews and create html file for this.

//        I bought this product and it's amazing. I love it!
//        This product is terrible. I hate it.
//        I'm not sure about this product. It's okay.
//        I found this product based on the other reviews. It worked for a bit, and then it didn't.
//        """;

//Console.WriteLine($"user >>> {analysisPrompt}");

//ChatResponse responseAnalysis = await client.GetResponseAsync(analysisPrompt);

//Console.WriteLine($"assistant >>> \n{responseAnalysis}");

#endregion

#region Structured Data Extraction

var carListings = new[]
{
    "Check out this stylish 2019 Toyota Camry. It has a clean title, only 40,000 miles on the odometer, and a well-maintained interior. The car offers great fuel efficiency, a spacious trunk, and modern safety features like lane departure alert. Minimum offer price: $18,000. Contact Metro Auto at (555) 111-2222 to schedule a test drive.",
    "Lease this sporty 2021 Honda Civic! With only 10,000 miles, it includes a sunroof, premium sound system, and backup camera. Perfect for city driving with its compact size and great fuel mileage. Located in Uptown Motors, monthly lease starts at $250 (excl. taxes). Call (555) 333-4444 for more info.",
    "A classic 1968 Ford Mustang, perfect for enthusiasts. The vehicle needs some interior restoration, but the engine runs smoothly. V8 engine, manual transmission, around 80,000 miles. This vintage gem is priced at $25,000. Contact Retro Wheels at (555) 777-8888 if you’re interested.",
    "Brand new 2023 Tesla Model 3 for lease. Zero miles, fully electric, autopilot capabilities, and a sleek design. Monthly lease starts at $450. Clean lines, minimalist interior, top-notch performance. For more details, call EVolution Cars at (555) 999-0000.",
    "Selling a 2015 Subaru Outback in good condition. 60,000 miles on it, includes all-wheel drive, heated seats, and ample cargo space for family getaways. Minimum offer price: $14,000. Contact Forrest Autos at (555) 222-1212 if you want a reliable adventure companion.",
    "Selling a 2018 Hyundai Creta in excellent condition. Driven 45,000 miles, this SUV comes with a fuel-efficient petrol engine, stylish design, and a comfortable interior perfect for city drives and long journeys. Features include touchscreen infotainment, reverse camera, airbags, and spacious seating for the whole family. Minimum offer price: $12,500. Contact Forrest Autos at (555) 222-1212 if you’re looking for a stylish and dependable ride."
};

foreach (var listing in carListings)
{
    var response = await client.GetResponseAsync<CarDetails>($"""
                Convert the following car listing into a JSON object matching this C# schema:
                Condition: "New" or "Used"
                Make: (car manufacturer)
                Model: (car model)
                Year: (four-digit year)
                Listing Type: "Sale" or "Lease"
                Price: integer only
                Features: array of short strings
                TenWordSummary: exactly ten words to summarize this listing
                CarType: e.g. "SUV", "Sedan", "Truck", etc.
                Here is the listing:
                {listing}
                """);

    if (response.TryGetResult(out var info))
    {
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(
      info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
    }
    else
    {
        Console.WriteLine("Response was not in the expected format.");
    }
}

#endregion

#region Chat Application

//List<ChatMessage> chatHistory = new()
//{
//        new ChatMessage(ChatRole.System, """
//            You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
//            You introduce yourself when first saying hello.
//            When helping people out, you always ask them for this information
//            to inform the hiking recommendation you provide:

//            1. The location where they would like to hike
//            2. What hiking intensity they are looking for

//            You will then provide three suggestions for nearby hikes that vary in length
//            after you get that information. You will also share an interesting fact about
//            the local nature on the hikes when making a recommendation. At the end of your
//            response, ask if there is anything else you can help with.
//        """)
//    };

//while (true)
//{
//    Console.Write("You >>> ");
//    var userPrompt = Console.ReadLine();
//    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt ?? string.Empty));

//    Console.WriteLine("AI Response:");
//    var response = string.Empty;

//    await foreach (var item in client.GetStreamingResponseAsync(chatHistory))
//    {
//        Console.Write(item.Text);
//        response += item.Text;
//    }

//    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
//    Console.WriteLine();
//}

#endregion