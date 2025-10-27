@echo off
REM Quick test with limited sources
echo Testing news aggregator with limited sources...
echo.

cd /d "%~dp0"

REM Create temporary test config
python -c "
import sys
sys.path.insert(0, '.')
from news_aggregator import *

# Temporarily disable most sources for quick test
NEWS_SOURCES_TEST = {
    'reddit_technology': NEWS_SOURCES['reddit_technology'].copy(),
    'reddit_localllama': NEWS_SOURCES['reddit_localllama'].copy(),
    'github_trending_daily': NEWS_SOURCES['github_trending_daily'].copy(),
}

# Reduce limits for testing
NEWS_SOURCES_TEST['reddit_technology']['params']['limit'] = 5
NEWS_SOURCES_TEST['reddit_localllama']['params']['limit'] = 5
NEWS_SOURCES_TEST['github_trending_daily']['params']['per_page'] = 3

# Override global NEWS_SOURCES
import news_aggregator
news_aggregator.NEWS_SOURCES = NEWS_SOURCES_TEST

print('\n🧪 TEST MODE - Limited sources and items')
print('Sources: Reddit Technology (5), LocalLLM (5), GitHub Trending (3)')
print('=' * 60)

# Run aggregation
aggregate_news()
"

pause
