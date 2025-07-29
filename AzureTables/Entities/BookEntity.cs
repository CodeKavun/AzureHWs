using Azure;
using Azure.Data.Tables;

namespace AzureTables.Entities
{
    public class BookEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Books";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public int PublishYear { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
