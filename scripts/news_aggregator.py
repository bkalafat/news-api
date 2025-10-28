"""
Automated News Aggregator - Multi-source news fetcher with automatic scheduling
Fetches technology news from multiple sources, translates to Turkish, finds relevant images

Sources:
- Reddit (technology, programming, science, AI, robotics, LocalLLM, türkiye, phones)
- GitHub Trending repositories
- HackerNews top stories
- Twitter/X trending topics (via nitter)
- Turkish tech news sites
"""

import requests
import json
import time
import schedule
import hashlib
import re
from datetime import datetime, timedelta
from typing import List, Dict, Optional
import logging
import pytz

# Configuration
API_BASE_URL = "http://localhost:5000/api"
ADMIN_USERNAME = "admin"
ADMIN_PASSWORD = "admin123"
UNSPLASH_ACCESS_KEY = "ATKlZDXPJKfdlYpmi0AY1QNTvjzNd6ymQdH3JRP3qWU"  # Get free key from https://unsplash.com/developers

# Timezone configuration
TURKEY_TZ = pytz.timezone('Europe/Istanbul')

# Setup logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler('news_aggregator.log'),
        logging.StreamHandler()
    ]
)
logger = logging.getLogger(__name__)

# News Sources Configuration
# Kategoriler backend ile uyumlu (İngilizce)
NEWS_SOURCES = {
    # Reddit Sources
    'reddit_technology': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/technology/top.json',
        'params': {'t': 'week', 'limit': 20},
        'category': 'technology',
        'parser': 'reddit'
    },
    'reddit_programming': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/programming/top.json',
        'params': {'t': 'week', 'limit': 15},
        'category': 'technology',
        'parser': 'reddit'
    },
    'reddit_artificial': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/artificial/top.json',
        'params': {'t': 'week', 'limit': 15},
        'category': 'technology',
        'parser': 'reddit'
    },
    'reddit_localllama': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/LocalLLaMA/top.json',
        'params': {'t': 'week', 'limit': 15},
        'category': 'technology',
        'parser': 'reddit'
    },
    'reddit_robotics': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/robotics/top.json',
        'params': {'t': 'week', 'limit': 15},
        'category': 'science',
        'parser': 'reddit'
    },
    'reddit_github': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/github/top.json',
        'params': {'t': 'week', 'limit': 10},
        'category': 'technology',
        'parser': 'reddit'
    },
    'reddit_androidapps': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/Android/top.json',
        'params': {'t': 'week', 'limit': 10},
        'category': 'technology',
        'parser': 'reddit'
    },
    'reddit_iphone': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/iphone/top.json',
        'params': {'t': 'week', 'limit': 10},
        'category': 'technology',
        'parser': 'reddit'
    },
    'reddit_turkey': {
        'enabled': True,
        'url': 'https://www.reddit.com/r/Turkey/top.json',
        'params': {'t': 'week', 'limit': 10},
        'category': 'world',
        'parser': 'reddit'
    },
    
    # GitHub Trending
    'github_trending_daily': {
        'enabled': True,
        'url': 'https://api.github.com/search/repositories',
        'params': {
            'q': 'created:>=' + (datetime.now() - timedelta(days=7)).strftime('%Y-%m-%d'),
            'sort': 'stars',
            'order': 'desc',
            'per_page': 15
        },
        'category': 'technology',
        'parser': 'github'
    },
    
    # HackerNews
    'hackernews': {
        'enabled': True,
        'url': 'https://hacker-news.firebaseio.com/v0/topstories.json',
        'limit': 20,
        'category': 'technology',
        'parser': 'hackernews'
    },
    
    # Turkish Tech News (Web scraping alternative - RSS feeds)
    'webrazzi': {
        'enabled': True,
        'url': 'https://webrazzi.com/feed/',
        'category': 'business',
        'parser': 'rss'
    },
    'shiftdelete': {
        'enabled': True,
        'url': 'https://shiftdelete.net/feed',
        'category': 'technology',
        'parser': 'rss'
    }
}

def slugify(text: str) -> str:
    """Convert text to URL-friendly slug"""
    text = text.lower()
    replacements = {'ş': 's', 'ğ': 'g', 'ü': 'u', 'ö': 'o', 'ç': 'c', 'ı': 'i',
                   'İ': 'i', 'Ş': 's', 'Ğ': 'g', 'Ü': 'u', 'Ö': 'o', 'Ç': 'c'}
    for tr, en in replacements.items():
        text = text.replace(tr, en)
    text = re.sub(r'[^a-z0-9\s-]', '', text)
    text = re.sub(r'\s+', '-', text)
    text = re.sub(r'-+', '-', text)
    return text.strip('-')

def translate_text(text: str, source='en', target='tr') -> str:
    """Translate text using Google Translate"""
    if not text or len(text.strip()) == 0:
        return text
    
    try:
        url = f"https://translate.googleapis.com/translate_a/single"
        params = {
            'client': 'gtx',
            'sl': source,
            'tl': target,
            'dt': 't',
            'q': text[:5000]
        }
        response = requests.get(url, params=params, timeout=10)
        response.raise_for_status()
        result = response.json()
        translated = ''.join([item[0] for item in result[0] if item[0]])
        return translated
    except Exception as e:
        logger.warning(f"Translation failed: {e}")
        return text

def search_unsplash_image(query: str) -> Optional[str]:
    """Search for relevant image on Unsplash based on keywords"""
    try:
        # Use demo access (limited) or add your own key
        url = "https://api.unsplash.com/search/photos"
        
        # Clean query for better results
        search_query = re.sub(r'[^\w\s]', '', query).strip()
        words = search_query.split()[:3]  # Use first 3 words
        search_term = ' '.join(words)
        
        params = {
            'query': search_term,
            'per_page': 1,
            'orientation': 'landscape',
            'client_id': UNSPLASH_ACCESS_KEY if UNSPLASH_ACCESS_KEY != "YOUR_UNSPLASH_KEY" else 'demo'
        }
        
        response = requests.get(url, params=params, timeout=10)
        
        if response.status_code == 200:
            data = response.json()
            if data.get('results') and len(data['results']) > 0:
                image_url = data['results'][0]['urls']['regular']
                logger.info(f"  📷 Found Unsplash image for: {search_term}")
                return image_url
        
        # Fallback to generic tech image
        logger.info(f"  ⚠️  No specific image found, using tech placeholder")
        return "https://images.unsplash.com/photo-1488590528505-98d2b5aba04b?w=800&auto=format&fit=crop"
        
    except Exception as e:
        logger.warning(f"Unsplash search failed: {e}")
        return "https://images.unsplash.com/photo-1488590528505-98d2b5aba04b?w=800&auto=format&fit=crop"

def get_auth_token() -> str:
    """Get JWT token from API"""
    try:
        response = requests.post(
            f"{API_BASE_URL}/Auth/login",
            json={"username": ADMIN_USERNAME, "password": ADMIN_PASSWORD},
            timeout=10
        )
        response.raise_for_status()
        return response.json()['token']
    except Exception as e:
        logger.error(f"Authentication failed: {e}")
        raise

def parse_reddit_posts(data: Dict, source_config: Dict) -> List[Dict]:
    """Parse Reddit JSON response"""
    posts = []
    children = data.get('data', {}).get('children', [])
    
    for post in children:
        post_data = post['data']
        
        # Extract image URL
        image_url = None
        if post_data.get('url') and ('i.redd.it' in post_data['url'] or 'i.imgur.com' in post_data['url']):
            image_url = post_data['url']
        
        posts.append({
            'title': post_data['title'],
            'content': post_data.get('selftext', ''),
            'url': f"https://reddit.com{post_data['permalink']}",
            'image_url': image_url,
            'score': post_data.get('score', 0),
            'category': source_config['category']
        })
    
    return posts

def parse_hackernews_story(story_id: int) -> Optional[Dict]:
    """Fetch and parse single HackerNews story"""
    try:
        url = f"https://hacker-news.firebaseio.com/v0/item/{story_id}.json"
        response = requests.get(url, timeout=5)
        response.raise_for_status()
        story = response.json()
        
        if story and story.get('type') == 'story' and not story.get('dead'):
            return {
                'title': story.get('title', ''),
                'content': story.get('text', ''),
                'url': story.get('url', f"https://news.ycombinator.com/item?id={story_id}"),
                'image_url': None,
                'score': story.get('score', 0),
                'category': 'technology'
            }
    except:
        pass
    return None

def parse_github_repos(data: Dict, source_config: Dict) -> List[Dict]:
    """Parse GitHub trending repositories"""
    repos = []
    items = data.get('items', [])
    
    for repo in items[:15]:  # Limit to 15
        description = repo.get('description', '')
        stars = repo.get('stargazers_count', 0)
        language = repo.get('language', 'Unknown')
        
        title = f"{repo['full_name']} - {language} projesi ({stars:,} yıldız)"
        content = description if description else "Popüler GitHub projesi"
        
        repos.append({
            'title': title,
            'content': content,
            'url': repo['html_url'],
            'image_url': None,
            'score': stars,
            'category': source_config['category']
        })
    
    return repos

def parse_rss_feed(xml_content: str, source_config: Dict) -> List[Dict]:
    """Parse RSS feed (basic XML parsing without external library)"""
    posts = []
    
    # Simple regex-based RSS parsing (works for most feeds)
    items = re.findall(r'<item>(.*?)</item>', xml_content, re.DOTALL)
    
    for item in items[:15]:  # Limit to 15
        # Extract title
        title_match = re.search(r'<title>(.*?)</title>', item, re.DOTALL)
        title = title_match.group(1).strip() if title_match else ''
        title = re.sub(r'<!\[CDATA\[(.*?)\]\]>', r'\1', title)
        
        # Extract link
        link_match = re.search(r'<link>(.*?)</link>', item, re.DOTALL)
        url = link_match.group(1).strip() if link_match else ''
        
        # Extract description
        desc_match = re.search(r'<description>(.*?)</description>', item, re.DOTALL)
        description = desc_match.group(1).strip() if desc_match else ''
        description = re.sub(r'<!\[CDATA\[(.*?)\]\]>', r'\1', description)
        description = re.sub(r'<[^>]+>', '', description)  # Remove HTML tags
        
        # Extract image from content:encoded or media:content
        image_url = None
        img_match = re.search(r'<img[^>]+src=["\']([^"\']+)["\']', item)
        if img_match:
            image_url = img_match.group(1)
        
        if title and url:
            posts.append({
                'title': title,
                'content': description[:500] if description else title,
                'url': url,
                'image_url': image_url,
                'score': 0,
                'category': source_config['category']
            })
    
    return posts

def fetch_hackernews_posts(limit: int = 15) -> List[Dict]:
    """Fetch top stories from HackerNews"""
    try:
        response = requests.get('https://hacker-news.firebaseio.com/v0/topstories.json', timeout=10)
        response.raise_for_status()
        story_ids = response.json()[:limit]
        
        posts = []
        for story_id in story_ids:
            story = parse_hackernews_story(story_id)
            if story:
                posts.append(story)
            time.sleep(0.2)  # Rate limiting
        
        return posts
    except Exception as e:
        logger.error(f"HackerNews fetch failed: {e}")
        return []

def fetch_source(source_name: str, config: Dict) -> List[Dict]:
    """Fetch posts from a news source"""
    try:
        logger.info(f"📰 Fetching from {source_name}...")
        
        if config['parser'] == 'reddit':
            headers = {'User-Agent': 'NewsPortal/1.0'}
            response = requests.get(config['url'], headers=headers, params=config.get('params', {}), timeout=15)
            response.raise_for_status()
            posts = parse_reddit_posts(response.json(), config)
            
        elif config['parser'] == 'hackernews':
            posts = fetch_hackernews_posts(config.get('limit', 15))
            
        elif config['parser'] == 'github':
            headers = {'Accept': 'application/vnd.github.v3+json'}
            response = requests.get(config['url'], headers=headers, params=config.get('params', {}), timeout=15)
            response.raise_for_status()
            posts = parse_github_repos(response.json(), config)
            
        elif config['parser'] == 'rss':
            headers = {'User-Agent': 'NewsPortal/1.0'}
            response = requests.get(config['url'], headers=headers, timeout=15)
            response.raise_for_status()
            posts = parse_rss_feed(response.text, config)
        
        else:
            logger.warning(f"Unknown parser: {config['parser']}")
            return []
        
        logger.info(f"✓ Fetched {len(posts)} posts from {source_name}")
        return posts
        
    except Exception as e:
        logger.error(f"Failed to fetch from {source_name}: {e}")
        return []

def article_exists(slug: str, token: str) -> bool:
    """Check if article with slug already exists"""
    try:
        headers = {'Authorization': f'Bearer {token}'}
        response = requests.get(
            f"{API_BASE_URL}/NewsArticle/by-slug",
            params={'slug': slug},
            headers=headers,
            timeout=10
        )
        return response.status_code == 200
    except:
        return False

def create_article(post: Dict, token: str) -> bool:
    """Create news article from post"""
    try:
        # Translate title if in English
        title = post['title']
        is_turkish = any(char in title.lower() for char in 'ğüşıöçĞÜŞİÖÇ')
        
        if not is_turkish and len(title) > 0:
            logger.info(f"  🌐 Translating: {title[:60]}...")
            title_tr = translate_text(title)
            time.sleep(0.5)
        else:
            title_tr = title
        
        # Generate slug and check if exists
        slug = slugify(title_tr)
        if article_exists(slug, token):
            logger.info(f"  ⏭️  Article already exists, skipping")
            return False
        
        # Get or search for image
        image_url = post.get('image_url')
        if not image_url:
            logger.info(f"  🔍 Searching for image...")
            image_url = search_unsplash_image(post['title'])
            time.sleep(0.5)
        
        # Translate content if in English
        content_en = post.get('content', '')
        if content_en and len(content_en) > 50:
            if not is_turkish:
                logger.info(f"  🌐 Translating content...")
                content_tr = translate_text(content_en[:2000])
                time.sleep(0.5)
            else:
                content_tr = content_en
            content_html = f"<p>{content_tr}</p>"
        else:
            content_html = f"<p>{title_tr}</p>"
        
        # Add source link
        content_html += f"<p><strong>Kaynak:</strong> <a href='{post['url']}' target='_blank'>Haberin devamını oku</a></p>"
        
        # Prepare article
        article = {
            "category": post['category'],
            "type": "Haber",
            "caption": title_tr[:500],
            "keywords": f"{post['category'].lower()}, teknoloji, haber",
            "socialTags": f"#{post['category'].lower().replace(' ', '')} #teknoloji",
            "summary": title_tr[:200],
            "imgPath": "",
            "imgAlt": title_tr[:200],
            "imageUrl": image_url,
            "thumbnailUrl": image_url,
            "content": content_html,
            "subjects": [post['category']],
            "authors": ["News Aggregator"],
            "expressDate": datetime.utcnow().isoformat() + "Z",
            "priority": 1,
            "isActive": True,
            "isSecondPageNews": False
        }
        
        # Send to API
        headers = {
            'Authorization': f'Bearer {token}',
            'Content-Type': 'application/json'
        }
        
        response = requests.post(
            f"{API_BASE_URL}/NewsArticle",
            json=article,
            headers=headers,
            timeout=15
        )
        response.raise_for_status()
        
        result = response.json()
        logger.info(f"  ✓ Created article (ID: {result.get('id', 'unknown')})")
        return True
        
    except requests.HTTPError as e:
        logger.error(f"  ❌ HTTP Error: {e.response.status_code}")
        return False
    except Exception as e:
        logger.error(f"  ❌ Failed to create article: {e}")
        return False

def aggregate_news():
    """Main aggregation function - fetch from all sources"""
    logger.info("=" * 60)
    logger.info("🚀 Starting News Aggregation")
    logger.info("=" * 60)
    
    try:
        # Authenticate
        logger.info("🔐 Authenticating...")
        token = get_auth_token()
        logger.info("✓ Authenticated successfully\n")
        
        total_created = 0
        total_fetched = 0
        
        # Fetch from each enabled source
        for source_name, config in NEWS_SOURCES.items():
            if not config.get('enabled', False):
                logger.info(f"⏭️  Skipping disabled source: {source_name}")
                continue
            
            logger.info(f"\n📡 Processing: {source_name}")
            logger.info("-" * 60)
            
            posts = fetch_source(source_name, config)
            total_fetched += len(posts)
            
            if not posts:
                logger.warning(f"No posts from {source_name}")
                continue
            
            # Create articles
            created_count = 0
            for i, post in enumerate(posts, 1):
                logger.info(f"\n[{i}/{len(posts)}] {post['title'][:60]}...")
                if create_article(post, token):
                    created_count += 1
                time.sleep(1)  # Rate limiting
            
            logger.info(f"\n✓ {source_name}: Created {created_count}/{len(posts)} articles")
            total_created += created_count
        
        # Summary
        logger.info("\n" + "=" * 60)
        logger.info(f"✓ Aggregation Complete!")
        logger.info(f"  Total Fetched: {total_fetched}")
        logger.info(f"  Total Created: {total_created}")
        logger.info(f"  Duplicates Skipped: {total_fetched - total_created}")
        logger.info("=" * 60)
        
    except Exception as e:
        logger.error(f"Aggregation failed: {e}")

def run_scheduler():
    """Run the scheduler - Daily at 5:00 AM Turkey Time"""
    logger.info("🤖 News Aggregator Scheduler Started")
    logger.info("⏰ Scheduled to run every day at 5:00 AM (Turkey Time)")
    logger.info("🌍 Current Turkey Time: " + datetime.now(TURKEY_TZ).strftime('%Y-%m-%d %H:%M:%S %Z'))
    logger.info("Press Ctrl+C to stop\n")
    
    # Schedule daily run at 5:00 AM Turkey time
    schedule.every().day.at("05:00").do(aggregate_news)
    
    # Run immediately on start (optional - comment out if you don't want initial run)
    logger.info("🔄 Running initial aggregation...")
    aggregate_news()
    
    # Keep running
    while True:
        # Get current time in Turkey timezone
        now_turkey = datetime.now(TURKEY_TZ)
        
        # Run pending tasks
        schedule.run_pending()
        
        # Sleep for 1 minute
        time.sleep(60)

if __name__ == "__main__":
    import sys
    
    if len(sys.argv) > 1 and sys.argv[1] == "--once":
        # Run once and exit
        aggregate_news()
    elif len(sys.argv) > 1 and sys.argv[1] == "--schedule":
        # Run scheduler (daily at 5 AM Turkey time)
        run_scheduler()
    else:
        # Default: run once
        print("Usage:")
        print("  python news_aggregator.py --once      # Run aggregation once")
        print("  python news_aggregator.py --schedule  # Run daily at 5:00 AM (Turkey Time)")
        print("\nRunning once by default...\n")
        aggregate_news()
