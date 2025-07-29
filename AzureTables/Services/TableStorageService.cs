using Azure.Data.Tables;
using AzureTables.Entities;

namespace AzureTables.Services
{
    public class TableStorageService
    {
        private readonly TableClient tableClient;

        public TableStorageService(IConfiguration configuration)
        {
            string connectionString = configuration["AzureStorage:ConnectionString"]!;
            string tableName = configuration["AzureStorage:TableName"]!;

            tableClient = new TableClient(connectionString, tableName);
            tableClient.CreateIfNotExists();
        }

        public async Task AddBook(BookEntity book) => await tableClient.AddEntityAsync(book);
    }
}
