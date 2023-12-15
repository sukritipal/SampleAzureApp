using System;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp.Demo
{
    public class MessageSender
    {
        [FunctionName("MessageSender")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var message = $"C# Timer trigger function executed at: {DateTime.Now}";
            HttpClient httpClient = new();
            HttpRequestMessage httpRequestMessage = new(HttpMethod.Post, "http://localhost:7291/api/MessageReceiver");
            httpRequestMessage.Content = new
            StringContent(
                JsonConvert.SerializeObject(message),
                Encoding.UTF8,
                "application/json"
            );
            httpClient.Send(httpRequestMessage);
            log.LogInformation("Timer Trigger Executed");
        }
    }
}
