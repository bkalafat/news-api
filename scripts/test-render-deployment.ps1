# Test Render Deployment
# Run after deployment completes

# Test 1: Health check
Write-Host "Testing health endpoint..." -ForegroundColor Cyan
curl https://newsportal-backend.onrender.com/health -UseBasicParsing | Select-Object StatusCode, Content

# Test 2: Get all news articles
Write-Host "`nTesting news articles endpoint..." -ForegroundColor Cyan
curl https://newsportal-backend.onrender.com/api/NewsArticle -UseBasicParsing | Select-Object StatusCode

# Test 3: Get specific article (if any exist)
Write-Host "`nFetching first article..." -ForegroundColor Cyan
$response = curl https://newsportal-backend.onrender.com/api/NewsArticle -UseBasicParsing
$articles = ($response.Content | ConvertFrom-Json)
if ($articles.Count -gt 0) {
    Write-Host "✅ Found $($articles.Count) articles!" -ForegroundColor Green
    Write-Host "Sample: $($articles[0].Caption)" -ForegroundColor Yellow
} else {
    Write-Host "⚠️ No articles found - check MongoDB connection" -ForegroundColor Yellow
}

Write-Host "`n✅ Deployment test complete!" -ForegroundColor Green
