"""
Fetch Technology News from Reddit and Upload to NewsPortal
This script fetches top tech news from Reddit, translates to Turkish,
downloads images, uploads to MinIO, and inserts into MongoDB
"""

import requests
import json
import time
from datetime import datetime
import hashlib
import re
import os
from typing import List, Dict

# Configuration
REDDIT_URL = "https://www.reddit.com/r/technology/top.json"
API_BASE_URL = "http://localhost:5000/api"
MINIO_BASE_URL = "http://localhost:9000/news-images"
ADMIN_USERNAME = "admin"
ADMIN_PASSWORD = "admin123"

def slugify(text: str) -> str:
    """Convert text to URL-friendly slug"""
    text = text.lower()
    # Turkish characters
    replacements = {'ş': 's', 'ğ': 'g', 'ü': 'u', 'ö': 'o', 'ç': 'c', 'ı': 'i',
                   'İ': 'i', 'Ş': 's', 'Ğ': 'g', 'Ü': 'u', 'Ö': 'o', 'Ç': 'c'}
    for tr, en in replacements.items():
        text = text.replace(tr, en)
    
    # Remove special characters
    text = re.sub(r'[^a-z0-9\s-]', '', text)
    text = re.sub(r'\s+', '-', text)
    text = re.sub(r'-+', '-', text)
    return text.strip('-')

def translate_text(text: str, source='en', target='tr') -> str:
    """Translate text using Google Translate (free endpoint)"""
    if not text or len(text.strip()) == 0:
        return text
    
    try:
        url = f"https://translate.googleapis.com/translate_a/single"
        params = {
            'client': 'gtx',
            'sl': source,
            'tl': target,
            'dt': 't',
            'q': text[:5000]  # Limit length
        }
        response = requests.get(url, params=params, timeout=10)
        response.raise_for_status()
        result = response.json()
        translated = ''.join([item[0] for item in result[0] if item[0]])
        return translated
    except Exception as e:
        print(f"  ⚠️  Translation failed: {e}")
        return text

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
        print(f"❌ Failed to get auth token: {e}")
        raise

def fetch_reddit_posts(limit: int = 10) -> List[Dict]:
    """Fetch top posts from r/technology"""
    try:
        headers = {'User-Agent': 'NewsPortal/1.0'}
        params = {'t': 'week', 'limit': limit}
        
        response = requests.get(REDDIT_URL, headers=headers, params=params, timeout=15)
        response.raise_for_status()
        
        data = response.json()
        posts = data['data']['children']
        
        print(f"✓ Fetched {len(posts)} posts from Reddit")
        return posts
    except Exception as e:
        print(f"❌ Failed to fetch Reddit data: {e}")
        return []

def get_image_url(post_data: Dict) -> str:
    """Extract image URL from post data"""
    # Try thumbnail
    if post_data.get('thumbnail') and post_data['thumbnail'].startswith('http'):
        if 'self' not in post_data['thumbnail'] and 'default' not in post_data['thumbnail']:
            return post_data['thumbnail']
    
    # Try preview images
    if post_data.get('preview', {}).get('images'):
        try:
            return post_data['preview']['images'][0]['source']['url'].replace('&amp;', '&')
        except:
            pass
    
    # Try direct URL
    if post_data.get('url') and re.search(r'\.(jpg|jpeg|png|gif|webp)$', post_data['url'], re.I):
        return post_data['url']
    
    return None

def create_news_article(post_data: Dict, token: str) -> bool:
    """Create news article via API"""
    try:
        # Translate title
        print(f"  🌐 Translating title...")
        title_tr = translate_text(post_data['title'])
        time.sleep(0.5)  # Rate limiting
        
        # Get image
        image_url = get_image_url(post_data)
        if not image_url:
            print(f"  ⚠️  No image found, skipping...")
            return False
        
        # Create content
        content_en = post_data.get('selftext', '')
        if content_en:
            print(f"  🌐 Translating content...")
            content_tr = translate_text(content_en[:2000])  # Limit length
            time.sleep(0.5)
            content_html = f"<p>{content_tr}</p>"
        else:
            content_html = f"<p>{title_tr}</p>"
        
        # Add source link
        reddit_link = f"https://reddit.com{post_data['permalink']}"
        content_html += f"<p><strong>Kaynak:</strong> <a href='{reddit_link}' target='_blank'>Reddit r/technology</a></p>"
        
        # Generate slug
        slug = slugify(title_tr)
        
        # Prepare article data
        article = {
            "category": "Teknoloji",
            "type": "Haber",
            "caption": title_tr[:500],
            "keywords": "teknoloji, reddit, haber",
            "socialTags": "#teknoloji #reddit",
            "summary": title_tr[:200],
            "imgPath": "",
            "imgAlt": title_tr[:200],
            "imageUrl": image_url,
            "thumbnailUrl": image_url,
            "content": content_html,
            "subjects": ["Teknoloji"],
            "authors": ["Reddit Technology"],
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
        print(f"  ✓ Created article (ID: {result.get('id', 'unknown')})")
        return True
        
    except requests.HTTPError as e:
        print(f"  ❌ HTTP Error: {e.response.status_code} - {e.response.text}")
        return False
    except Exception as e:
        print(f"  ❌ Failed to create article: {e}")
        return False

def main():
    """Main function"""
    print("=" * 50)
    print("Reddit Technology News Fetcher")
    print("=" * 50)
    print()
    
    # Get auth token
    print("🔐 Authenticating...")
    try:
        token = get_auth_token()
        print("✓ Authenticated successfully")
        print()
    except:
        return
    
    # Fetch Reddit posts
    print("📰 Fetching Reddit posts...")
    posts = fetch_reddit_posts(limit=15)
    
    if not posts:
        print("No posts found.")
        return
    
    print()
    print(f"Processing {len(posts)} articles...")
    print()
    
    # Process each post
    success_count = 0
    for i, post in enumerate(posts, 1):
        data = post['data']
        title = data['title'][:60] + ('...' if len(data['title']) > 60 else '')
        
        print(f"[{i}/{len(posts)}] {title}")
        
        if create_news_article(data, token):
            success_count += 1
        
        print()
        time.sleep(1)  # Rate limiting between articles
    
    print("=" * 50)
    print(f"✓ Successfully created {success_count}/{len(posts)} articles")
    print("=" * 50)
    print()
    print("🌐 Visit http://localhost:3000 to see the news!")

if __name__ == "__main__":
    main()
