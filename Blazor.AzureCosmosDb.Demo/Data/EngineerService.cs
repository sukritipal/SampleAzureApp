using Microsoft.Azure.Cosmos;

namespace Blazor.AzureCosmosDb.Demo.Data
{
    public class EngineerService : IEngineerService
    {
        private readonly string CosmosDbConnectionString = "ConnectionString";
        private readonly string CosmosDbName = "Contractors";
        private readonly string CosmosDbContainerName = "Engineers";

        private Container GetContainerClient()
        {
            var cosmosClient = new CosmosClient(CosmosDbConnectionString);
            var container = cosmosClient.GetContainer(CosmosDbName, CosmosDbContainerName);
            return container;
        }

        public async Task UpsertEngineer(Engineer engineer)
        {
            try
            {
                if(engineer.id == null)
                {
                    engineer.id = Guid.NewGuid();
                }
                var container = GetContainerClient();
                var response = await container.UpsertItemAsync(engineer, new PartitionKey(engineer.id.ToString()));
                Console.Write(response.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }
        }

        public async Task DeleteEngineer(string? id, string? partitionKey)
        {
            try
            {
                var container = GetContainerClient();
                var updateResponse = await container.DeleteItemAsync<Engineer>(id, new PartitionKey(partitionKey));
                Console.Write(updateResponse.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }
        }

        public async Task<List<Engineer>> GetEngineerDetails()
        {
            List<Engineer> engineers = new List<Engineer>();
            try
            {
                var container = GetContainerClient();
                var sqlQuery = "select * from c";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<Engineer> feedIterator = container.GetItemQueryIterator<Engineer>(queryDefinition);

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<Engineer> currentResultSet = await feedIterator.ReadNextAsync();
                    foreach (Engineer engineer in currentResultSet)
                    {
                        engineers.Add(engineer);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return engineers;
        }

        public async Task<Engineer> GetEngineerDetailsById(string? id, string? partitionKey)
        {
            try
            {
                var container = GetContainerClient();
                ItemResponse<Engineer> response = await container.ReadItemAsync<Engineer>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }
        }
    }
}
