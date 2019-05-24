using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using UrlShortener.Models.Azure;

namespace UrlShortener.Repositories.Azure
{
    public class UrlAnalyticsRepository : IUrlAnalyticsRepository
    {
        private readonly CloudTable _urlAnalyticsTable;
        private static readonly string UrlAnalyticsTableName = "UrlAnalytics";

        public UrlAnalyticsRepository(string storageConnString)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(storageConnString);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            _urlAnalyticsTable = cloudTableClient.GetTableReference(UrlAnalyticsTableName);
        }
        public async Task<bool> CreateOrUpdateUrlAnalytics(UrlAnalytics urlAnalytics)
        {
            await _urlAnalyticsTable.CreateIfNotExistsAsync();
            await _urlAnalyticsTable.ExecuteAsync(TableOperation.InsertOrReplace(urlAnalytics));
            return true;
        }

        public async Task<UrlAnalytics> GetUrlAnalytics(string partitionKey, string rowKey)
        {
            var tableResult = await _urlAnalyticsTable.ExecuteAsync(TableOperation.Retrieve<UrlAnalytics>(partitionKey, rowKey));
            return tableResult.Result as UrlAnalytics;
        }
    }
}
