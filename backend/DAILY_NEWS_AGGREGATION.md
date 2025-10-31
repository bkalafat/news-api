# Daily News Aggregation System

## Overview

This system automatically fetches AI/tech news from multiple free sources, translates them to Turkish, and publishes them to your news portal **every day at 5:00 AM**.

## Features

### 🌐 News Sources (All Free!)

1. **Reddit** - AI and tech subreddits
   - r/artificial (AI discussions)
   - r/MachineLearning (ML community)
   - r/OpenAI (ChatGPT, GPT-4)
   - r/ClaudeAI (Anthropic Claude)
   - r/github (GitHub news)
   - r/programming (Programming trends)
   - r/technology (Tech news)

2. **GitHub Trending** - Hot repositories
   - Trending repos (all languages)
   - Popular AI/ML projects

3. **Hacker News** - Top tech stories
   - Top 30 stories daily
   - Tech startup news

4. **Dev.to** - Developer articles
   - Tags: ai, machinelearning, chatgpt, github, copilot
   - Top articles from past week

5. **Medium** - Tech blogs (via RSS)
   - artificial-intelligence
   - machine-learning
   - chatgpt
   - github-copilot

6. **Ars Technica** - Professional tech journalism
   - Technology Lab RSS feed

7. **TechCrunch** - Startup & tech news
   - Main RSS feed

### 🇹🇷 Turkish Translation

- **MyMemory Translation API** (Free tier: 10,000 chars/day)
- Automatic language detection
- Smart chunking for long texts
- Rate limiting to stay within quota

### 📊 Intelligent Prioritization

News articles are prioritized based on:
- **Score/upvotes** from source
- **Source quality** (TechCrunch, Ars Technica get highest priority)
- **Recency** (newer articles preferred)

Priority formula:
```
Priority = min(score/10, 50) + source_boost
- Ars Technica: +25
- TechCrunch: +25
- Hacker News: +20
- GitHub Trending: +15
- Dev.to: +10
- Medium: +10
- Reddit: +5
```

### 🗑️ Automatic Cache Clearing

After publishing new articles, the system automatically clears:
- All news cache
- Category-specific caches
- Type-specific caches

This ensures users see fresh content immediately.

## How It Works

### Daily Automated Job (5:00 AM)

The `DailyNewsAggregatorJob` background service:

1. **Runs at 5:00 AM** every day (Turkish time)
2. **Fetches news** from all 7+ sources in parallel
3. **Filters top 50** most relevant items
4. **Detects duplicates** by slug (avoids republishing)
5. **Translates to Turkish** (title, summary, content)
6. **Saves to database** with proper metadata
7. **Clears all caches** to refresh frontend

### Manual Trigger (For Testing)

You can manually trigger the aggregation process:

```bash
POST /api/Seed/aggregate-and-publish
```

**Example with curl:**
```bash
curl -X POST http://localhost:5000/api/Seed/aggregate-and-publish
```

**Response:**
```json
{
  "message": "News aggregation and publishing completed successfully!",
  "durationSeconds": 45.23,
  "fetched": 127,
  "created": 50,
  "skipped": 12,
  "errors": 0,
  "sources": [
    "Reddit (AI/Tech subreddits)",
    "GitHub Trending",
    "Hacker News",
    "Dev.to",
    "Medium",
    "Ars Technica",
    "TechCrunch"
  ]
}
```

## Configuration

### appsettings.json

```json
{
  "NewsAggregatorSettings": {
    "ScheduleTime": "05:00:00",
    "MaxNewsItems": 50,
    "TargetLanguage": "tr",
    "SourceLanguage": "en",
    "EnableTranslation": true,
    "TranslationProvider": "MyMemory"
  }
}
```

### Environment Variables (Optional)

```bash
# None required! All sources are free and don't need API keys
```

## Translation Limits

### MyMemory API
- **Free tier**: 10,000 characters per day
- **Per request**: 500 characters max
- **Strategy**: System splits long texts into chunks
- **Fallback**: If quota exceeded, original English text is kept

**Daily capacity**: ~50 articles with 200-char summaries

To increase capacity:
1. Use multiple translation services (round-robin)
2. Upgrade to MyMemory paid tier
3. Self-host LibreTranslate (unlimited, free)

## Monitoring

### Logs

Check logs for aggregation status:

```bash
docker logs -f newsportal-backend | grep "DailyNewsAggregatorJob"
```

**Expected output:**
```
[2025-10-31 05:00:01] Daily News Aggregator Job started
[2025-10-31 05:00:02] Fetching news from all sources...
[2025-10-31 05:00:15] Fetched 127 news items
[2025-10-31 05:00:15] Processing top 50 news items
[2025-10-31 05:02:30] Published: [Technology] Claude 3.5 Sonnet Beats GPT-4
[2025-10-31 05:03:45] News aggregation complete: 50 published, 12 skipped, 0 failed
[2025-10-31 05:03:46] All news caches cleared
```

### Health Check

The background job status is included in health checks:

```bash
curl http://localhost:5000/health
```

## Troubleshooting

### Issue: Translation quota exceeded

**Symptoms**: Articles appear in English instead of Turkish

**Solution**: 
1. Wait until next day (quota resets daily)
2. Reduce `MaxNewsItems` in settings
3. Consider self-hosting LibreTranslate

### Issue: No news fetched

**Symptoms**: `fetched: 0` in response

**Possible causes**:
- Network connectivity issues
- Reddit/API rate limits
- Firewall blocking requests

**Solution**:
```bash
# Check backend can reach external APIs
docker exec -it newsportal-backend curl -I https://www.reddit.com
```

### Issue: Duplicate articles

**Symptoms**: Same news appearing multiple times

**Cause**: Slug collision (different titles generate same slug)

**Solution**: System automatically skips duplicates by checking existing slugs

### Issue: Job not running at 5 AM

**Check logs**:
```bash
docker logs newsportal-backend | grep "Next run scheduled"
```

**Verify timezone**: Container should use your local timezone

## Removing Old Seed Data

The old static seed data (`news_export.json`) has been removed. The system now:
- ✅ Fetches live news daily
- ✅ Translates to Turkish automatically
- ✅ No manual seed data needed
- ✅ Always fresh, trending content

## API Endpoints

### Manual News Aggregation
```
POST /api/Seed/aggregate-and-publish
```
Triggers the daily job immediately (for testing)

### Old Endpoints (Still Available)
```
POST /api/Seed/news                    # Static seed data (deprecated)
POST /api/Seed/reddit                  # Reddit only (deprecated)
POST /api/Seed/fetch-external-news     # External API (requires API key)
```

## Future Enhancements

Potential improvements:
- [ ] Add more sources (The Verge, Wired, etc.)
- [ ] Support multiple languages (not just Turkish)
- [ ] AI-powered content summarization
- [ ] Sentiment analysis
- [ ] Image extraction from articles
- [ ] Email digest of daily news
- [ ] Webhook notifications on new articles

## Performance

### Current Stats
- **Sources**: 7+ concurrent fetches
- **Articles fetched**: ~120-150 per day
- **Top articles published**: 50 per day
- **Translation time**: ~2-3 minutes (with rate limiting)
- **Total duration**: ~3-5 minutes

### Resource Usage
- **Memory**: ~100 MB during aggregation
- **Network**: ~5-10 MB download
- **Database**: ~2 MB per day (50 articles)

## Development

### Running Locally

The background job starts automatically when you run the backend:

```bash
docker compose up -d
```

**To test without waiting until 5 AM:**
```bash
curl -X POST http://localhost:5000/api/Seed/aggregate-and-publish
```

### Adjusting Schedule

Edit `DailyNewsAggregatorJob.cs`:

```csharp
// Change from 5 AM to 8 AM
private static readonly TimeSpan TargetTime = new(8, 0, 0);
```

Or use cron-based scheduling (future enhancement).

## License

Part of News Portal project. See main README for license.

## Support

For issues or questions:
1. Check logs: `docker logs newsportal-backend`
2. Open GitHub issue
3. Review API responses for error details

---

**Last Updated**: October 31, 2025  
**System Status**: ✅ Production Ready
