using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener.Models.Azure
{
    public class UrlAnalytics : TableEntity
    {
        public string Referrer { get; set; }
        public string Browser { get; set; }
        public string BrowserVer { get; set; }
        public string Country { get; set; }
        public string Os { get; set; }
        public string OsVer { get; set; }
    }
}
