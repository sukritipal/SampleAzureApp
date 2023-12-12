using Azure.Storage.Queues;
using Mvc.StorageAccount.Demo.Models;
using Newtonsoft.Json;

namespace Mvc.StorageAccount.Demo.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConfiguration _configuration;
        private readonly QueueClient _queueClient;
        public QueueService(IConfiguration configuration, QueueClient queueClient)
        {
            _configuration = configuration;
            _queueClient = queueClient;
        }

        public async Task SendMessage(EmailMessage emailMessage)
        {
            await _queueClient.CreateIfNotExistsAsync();
            var message = JsonConvert.SerializeObject(emailMessage);
            await _queueClient.SendMessageAsync(message);
        }
    }
}
