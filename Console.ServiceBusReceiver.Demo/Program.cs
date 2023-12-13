using Azure.Messaging.ServiceBus;

const string serviceBusConnectionString = "Endpoint=sb://spsamplesb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=i0hvLtbdTSCv116CFj/vh4PqaB0RvfdJh+ASbJewVao=";
const string queueOrTopicName = "spsamplequeue";
const string subName = "sub1";

ServiceBusClient client;
ServiceBusProcessor processor = default!;

async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine(body);
    await args.CompleteMessageAsync(args.Message);
}

Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

client = new ServiceBusClient(serviceBusConnectionString);
processor = client.CreateProcessor(queueOrTopicName, subName, new ServiceBusProcessorOptions());

try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();

    Console.WriteLine("\nPress any key to end the processing");
    Console.ReadKey();

    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("\nStopped receiving messages");
}
catch(Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
    throw;
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();
}