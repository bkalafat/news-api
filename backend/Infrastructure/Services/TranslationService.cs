using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Service for translating content to Turkish using free APIs
/// </summary>
public sealed class TranslationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TranslationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _targetLanguage = "tr"; // Turkish

    public TranslationService(
        HttpClient httpClient,
        ILogger<TranslationService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Translate text to Turkish using MyMemory free API (10,000 chars/day limit)
    /// </summary>
    public async Task<string> TranslateToTurkishAsync(string text, string sourceLanguage = "en")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        try
        {
            // Try MyMemory first (no API key required, 10k chars/day)
            var translated = await TranslateWithMyMemoryAsync(text, sourceLanguage);
            if (!string.IsNullOrEmpty(translated))
            {
                return translated;
            }

            // Fallback: Return original if translation fails
            _logger.LogWarning("Translation failed, returning original text");
            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating text");
            return text;
        }
    }

    /// <summary>
    /// Translate using MyMemory Translation API (free tier)
    /// </summary>
    private async Task<string> TranslateWithMyMemoryAsync(string text, string sourceLanguage)
    {
        try
        {
            // MyMemory has 500 char limit per request
            if (text.Length > 500)
            {
                return await TranslateLongTextAsync(text, sourceLanguage);
            }

            var encodedText = HttpUtility.UrlEncode(text);
            var url = $"https://api.mymemory.translated.net/get?q={encodedText}&langpair={sourceLanguage}|{_targetLanguage}";

            var response = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<MyMemoryResponse>(response);

            if (result?.ResponseData?.TranslatedText != null)
            {
                _logger.LogDebug("Successfully translated {Length} characters", text.Length);
                return result.ResponseData.TranslatedText;
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MyMemory translation failed");
            return string.Empty;
        }
    }

    /// <summary>
    /// Translate long text by splitting into chunks
    /// </summary>
    private async Task<string> TranslateLongTextAsync(string text, string sourceLanguage)
    {
        var chunks = SplitTextIntoChunks(text, 450); // Leave some margin
        var translatedChunks = new List<string>();

        foreach (var chunk in chunks)
        {
            var translated = await TranslateWithMyMemoryAsync(chunk, sourceLanguage);
            if (!string.IsNullOrEmpty(translated))
            {
                translatedChunks.Add(translated);
            }
            else
            {
                translatedChunks.Add(chunk); // Keep original if translation fails
            }

            // Rate limiting - wait between requests
            await Task.Delay(300);
        }

        return string.Join(" ", translatedChunks);
    }

    /// <summary>
    /// Split text into chunks at sentence boundaries
    /// </summary>
    private static List<string> SplitTextIntoChunks(string text, int maxLength)
    {
        var chunks = new List<string>();
        var sentences = text.Split(new[] { ". ", "! ", "? ", "\n" }, StringSplitOptions.None);
        var currentChunk = new StringBuilder();

        foreach (var sentence in sentences)
        {
            if (currentChunk.Length + sentence.Length > maxLength)
            {
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                }
            }

            currentChunk.Append(sentence);
            if (!sentence.EndsWith('.') && !sentence.EndsWith('!') && !sentence.EndsWith('?'))
            {
                currentChunk.Append(". ");
            }
        }

        if (currentChunk.Length > 0)
        {
            chunks.Add(currentChunk.ToString());
        }

        return chunks;
    }

    /// <summary>
    /// Detect language of text (simple heuristic)
    /// </summary>
    public string DetectLanguage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "en";
        }

        // Simple Turkish detection - check for Turkish characters
        var turkishChars = new[] { 'ı', 'ğ', 'ü', 'ş', 'ö', 'ç', 'İ', 'Ğ', 'Ü', 'Ş', 'Ö', 'Ç' };
        foreach (var c in turkishChars)
        {
            if (text.Contains(c))
            {
                return "tr"; // Already Turkish
            }
        }

        return "en"; // Assume English
    }

    #region DTO Classes

    private class MyMemoryResponse
    {
        public ResponseData? ResponseData { get; set; }
        public QuotaFinished? QuotaFinished { get; set; }
        public string? ResponseStatus { get; set; }
    }

    private class ResponseData
    {
        public string? TranslatedText { get; set; }
        public string? Match { get; set; }
    }

    private class QuotaFinished
    {
        public bool IsFinished { get; set; }
    }

    #endregion
}
