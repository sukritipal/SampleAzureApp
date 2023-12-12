using Azure;
using Azure.Data.Tables;
using Mvc.StorageAccount.Demo.Data;

namespace Mvc.StorageAccount.Demo.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableClient _tableClient;
        public TableStorageService(TableClient tableClient)
        {
           _tableClient = tableClient;
        }

        public async Task<AttendeeEntity> GetAttendee(string id, string industry)
        {
            return await _tableClient.GetEntityAsync<AttendeeEntity>(industry, id);
        }

        public async Task<List<AttendeeEntity>> GetAttendees()
        {
            Pageable<AttendeeEntity> attendeeEntities = _tableClient.Query<AttendeeEntity>();
            return attendeeEntities.ToList();
        }

        public async Task UpsertAttendee(AttendeeEntity entity)
        {
            await _tableClient.UpsertEntityAsync(entity);
        }

        public async Task DeleteAttendee(string id, string industry)
        {
            await _tableClient.DeleteEntityAsync(industry, id);
        }
    }
}
