using System;
using System.Text.RegularExpressions;

namespace NewsApi.Common;

internal static class SlugHelper
{
    /// <summary>
    /// Generates a URL-friendly slug from Turkish text
    /// Example: "Yapay Zeka Çağında Yeni Gelişmeler" -> "yapay-zeka-caginda-yeni-gelismeler"
    /// </summary>
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Replace Turkish characters
        text = text.Replace("ı", "i", StringComparison.Ordinal)
            .Replace("ğ", "g", StringComparison.Ordinal)
            .Replace("ü", "u", StringComparison.Ordinal)
            .Replace("ş", "s", StringComparison.Ordinal)
            .Replace("ö", "o", StringComparison.Ordinal)
            .Replace("ç", "c", StringComparison.Ordinal)
            .Replace("İ", "i", StringComparison.Ordinal)
            .Replace("Ğ", "g", StringComparison.Ordinal)
            .Replace("Ü", "u", StringComparison.Ordinal)
            .Replace("Ş", "s", StringComparison.Ordinal)
            .Replace("Ö", "o", StringComparison.Ordinal)
            .Replace("Ç", "c", StringComparison.Ordinal);

        // Remove special characters and replace spaces with hyphens
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "", RegexOptions.NonBacktracking);
        text = Regex.Replace(text, @"\s+", " ", RegexOptions.NonBacktracking).Trim();
        text = Regex.Replace(text, @"\s", "-", RegexOptions.NonBacktracking);
        text = Regex.Replace(text, "-+", "-", RegexOptions.NonBacktracking);

        // Limit length to 100 characters
        if (text.Length > 100)
        {
            text = text.Substring(0, 100).TrimEnd('-');
        }

        return text;
    }
}
