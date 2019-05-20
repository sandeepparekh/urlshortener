namespace UrlShortener.Models.Common
{
    public interface IAppSettings
    {
        string DbConnectionString { get; set; }
        string EncodingAlphabet { get; set; }
        int ShortUrlCodeLength { get; set; }
        string ShortUrlDomain { get; set; }
        string CacheConnectionString { get; set; }
    }
    public class UrlShortenerSettings : IAppSettings
    {
        public string DbConnectionString { get; set; }
        public string EncodingAlphabet { get; set; }
        public int ShortUrlCodeLength { get; set; }
        public string ShortUrlDomain { get; set; }
        public string CacheConnectionString { get; set; }
    }
}
