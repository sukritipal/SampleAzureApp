﻿namespace Blazor.AzureCosmosDb.Demo.Data
{
    public interface IEngineerService
    {
        Task UpsertEngineer(Engineer engineer);
        Task DeleteEngineer(string? id, string? partitionKey);
        Task<List<Engineer>> GetEngineerDetails();
        Task<Engineer> GetEngineerDetailsById(string? id, string? partitionKey);
    }
}