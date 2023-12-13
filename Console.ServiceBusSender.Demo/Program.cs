using Azure.Messaging.ServiceBus;

const string serviceBusConnectionString = "Endpoint=sb://spsamplesb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=i0hvLtbdTSCv116CFj/vh4PqaB0RvfdJh+ASbJewVao=";
const string queueOrTopicName = "spsamplequeue";
const int maxNumberOfMessages = 3;

ServiceBusClient client;
ServiceBusSender sender;

client = new ServiceBusClient(serviceBusConnectionString);
sender = client.CreateSender(queueOrTopicName);

using ServiceBusMessageBatch batch = await sender.CreateMessageBatchAsync();
for(int i = 1; i <= maxNumberOfMessages; i++)
{
    if (!batch.TryAddMessage(new ServiceBusMessage($"This is a message-{i}")))
    {
        Console.WriteLine($"Message-{i} was not added to the batch");
    }
}

try
{
    await sender.SendMessagesAsync(batch);
    Console.WriteLine("Messages sent");
}
catch(Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
    throw;
}
finally
{
    await sender.DisposeAsync();
    await client.DisposeAsync();
}
