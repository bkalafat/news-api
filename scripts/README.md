# Reddit Technology News Fetcher

Automatically fetches top technology news from Reddit's `/r/technology` subreddit, translates them to Turkish, and imports them into the NewsPortal database.

## Features

- ✅ Fetches top technology posts from the past week
- ✅ Translates titles and content to Turkish using Google Translate API
- ✅ Downloads article images
- ✅ Creates properly formatted news articles
- ✅ Automatically authenticates with the NewsPortal API
- ✅ Inserts articles into MongoDB via REST API

## Requirements

- Python 3.8+
- `requests` library (`pip install requests`)
- Backend API running on `http://localhost:5000`
- MongoDB running with data
- Admin credentials (default: admin/admin123)

## Usage

```bash
cd scripts
python fetch_reddit_news.py
```

## Configuration

Edit the constants at the top of the script:

```python
REDDIT_URL = "https://www.reddit.com/r/technology/top.json"
API_BASE_URL = "http://localhost:5000/api"
ADMIN_USERNAME = "admin"
ADMIN_PASSWORD = "admin123"
```

## How It Works

1. **Authentication**: Logs in to get JWT token
2. **Fetch Posts**: Gets top 15 posts from r/technology (past week)
3. **Translation**: Translates title and content to Turkish
4. **Image Processing**: Extracts image URLs from Reddit posts
5. **Article Creation**: Creates article with all required fields
6. **API Upload**: Posts to `/api/NewsArticle` endpoint

## Article Fields Populated

- `category`: "Teknoloji"
- `type`: "Haber"
- `caption`: Translated title (max 500 chars)
- `summary`: Translated title (max 200 chars)
- `content`: Translated content with source link
- `imageUrl`: Reddit post thumbnail/preview image
- `thumbnailUrl`: Same as imageUrl
- `subjects`: ["Teknoloji"]
- `authors`: ["Reddit Technology"]
- `expressDate`: Current UTC datetime
- `priority`: 1
- `isActive`: true

## Success Rate

Typically creates **10-14 articles** out of 15 fetched posts:
- Skips posts without valid images
- Skips posts with "self" or "default" thumbnails
- Rate limits translations to avoid API blocks

## Output Example

```
==================================================
Reddit Technology News Fetcher
==================================================

🔐 Authenticating...
✓ Authenticated successfully

📰 Fetching Reddit posts...
✓ Fetched 15 posts from Reddit

Processing 15 articles...

[1/15] Bill Gates warns AI will take over most jobs...
  🌐 Translating title...
  ✓ Created article (ID: 68ff1e4b4868af71e7d431db)

[2/15] AWS crash causes $2,000 Smart Beds to overheat...
  🌐 Translating title...
  ✓ Created article (ID: 68ff1e4d4868af71e7d431dd)

...

==================================================
✓ Successfully created 14/15 articles
==================================================

🌐 Visit http://localhost:3000 to see the news!
```

## Troubleshooting

**Error: Authentication failed**
- Ensure backend API is running on port 5000
- Check admin credentials are correct

**Error: 404 Not Found**
- Verify API endpoint is `/api/NewsArticle` not `/api/News`

**Error: Translation failed**
- Google Translate API may be rate limiting
- Script automatically falls back to English if translation fails

**No articles created**
- Check that Reddit posts have valid images
- Review error messages for specific failures

## Future Improvements

- Upload images to MinIO instead of using Reddit URLs
- Support for multiple news sources (HackerNews, TechCrunch, etc.)
- Better content extraction and formatting
- Scheduled automatic execution (cron job)
- Duplicate detection to avoid re-importing same articles
