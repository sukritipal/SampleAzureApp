using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

QueueClient queue = new QueueClient(connectionString, "attendee-emails");

if (await queue.ExistsAsync())
{
    QueueProperties properties = await queue.GetPropertiesAsync();
    for(int i = 0; i < properties.ApproximateMessagesCount; i++)
    {
        string message = await RetrieveNextMessage();
        Console.WriteLine($"Received: {message}");
    }
}

async Task<string> RetrieveNextMessage()
{
    QueueMessage[] retrievedMessage = await queue.ReceiveMessagesAsync(1);
    var data = Convert.FromBase64String(retrievedMessage[0].Body.ToString());
    string message = Encoding.UTF8.GetString(data);

    await queue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);

    return message;
}