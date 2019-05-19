using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener.Models.Azure
{
    public class Url : TableEntity
    {
        public Url() { }

        public Url(string partitionKey, string rowKey, string longUrl)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            LongUrl = longUrl;
        }
        public string LongUrl { get; set; }
    }
}
