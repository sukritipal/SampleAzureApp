using Mvc.StorageAccount.Demo.Data;

namespace Mvc.StorageAccount.Demo.Services
{
    public interface ITableStorageService
    {
        Task DeleteAttendee(string id, string industry);
        Task<AttendeeEntity> GetAttendee(string id, string industry);
        Task<List<AttendeeEntity>> GetAttendees();
        Task UpsertAttendee(AttendeeEntity entity);
    }
}