using System.Text;
using System.Text.RegularExpressions;

namespace NewsApi.Common;

public static class SlugHelper
{
    /// <summary>
    /// Generates a URL-friendly slug from Turkish text
    /// Example: "Yapay Zeka Çağında Yeni Gelişmeler" -> "yapay-zeka-caginda-yeni-gelismeler"
    /// </summary>
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Replace Turkish characters
        text = text.Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c")
            .Replace("İ", "i")
            .Replace("Ğ", "g")
            .Replace("Ü", "u")
            .Replace("Ş", "s")
            .Replace("Ö", "o")
            .Replace("Ç", "c");

        // Remove special characters and replace spaces with hyphens
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        text = Regex.Replace(text, @"\s", "-");
        text = Regex.Replace(text, @"-+", "-");

        // Limit length to 100 characters
        if (text.Length > 100)
            text = text.Substring(0, 100).TrimEnd('-');

        return text;
    }
}
