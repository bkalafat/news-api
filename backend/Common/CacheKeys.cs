namespace NewsApi.Common;

internal static class CacheKeys
{
    public static string NewsList => "_NewsList";
    public static string AllNews => "_AllNews";
    
    public static string GetNewsByCategory(string category) => $"_News_Category_{category}";
    
    public static string GetNewsByType(string type) => $"_News_Type_{type}";
}
