using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;
using NewsApi.Tests.Integration;

namespace NewsApi.Tests.E2E;

/// <summary>
/// End-to-End tests that verify complete user workflows from HTTP request to database and back.
/// These tests ensure the entire application stack works together correctly.
/// </summary>
public class NewsApiE2ETests : IClassFixture<NewsApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public NewsApiE2ETests(NewsApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task CompleteNewsLifecycle_CreateReadUpdateDelete_ShouldWorkEndToEnd()
    {
        // Step 1: Create a news article
        var createDto = new CreateNewsDto
        {
            Category = "Technology",
            Type = "Breaking News",
            Caption = "Revolutionary AI Technology Announced",
            Keywords = "AI, technology, innovation, breakthrough",
            SocialTags = "#AI #technology #innovation #breakthrough",
            Summary = "A groundbreaking AI technology has been announced that could revolutionize the industry.",
            ImgPath = "/images/ai-breakthrough.jpg",
            ImgAlt = "AI technology illustration",
            Content = "Today, a major technology company announced a revolutionary AI system that promises to change how we interact with technology. The new system demonstrates unprecedented capabilities in natural language processing and reasoning.",
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
            Url = "revolutionary-ai-technology-announced",
            Subjects = new[] { "Artificial Intelligence", "Technology", "Innovation" },
            Authors = new[] { "Tech Reporter", "AI Specialist" }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/news", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdNewsJson = await createResponse.Content.ReadAsStringAsync();
        var createdNews = JsonSerializer.Deserialize<News>(createdNewsJson, _jsonOptions);
        createdNews.Should().NotBeNull();
        var newsId = createdNews!.Id;

        // Step 2: Verify the news appears in the list
        var listResponse = await _client.GetAsync("/api/news");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listJson = await listResponse.Content.ReadAsStringAsync();
        var newsList = JsonSerializer.Deserialize<List<News>>(listJson, _jsonOptions);
        newsList.Should().Contain(n => n.Id == newsId);

        // Step 3: Read the specific news article
        var getResponse = await _client.GetAsync($"/api/news/{newsId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedNewsJson = await getResponse.Content.ReadAsStringAsync();
        var retrievedNews = JsonSerializer.Deserialize<News>(retrievedNewsJson, _jsonOptions);
        retrievedNews.Should().NotBeNull();
        retrievedNews!.Caption.Should().Be(createDto.Caption);
        retrievedNews.Content.Should().Be(createDto.Content);

        // Step 4: Verify URL-based retrieval works
        var urlResponse = await _client.GetAsync($"/api/news/url/{createDto.Url}");
        urlResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var urlNewsJson = await urlResponse.Content.ReadAsStringAsync();
        var urlNews = JsonSerializer.Deserialize<News>(urlNewsJson, _jsonOptions);
        urlNews!.Id.Should().Be(newsId);

        // Step 5: Update the news article
        var updateDto = new UpdateNewsDto
        {
            Category = "Technology",
            Type = "Breaking News",
            Caption = "Updated: Revolutionary AI Technology Gets Global Recognition",
            Keywords = "AI, technology, innovation, breakthrough, recognition",
            SocialTags = "#AI #technology #innovation #breakthrough #recognition",
            Summary = "The groundbreaking AI technology announced earlier has now received global recognition from industry leaders.",
            ImgPath = "/images/ai-breakthrough-updated.jpg",
            ImgAlt = "Updated AI technology illustration",
            Content = "Following the initial announcement, the revolutionary AI system has garnered widespread attention and recognition from technology leaders worldwide. Industry experts are calling it a paradigm shift in artificial intelligence.",
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
            Url = "revolutionary-ai-technology-gets-recognition",
            Subjects = new[] { "Artificial Intelligence", "Technology", "Innovation", "Recognition" },
            Authors = new[] { "Tech Reporter", "AI Specialist", "Industry Analyst" }
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/news/{newsId}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Step 6: Verify the update was applied
        var updatedGetResponse = await _client.GetAsync($"/api/news/{newsId}");
        var updatedNewsJson = await updatedGetResponse.Content.ReadAsStringAsync();
        var updatedNews = JsonSerializer.Deserialize<News>(updatedNewsJson, _jsonOptions);
        
        updatedNews!.Caption.Should().Be(updateDto.Caption);
        updatedNews.Content.Should().Be(updateDto.Content);
        updatedNews.Authors.Should().BeEquivalentTo(updateDto.Authors);

        // Step 7: Test filtering functionality
        var categoryFilterResponse = await _client.GetAsync("/api/news?category=Technology");
        categoryFilterResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var categoryFilterJson = await categoryFilterResponse.Content.ReadAsStringAsync();
        var categoryFilteredNews = JsonSerializer.Deserialize<List<News>>(categoryFilterJson, _jsonOptions);
        categoryFilteredNews.Should().Contain(n => n.Id == newsId);

        // Step 8: Delete the news article
        var deleteResponse = await _client.DeleteAsync($"/api/news/{newsId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Step 9: Verify the news is no longer accessible
        var deletedGetResponse = await _client.GetAsync($"/api/news/{newsId}");
        deletedGetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Step 10: Verify it no longer appears in the list
        var finalListResponse = await _client.GetAsync("/api/news");
        var finalListJson = await finalListResponse.Content.ReadAsStringAsync();
        var finalNewsList = JsonSerializer.Deserialize<List<News>>(finalListJson, _jsonOptions);
        finalNewsList.Should().NotContain(n => n.Id == newsId);
    }

    [Fact]
    public async Task NewsFiltering_WithMultipleArticles_ShouldWorkCorrectly()
    {
        // Create multiple news articles with different categories and types
        var newsArticles = new[]
        {
            new CreateNewsDto
            {
                Category = "Technology",
                Type = "Article",
                Caption = "Tech Article 1",
                Summary = "Technology news summary",
                Content = "Technology content",
                ExpressDate = DateTime.UtcNow,
                Priority = 1,
                Url = "tech-article-1"
            },
            new CreateNewsDto
            {
                Category = "Sports",
                Type = "Breaking",
                Caption = "Sports Breaking News",
                Summary = "Sports news summary",
                Content = "Sports content",
                ExpressDate = DateTime.UtcNow,
                Priority = 2,
                Url = "sports-breaking-news"
            },
            new CreateNewsDto
            {
                Category = "Technology",
                Type = "Feature",
                Caption = "Tech Feature Article",
                Summary = "Technology feature summary",
                Content = "Technology feature content",
                ExpressDate = DateTime.UtcNow,
                Priority = 1,
                Url = "tech-feature-article"
            }
        };

        // Create all articles
        var createdIds = new List<Guid>();
        foreach (var article in newsArticles)
        {
            var response = await _client.PostAsJsonAsync("/api/news", article);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var json = await response.Content.ReadAsStringAsync();
            var news = JsonSerializer.Deserialize<News>(json, _jsonOptions);
            createdIds.Add(news!.Id);
        }

        try
        {
            // Test category filtering
            var techResponse = await _client.GetAsync("/api/news?category=Technology");
            var techJson = await techResponse.Content.ReadAsStringAsync();
            var techNews = JsonSerializer.Deserialize<List<News>>(techJson, _jsonOptions);
            techNews!.Count.Should().Be(2);
            techNews.All(n => n.Category == "Technology").Should().BeTrue();

            // Test type filtering
            var articleResponse = await _client.GetAsync("/api/news?type=Article");
            var articleJson = await articleResponse.Content.ReadAsStringAsync();
            var articleNews = JsonSerializer.Deserialize<List<News>>(articleJson, _jsonOptions);
            articleNews!.Count.Should().Be(1);
            articleNews.All(n => n.Type == "Article").Should().BeTrue();

            // Test combined filtering (should work with AND logic)
            var combinedResponse = await _client.GetAsync("/api/news?category=Technology&type=Article");
            var combinedJson = await combinedResponse.Content.ReadAsStringAsync();
            var combinedNews = JsonSerializer.Deserialize<List<News>>(combinedJson, _jsonOptions);
            combinedNews!.Count.Should().Be(1);
            combinedNews[0].Category.Should().Be("Technology");
            combinedNews[0].Type.Should().Be("Article");
        }
        finally
        {
            // Cleanup: Delete all created articles
            foreach (var id in createdIds)
            {
                await _client.DeleteAsync($"/api/news/{id}");
            }
        }
    }

    [Fact]
    public async Task ValidationErrors_ShouldBeHandledGracefully()
    {
        // Test with completely invalid data
        var invalidDto = new CreateNewsDto
        {
            Category = "", // Required
            Type = "", // Required
            Caption = "", // Required
            Summary = "", // Required
            Content = "", // Required
            ExpressDate = DateTime.MinValue, // Invalid date
            Priority = 0 // Out of range (should be 1-100)
        };

        var response = await _client.PostAsJsonAsync("/api/news", invalidDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Test with data that's too long
        var tooLongDto = new CreateNewsDto
        {
            Category = new string('a', 101), // Max 100 characters
            Type = new string('b', 51), // Max 50 characters
            Caption = new string('c', 501), // Max 500 characters
            Keywords = new string('d', 1001), // Max 1000 characters
            SocialTags = new string('e', 501), // Max 500 characters
            Summary = new string('f', 2001), // Max 2000 characters
            ImgPath = new string('g', 501), // Max 500 characters
            ImgAlt = new string('h', 201), // Max 200 characters
            Content = "Valid content",
            ExpressDate = DateTime.UtcNow,
            Priority = 101, // Out of range (should be 1-100)
            Url = new string('i', 501) // Max 500 characters
        };

        var tooLongResponse = await _client.PostAsJsonAsync("/api/news", tooLongDto);
        tooLongResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CachingBehavior_ShouldImprovePerformance()
    {
        // Create a test article
        var createDto = new CreateNewsDto
        {
            Category = "Performance",
            Type = "Test",
            Caption = "Caching Test Article",
            Summary = "Test caching behavior",
            Content = "This article tests caching functionality",
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
            Url = "caching-test-article"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/news", createDto);
        var createdNewsJson = await createResponse.Content.ReadAsStringAsync();
        var createdNews = JsonSerializer.Deserialize<News>(createdNewsJson, _jsonOptions);
        var newsId = createdNews!.Id;

        try
        {
            // First request - populates cache
            var firstRequest = DateTime.UtcNow;
            var firstResponse = await _client.GetAsync($"/api/news/{newsId}");
            firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var firstRequestTime = DateTime.UtcNow - firstRequest;

            // Second request - should be served from cache (faster)
            var secondRequest = DateTime.UtcNow;
            var secondResponse = await _client.GetAsync($"/api/news/{newsId}");
            secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var secondRequestTime = DateTime.UtcNow - secondRequest;

            // Both requests should return the same data
            var firstNewsJson = await firstResponse.Content.ReadAsStringAsync();
            var secondNewsJson = await secondResponse.Content.ReadAsStringAsync();
            firstNewsJson.Should().Be(secondNewsJson);

            // Verify cache invalidation on update
            var updateDto = new UpdateNewsDto
            {
                Caption = "Updated Caching Test Article"
            };

            await _client.PutAsJsonAsync($"/api/news/{newsId}", updateDto);

            // Fetch after update - should get updated data
            var afterUpdateResponse = await _client.GetAsync($"/api/news/{newsId}");
            var afterUpdateJson = await afterUpdateResponse.Content.ReadAsStringAsync();
            var afterUpdateNews = JsonSerializer.Deserialize<News>(afterUpdateJson, _jsonOptions);
            
            afterUpdateNews!.Caption.Should().Be(updateDto.Caption);
        }
        finally
        {
            // Cleanup
            await _client.DeleteAsync($"/api/news/{newsId}");
        }
    }

    [Fact]
    public async Task ConcurrentRequests_ShouldBeHandledCorrectly()
    {
        // Create multiple news articles concurrently
        var createTasks = Enumerable.Range(1, 5).Select(async i =>
        {
            var dto = new CreateNewsDto
            {
                Category = "Concurrency",
                Type = "Test",
                Caption = $"Concurrent Test Article {i}",
                Summary = $"Test article {i} for concurrency testing",
                Content = $"Content for test article {i}",
                ExpressDate = DateTime.UtcNow,
                Priority = 1,
                Url = $"concurrent-test-article-{i}"
            };

            var response = await _client.PostAsJsonAsync("/api/news", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var json = await response.Content.ReadAsStringAsync();
            var news = JsonSerializer.Deserialize<News>(json, _jsonOptions);
            return news!.Id;
        });

        var createdIds = await Task.WhenAll(createTasks);
        createdIds.Should().HaveCount(5);
        createdIds.Should().OnlyHaveUniqueItems();

        try
        {
            // Read all articles concurrently
            var readTasks = createdIds.Select(async id =>
            {
                var response = await _client.GetAsync($"/api/news/{id}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                
                var json = await response.Content.ReadAsStringAsync();
                var news = JsonSerializer.Deserialize<News>(json, _jsonOptions);
                return news!;
            });

            var readNews = await Task.WhenAll(readTasks);
            readNews.Should().HaveCount(5);
            readNews.Select(n => n.Id).Should().BeEquivalentTo(createdIds);
        }
        finally
        {
            // Cleanup all articles concurrently
            var deleteTasks = createdIds.Select(id => _client.DeleteAsync($"/api/news/{id}"));
            await Task.WhenAll(deleteTasks);
        }
    }
}