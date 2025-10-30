# Reddit News Seed - Implementation Guide

## Overview
This implementation converts Reddit posts from technology and GitHub-related subreddits into Turkish news articles for the News Portal.

## Files Created

### 1. Prompt Documentation
**Location:** `.github/prompts/reddit-news-seed.prompt.md`

Comprehensive guide covering:
- Reddit API usage and best practices
- Target subreddits (r/github, r/technology, r/programming, r/webdev, etc.)
- Post selection criteria and priority scoring
- Content transformation workflow
- Turkish translation guidelines
- Image sourcing strategies (Unsplash, MinIO)
- HTML formatting examples
- Quality assurance checklist

### 2. Seed Data Class
**Location:** `backend/Infrastructure/Data/SeedRedditNewsData.cs`

Contains converted Reddit posts as Turkish news articles:
- GitHub Enterprise billing issue
- Developer experience with Copilot (stress discussion)
- Copilot Pro free access uncertainty
- Personal vs work GitHub account dilemma
- GitHub UI bug (activity section missing)

### 3. Controller Endpoint
**Location:** `backend/Presentation/Controllers/SeedController.cs`

New endpoint added:
```csharp
POST /api/Seed/reddit
```

## Usage

### Docker Environment (Recommended)

1. **Start services:**
```powershell
docker-compose up -d
```

2. **Seed Reddit news:**
```powershell
$headers = @{'Content-Type'='application/json'}
Invoke-RestMethod -Uri "http://localhost:5000/api/Seed/reddit" -Method POST -Headers $headers
```

### Swagger UI

1. Navigate to: `http://localhost:5000/swagger`
2. Find `POST /api/Seed/reddit`
3. Click "Try it out" → "Execute"

### Results

The seed will:
- ✅ Clear existing Reddit/GitHub category news
- ✅ Insert 5 high-quality Turkish news articles
- ✅ All articles have proper Turkish slugs (ı→i, ğ→g, etc.)
- ✅ Rich HTML content with images, tables, blockquotes
- ✅ Relevant keywords and social tags
- ✅ Priority scoring based on Reddit engagement

## News Articles Included

### 1. GitHub Enterprise Billing Issue (Priority 1)
**Source:** Reddit r/github - 25 upvotes  
**Topic:** Double billing ($168 instead of $84), no support response for 3+ weeks  
**Category:** `github`  
**Image:** Unsplash financial/billing themed

### 2. Copilot Stress Experience (Priority 1)
**Source:** Reddit r/webdev - 222 upvotes  
**Topic:** Developer finds coding less stressful without Copilot  
**Category:** `reddit`  
**Image:** Unsplash coding/laptop themed

### 3. Copilot Free Access Concerns (Priority 2)
**Source:** Reddit r/github - Low engagement  
**Topic:** Open source contributors worried about losing free Copilot Pro  
**Category:** `github`  
**Image:** Unsplash GitHub themed

### 4. Personal vs Work Accounts (Priority 2)
**Source:** Reddit r/github - 9 upvotes  
**Topic:** Security dilemma with personal GitHub accounts at work  
**Category:** `github`  
**Image:** Unsplash cybersecurity themed

### 5. Missing Activity Section (Priority 3)
**Source:** Reddit r/github - 10 upvotes  
**Topic:** UI bug, activity sidebar disappeared after SSO setup  
**Category:** `github`  
**Image:** Unsplash interface/UI themed

## Reddit API Integration (Future)

For automated scraping in production:

### Required Setup
1. **Reddit App Registration:** https://www.reddit.com/prefs/apps
2. **OAuth2 Token:** Use client credentials flow
3. **Rate Limits:** 60 requests/minute with OAuth

### Example API Call
```bash
curl -H "Authorization: Bearer YOUR_TOKEN" \
     "https://oauth.reddit.com/r/github/top?t=month&limit=25"
```

### Recommended Libraries
- **.NET:** `Reddit.NET` or custom HttpClient
- **Alternative:** PRAW Python wrapper + C# interop

## Translation Strategy

### Current Implementation
Manual Turkish translation with:
- Natural Turkish writing style
- Technical term explanations
- Cultural context additions
- Professional journalism tone

### Future Automation Options
1. **DeepL API:** High-quality translations
2. **Azure Translator:** Microsoft's service
3. **Manual review:** Always recommended for quality

## Image Sources

### Priority Order
1. **Reddit post images:** Use if available and relevant
2. **Unsplash:** Free high-quality stock photos
3. **MinIO:** Custom uploaded images

### Current Unsplash URLs
All articles use relevant Unsplash images:
- Financial: `photo-1551288049-bebda4e38f71`
- Coding: `photo-1555066931-4365d14bab8c`
- GitHub: `photo-1618401471353-b98afee0b2eb`
- Security: `photo-1563986768609-322da13575f3`
- UI/Interface: `photo-1618044733300-9472054094ee`

## Content Quality Standards

### ✅ What We Did Right
- Comprehensive Turkish translation (500-1200 words each)
- Rich HTML formatting (images, tables, blockquotes, lists)
- Proper Turkish slugs using `SlugHelper`
- Context and explanations for international topics
- Multiple perspectives and community reactions
- Technical accuracy maintained

### 📋 Quality Checklist
- [x] Turkish translation is natural and accurate
- [x] All technical terms explained or translated
- [x] Images are high-quality and relevant
- [x] HTML renders correctly
- [x] Turkish-safe slugs (no special chars)
- [x] Keywords in Turkish
- [x] Social tags include both languages
- [x] 500+ word content
- [x] Priority calculated correctly
- [x] Authors field populated

## Monitoring & Maintenance

### Check Seeded Data
```powershell
# View in Mongo Express: http://localhost:8081
# Or via MongoDB shell:
docker exec -it newsportal-mongodb mongosh -u admin -p password123 --eval "use NewsDb; db.News.find({category: 'github'}).pretty()"
```

### Update Strategy
- **Daily:** Fetch top posts from yesterday
- **Weekly:** Review and add top weekly posts
- **Monthly:** Comprehensive coverage of trending topics

## Best Practices

### Do's ✅
- Use Reddit API properly with OAuth
- Respect rate limits (60 req/min)
- Attribute content to original posts
- Add context for Turkish audience
- Use high-quality images
- Maintain consistent formatting
- Review translations for accuracy

### Don'ts ❌
- Don't scrape without authentication
- Don't ignore copyright (give credit)
- Don't use low-quality images
- Don't over-translate technical terms
- Don't skip quality review
- Don't duplicate content
- Don't violate Reddit ToS

## Future Enhancements

### Planned Features
1. **Automated scraping:** Background service for daily Reddit checks
2. **Translation API:** Integrate DeepL or Azure Translator
3. **Image processing:** Automatic thumbnail generation
4. **Sentiment analysis:** Better priority calculation
5. **Admin dashboard:** Manual review and approval workflow
6. **Category detection:** ML-based categorization
7. **Duplicate detection:** Avoid similar posts

### Additional Subreddits
- r/MachineLearning - AI/ML news
- r/devops - DevOps practices
- r/artificial - Artificial Intelligence
- r/coding - General programming
- r/opensource - Open source projects

## Troubleshooting

### Issue: Seed endpoint returns 500 error
**Solution:** Check MongoDB connection and ensure `MongoDbContext` is properly configured

### Issue: Turkish characters not displaying
**Solution:** Verify UTF-8 encoding in MongoDB and API responses

### Issue: Images not loading
**Solution:** Check Unsplash URLs are valid and accessible

### Issue: Slugs have special characters
**Solution:** Verify `SlugHelper.GenerateSlug()` is being used correctly

## References

### Documentation
- Reddit API: https://www.reddit.com/dev/api/
- Unsplash API: https://unsplash.com/developers
- MongoDB C# Driver: https://mongodb.github.io/mongo-csharp-driver/

### Related Files
- Original MongoDB export: `socialmedia_export.json`
- SlugHelper: `backend/Common/SlugHelper.cs`
- News entity: `backend/Domain/Entities/NewsArticle.cs`
- Seed controller: `backend/Presentation/Controllers/SeedController.cs`

## Support

For questions or issues:
1. Check `.github/prompts/reddit-news-seed.prompt.md` for detailed guidance
2. Review existing seed implementations in `SeedNewsData.cs`
3. Test with Swagger UI before automation
4. Monitor logs in Docker: `docker-compose logs -f newsapi`

---

**Created:** October 31, 2025  
**Version:** 1.0  
**Author:** Teknoloji Editörlüğü
