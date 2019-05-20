namespace UrlShortener.Services
{
    public interface ICacheService
    {
        void SetCache(string key, string value);

        string GetCache(string key);
    }
}