namespace UrlShortener.Models.Azure
{
    public class UrlAnalyticsQueueMessage
    {
        public string ShortUrlCode { get; set; }
        public string UserAgent { get; set; }
        public string Referer { get; set; }
    }
}
