using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Service for intelligently detecting news categories based on content, source, and engagement
/// </summary>
internal sealed class CategoryDetectionService
{
    private readonly ILogger<CategoryDetectionService> _logger;

    // Category keywords with weights
    private static readonly Dictionary<string, (string Category, List<string> Keywords, int Weight)> CategoryPatterns = new()
    {
        // Technology - AI, ML, Programming
        ["tech_ai"] = ("Technology", new List<string>
        {
            "ai", "artificial intelligence", "machine learning", "ml", "deep learning",
            "neural network", "chatgpt", "gpt-4", "claude", "llm", "openai", "anthropic",
            "copilot", "github copilot", "transformer", "bert", "nlp"
        }, 100),

        ["tech_programming"] = ("Technology", new List<string>
        {
            "programming", "code", "developer", "software", "github", "git",
            "python", "javascript", "typescript", "rust", "go", "java", "c#",
            "framework", "library", "api", "sdk", "docker", "kubernetes"
        }, 90),

        ["tech_web"] = ("Technology", new List<string>
        {
            "web", "frontend", "backend", "react", "vue", "angular", "next.js",
            "node.js", "express", "fastapi", "django", "flask"
        }, 85),

        // Science - Research, Physics, Space
        ["science_research"] = ("Science", new List<string>
        {
            "research", "study", "scientist", "discovery", "breakthrough",
            "experiment", "analysis", "data science", "statistics"
        }, 95),

        ["science_space"] = ("Science", new List<string>
        {
            "space", "nasa", "spacex", "rocket", "mars", "moon", "astronomy",
            "telescope", "satellite", "iss", "astronaut"
        }, 90),

        ["science_physics"] = ("Science", new List<string>
        {
            "physics", "quantum", "particle", "atom", "nuclear", "energy",
            "cern", "black hole", "relativity"
        }, 90),

        // Business - Startups, Finance, Economy
        ["business_startup"] = ("Business", new List<string>
        {
            "startup", "founder", "vc", "venture capital", "funding", "investment",
            "unicorn", "ipo", "acquisition", "merger", "y combinator"
        }, 85),

        ["business_finance"] = ("Business", new List<string>
        {
            "stock", "market", "trading", "crypto", "bitcoin", "ethereum",
            "finance", "bank", "economy", "inflation", "recession"
        }, 80),

        // Health - Medicine, Biotech
        ["health_medical"] = ("Health", new List<string>
        {
            "health", "medical", "doctor", "hospital", "medicine", "drug",
            "vaccine", "disease", "covid", "pandemic", "treatment", "therapy"
        }, 85),

        ["health_biotech"] = ("Health", new List<string>
        {
            "biotech", "biology", "genetics", "dna", "crispr", "gene",
            "pharmaceutical", "clinical trial"
        }, 85),

        // Entertainment - Gaming, Movies
        ["entertainment_gaming"] = ("Entertainment", new List<string>
        {
            "game", "gaming", "esports", "xbox", "playstation", "nintendo",
            "steam", "unity", "unreal", "indie game"
        }, 75),

        ["entertainment_media"] = ("Entertainment", new List<string>
        {
            "movie", "film", "netflix", "streaming", "tv show", "series",
            "hollywood", "actor", "director"
        }, 70),

        // World - Politics, Global Events
        ["world_politics"] = ("World", new List<string>
        {
            "politics", "government", "election", "president", "congress",
            "parliament", "policy", "regulation", "law", "legislation"
        }, 80),

        ["world_global"] = ("World", new List<string>
        {
            "international", "global", "world", "country", "nation",
            "united nations", "un", "eu", "nato"
        }, 75),
    };

    public CategoryDetectionService(ILogger<CategoryDetectionService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Detect the most appropriate category based on title, content, source, and engagement metrics
    /// </summary>
    /// <param name="title">News title</param>
    /// <param name="content">News content</param>
    /// <param name="source">News source (e.g., "Reddit - r/programming")</param>
    /// <param name="tags">Tags associated with the news</param>
    /// <param name="score">Engagement score (upvotes, likes, etc.)</param>
    /// <returns>The detected category</returns>
    public string DetectCategory(
        string title,
        string content,
        string source,
        List<string> tags,
        int score)
    {
        var categoryScores = new Dictionary<string, int>();

        // Combine all text for analysis
        var combinedText = $"{title} {content} {source} {string.Join(" ", tags)}".ToLowerInvariant();

        // Score based on keyword matching
        foreach (var (patternKey, (category, keywords, weight)) in CategoryPatterns)
        {
            var matches = keywords.Count(keyword => 
                Regex.IsMatch(combinedText, $@"\b{Regex.Escape(keyword)}\b", RegexOptions.IgnoreCase));

            if (matches > 0)
            {
                if (!categoryScores.ContainsKey(category))
                {
                    categoryScores[category] = 0;
                }
                categoryScores[category] += matches * weight;
            }
        }

        // Boost category based on source
        var sourceCategory = GetCategoryFromSource(source);
        if (!string.IsNullOrEmpty(sourceCategory) && categoryScores.ContainsKey(sourceCategory))
        {
            categoryScores[sourceCategory] += 50; // Source boost
        }

        // Boost category based on high engagement (viral content)
        if (score > 1000)
        {
            // High engagement usually indicates important tech or world news
            if (categoryScores.ContainsKey("Technology"))
            {
                categoryScores["Technology"] += 30;
            }
            if (categoryScores.ContainsKey("World"))
            {
                categoryScores["World"] += 20;
            }
        }

        // Select category with highest score
        if (categoryScores.Any())
        {
            var detectedCategory = categoryScores.OrderByDescending(x => x.Value).First().Key;
            _logger.LogDebug(
                "Detected category '{Category}' for '{Title}' (scores: {Scores})",
                detectedCategory,
                title.Length > 50 ? title[..50] + "..." : title,
                string.Join(", ", categoryScores.Select(x => $"{x.Key}:{x.Value}")));
            return detectedCategory;
        }

        // Default to Technology for tech-heavy sources
        var defaultCategory = GetDefaultCategoryFromSource(source);
        _logger.LogDebug("Using default category '{Category}' for source '{Source}'", defaultCategory, source);
        return defaultCategory;
    }

    /// <summary>
    /// Get category hint from news source
    /// </summary>
    private static string GetCategoryFromSource(string source)
    {
        var lowerSource = source.ToLowerInvariant();

        // Reddit subreddit mapping
        if (lowerSource.Contains("artificial") || lowerSource.Contains("machinelearning") ||
            lowerSource.Contains("openai") || lowerSource.Contains("claudeai"))
        {
            return "Science";
        }

        if (lowerSource.Contains("programming") || lowerSource.Contains("github"))
        {
            return "Technology";
        }

        // Source-based detection
        if (lowerSource.Contains("techcrunch") || lowerSource.Contains("ars technica") ||
            lowerSource.Contains("hacker news"))
        {
            return "Technology";
        }

        return string.Empty;
    }

    /// <summary>
    /// Get default category when no strong signals detected
    /// </summary>
    private static string GetDefaultCategoryFromSource(string source)
    {
        var lowerSource = source.ToLowerInvariant();

        // For most tech sources, default to Technology
        if (lowerSource.Contains("reddit") || lowerSource.Contains("github") ||
            lowerSource.Contains("dev.to") || lowerSource.Contains("medium") ||
            lowerSource.Contains("hacker news") || lowerSource.Contains("techcrunch") ||
            lowerSource.Contains("ars technica"))
        {
            return "Technology";
        }

        // Default fallback
        return "Technology";
    }

    /// <summary>
    /// Get top trending categories from a list of aggregated news items
    /// </summary>
    public Dictionary<string, int> GetTrendingCategories(List<AggregatedNewsItem> newsItems)
    {
        var categoryEngagement = new Dictionary<string, int>();

        foreach (var item in newsItems)
        {
            var category = DetectCategory(item.Title, item.Content, item.Source, item.Tags, item.Score);
            
            if (!categoryEngagement.ContainsKey(category))
            {
                categoryEngagement[category] = 0;
            }
            categoryEngagement[category] += item.Score;
        }

        return categoryEngagement.OrderByDescending(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}
