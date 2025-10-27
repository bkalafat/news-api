# Automated News Aggregator

Multi-source news aggregation system that automatically fetches, translates, and publishes technology news.

## Features

- **Multiple News Sources**:
  - Reddit (r/technology, r/programming, r/science)
  - HackerNews top stories
  - Easy to add more sources

- **Automatic Translation**: English → Turkish using Google Translate
- **Smart Image Handling**:
  - Uses source images when available
  - Searches Unsplash for relevant images based on article title
  - Falls back to tech placeholder if nothing found
  
- **Duplicate Prevention**: Checks existing articles by slug before creating
- **Scheduled Execution**: Runs automatically every Sunday at 10:00 AM
- **Comprehensive Logging**: File and console logging for monitoring

## Installation

### 1. Install Dependencies

```bash
pip install requests schedule
```

### 2. Get Unsplash API Key (Optional but Recommended)

1. Go to https://unsplash.com/developers
2. Create a free developer account
3. Create a new application
4. Copy your Access Key
5. Edit `news_aggregator.py` and replace:
   ```python
   UNSPLASH_ACCESS_KEY = "YOUR_UNSPLASH_KEY"
   ```

**Note**: Without Unsplash key, it will use demo mode (limited) or fallback to placeholder images.

## Usage

### Run Once (Manual)

```bash
cd C:\dev\newsportal\scripts
python news_aggregator.py --once
```

### Run on Schedule

```bash
cd C:\dev\newsportal\scripts
python news_aggregator.py --schedule
```

This will:
- Run immediately once
- Then run every Sunday at 10:00 AM
- Keep running until you press Ctrl+C

### Run as Windows Service (Advanced)

For production, you can run this as a Windows service using `nssm` or Task Scheduler:

**Option 1: Windows Task Scheduler**

1. Open Task Scheduler
2. Create Basic Task → "News Aggregator"
3. Trigger: Weekly, Sunday, 10:00 AM
4. Action: Start a program
   - Program: `python.exe`
   - Arguments: `C:\dev\newsportal\scripts\news_aggregator.py --once`
   - Start in: `C:\dev\newsportal\scripts`

**Option 2: NSSM (Non-Sucking Service Manager)**

```bash
# Install NSSM
choco install nssm

# Create service
nssm install NewsAggregator python.exe
nssm set NewsAggregator AppDirectory C:\dev\newsportal\scripts
nssm set NewsAggregator AppParameters "C:\dev\newsportal\scripts\news_aggregator.py --schedule"

# Start service
nssm start NewsAggregator
```

## Configuration

### Add New News Source

Edit `news_aggregator.py` and add to `NEWS_SOURCES`:

```python
'your_source_name': {
    'enabled': True,
    'url': 'https://api.example.com/news',
    'params': {'limit': 10},
    'category': 'Your Category',
    'parser': 'your_parser'  # You'll need to implement parser function
}
```

### Disable a Source

Set `enabled: False` in NEWS_SOURCES:

```python
'reddit_science': {
    'enabled': False,  # This source will be skipped
    ...
}
```

### Change Schedule

Edit the schedule line in `run_scheduler()`:

```python
# Run every Sunday at 10:00
schedule.every().sunday.at("10:00").do(aggregate_news)

# Run every day at 6:00 AM
schedule.every().day.at("06:00").do(aggregate_news)

# Run every 6 hours
schedule.every(6).hours.do(aggregate_news)

# Run every Monday and Friday at 9:00
schedule.every().monday.at("09:00").do(aggregate_news)
schedule.every().friday.at("09:00").do(aggregate_news)
```

## How It Works

### Workflow

1. **Authentication**: Get JWT token from API
2. **Fetch**: Download posts from all enabled sources
3. **For each post**:
   - Translate title to Turkish
   - Generate slug from Turkish title
   - Check if article already exists (skip duplicates)
   - Find/search for relevant image:
     - Use source image if available (i.redd.it, i.imgur.com)
     - Search Unsplash with article keywords
     - Fallback to tech placeholder
   - Translate content (if available)
   - Create article via API
4. **Logging**: Record all operations to `news_aggregator.log`

### Image Priority

1. **Direct Image URLs**: If source provides i.redd.it or i.imgur.com
2. **Unsplash Search**: Search with article title keywords
3. **Fallback**: Generic tech placeholder image

### Duplicate Detection

Uses slug-based detection:
- Turkish title → slug (e.g., "Yapay Zeka Haberi" → "yapay-zeka-haberi")
- Checks if slug exists via API before creating
- Skips if article already exists

## Monitoring

### View Logs

```bash
# Real-time log monitoring
Get-Content news_aggregator.log -Wait -Tail 50

# View last 100 lines
Get-Content news_aggregator.log -Tail 100
```

### Check Service Status

If running as Windows service:

```bash
nssm status NewsAggregator
```

### Test Single Source

You can temporarily disable sources to test specific ones:

```python
NEWS_SOURCES = {
    'reddit_technology': {'enabled': True, ...},
    'reddit_programming': {'enabled': False, ...},  # Disabled
    'hackernews': {'enabled': False, ...},  # Disabled
}
```

## Troubleshooting

### Images Not Loading

1. Check Unsplash API key is set correctly
2. Verify API quota (5000 requests/hour for free tier)
3. Check logs for "Unsplash search failed"

### Authentication Failed

1. Verify backend is running: `docker-compose ps`
2. Check credentials in script match your admin user
3. Verify API is accessible: `curl http://localhost:5000/health`

### Duplicate Articles Being Created

1. Check slug generation is working correctly
2. Verify API endpoint `/NewsArticle/by-slug` is working
3. Check logs for "Article already exists" messages

### Translation Errors

Google Translate free endpoint may have rate limits:
- Add longer `time.sleep()` delays between translations
- Consider using paid translation service for production

## Performance

- **Rate Limiting**: Built-in delays to avoid API throttling
  - 0.5s between translations
  - 1s between article creations
  - 0.2s between HackerNews story fetches

- **Expected Runtime**: 
  - ~15 articles from Reddit = ~30-45 seconds
  - ~15 articles from HackerNews = ~45-60 seconds
  - Total: ~2-3 minutes for full aggregation

## Future Improvements

- [ ] Add more news sources (TechCrunch, Dev.to, etc.)
- [ ] Download images and upload to MinIO (avoid external dependencies)
- [ ] Better content extraction from URLs
- [ ] Category classification using AI/ML
- [ ] Sentiment analysis for article prioritization
- [ ] Email notifications on completion
- [ ] Web dashboard for monitoring
- [ ] Database cleanup of old articles
- [ ] Multi-language support (not just Turkish)

## Comparison with Manual Script

| Feature | fetch_reddit_news.py | news_aggregator.py |
|---------|---------------------|-------------------|
| Sources | Reddit only | Reddit + HackerNews (extensible) |
| Scheduling | Manual | Automatic (cron-like) |
| Image Search | Placeholder only | Unsplash search + placeholder |
| Duplicate Detection | None | Slug-based |
| Logging | Console only | File + console |
| Configuration | Hardcoded | Configurable sources |
| Error Handling | Basic | Comprehensive |

## Example Output

```
============================================================
🚀 Starting News Aggregation
============================================================
🔐 Authenticating...
✓ Authenticated successfully

📡 Processing: reddit_technology
------------------------------------------------------------
📰 Fetching from reddit_technology...
✓ Fetched 15 posts from reddit_technology

[1/15] New AI breakthrough in language models...
  🌐 Translating: New AI breakthrough in language models...
  🔍 Searching for image...
  📷 Found Unsplash image for: AI breakthrough language
  🌐 Translating content...
  ✓ Created article (ID: 68ff29764868af71e7d4321c)

[2/15] SpaceX launches new satellite constellation...
  🌐 Translating: SpaceX launches new satellite...
  ⏭️  Article already exists, skipping

...

✓ reddit_technology: Created 12/15 articles

============================================================
✓ Aggregation Complete!
  Total Fetched: 45
  Total Created: 35
  Duplicates Skipped: 10
============================================================
```

## License

Part of NewsPortal project. Same license applies.
