using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mvc.StorageAccount.Demo.Data;

namespace Mvc.StorageAccount.Demo.Services
{
    public class TableStorageService : ITableStorageService
    {
        private const string TableName = "Attendees";
        private readonly IConfiguration _configuration;
        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;   
        }

        public async Task<AttendeeEntity> GetAttendee(string id, string industry)
        {
            var tableClient = await GetTableClient();
            return await tableClient.GetEntityAsync<AttendeeEntity>(industry, id);
        }

        public async Task<List<AttendeeEntity>> GetAttendees()
        {
            var tableClient = await GetTableClient();
            Pageable<AttendeeEntity> attendeeEntities = tableClient.Query<AttendeeEntity>();
            return attendeeEntities.ToList();
        }

        public async Task UpsertAttendee(AttendeeEntity entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.UpsertEntityAsync<AttendeeEntity>(entity);
        }

        public async Task DeleteAttendee(string id, string industry)
        {
            var tableClient = await GetTableClient();
            await tableClient.DeleteEntityAsync(industry, id);
        }

        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["StorageConnectionStrings"]);
            var tableClient = serviceClient.GetTableClient(TableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
    }
}
