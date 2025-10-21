using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;
using NewsApi.Tests.Helpers;
using NewsApi.Tests.Integration.Fixtures;

namespace NewsApi.Tests.Integration.Controllers;

public class NewsControllerIntegrationTests : IClassFixture<NewsApiWebApplicationFactory>
{
 private readonly HttpClient _client;
    private readonly NewsApiWebApplicationFactory _factory;

    public NewsControllerIntegrationTests(NewsApiWebApplicationFactory factory)
  {
  _factory = factory;
 _client = factory.CreateClient();
    }

    #region GET /api/news Tests

    [Fact]
public async Task GetAllNews_WhenNoNewsExists_ShouldReturnEmptyList()
    {
  // Act
    var response = await _client.GetAsync("/api/news");

    // Assert
     response.StatusCode.Should().Be(HttpStatusCode.OK);
     var news = await response.Content.ReadFromJsonAsync<List<News>>();
   news.Should().NotBeNull();
        news.Should().BeEmpty();
    }

  [Fact]
public async Task GetAllNews_AfterCreatingNews_ShouldReturnNews()
{
        // Arrange
    var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
   var createResponse = await CreateNewsWithAuth(createDto);
        createResponse.Should().Be(HttpStatusCode.Created);

   // Act
      var response = await _client.GetAsync("/api/news");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var news = await response.Content.ReadFromJsonAsync<List<News>>();
   news.Should().NotBeNull();
   news.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetAllNews_WithCategoryFilter_ShouldReturnFilteredNews()
    {
      // Arrange
  await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithCategory("Technology").Build());
 await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithCategory("Sports").Build());
        await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithCategory("Technology").Build());

  // Act
     var response = await _client.GetAsync("/api/news?category=Technology");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
     var news = await response.Content.ReadFromJsonAsync<List<News>>();
  news.Should().HaveCount(2);
   news.Should().OnlyContain(n => n.Category == "Technology");
    }

    [Fact]
    public async Task GetAllNews_WithTypeFilter_ShouldReturnFilteredNews()
    {
    // Arrange
     await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithType("Breaking").Build());
  await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithType("Article").Build());
     await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithType("Breaking").Build());

   // Act
   var response = await _client.GetAsync("/api/news?type=Breaking");

     // Assert
response.StatusCode.Should().Be(HttpStatusCode.OK);
     var news = await response.Content.ReadFromJsonAsync<List<News>>();
news.Should().HaveCount(2);
      news.Should().OnlyContain(n => n.Type == "Breaking");
    }

    #endregion

    #region GET /api/news/{id} Tests

 [Fact]
    public async Task GetNewsById_WithValidId_ShouldReturnNews()
    {
        // Arrange
 var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
   var createResponse = await _client.PostAsJsonAsync("/api/news", createDto);
   var createdNews = await createResponse.Content.ReadFromJsonAsync<News>();

   // Act
 var response = await _client.GetAsync($"/api/news/{createdNews!.Id}");

   // Assert
   response.StatusCode.Should().Be(HttpStatusCode.OK);
    var news = await response.Content.ReadFromJsonAsync<News>();
   news.Should().NotBeNull();
  news!.Id.Should().Be(createdNews.Id);
    }

    [Fact]
    public async Task GetNewsById_WithInvalidId_ShouldReturn404()
    {
 // Arrange
        var invalidId = Guid.NewGuid().ToString();

   // Act
     var response = await _client.GetAsync($"/api/news/{invalidId}");

        // Assert
     response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

  #endregion

    #region GET /api/news/by-url Tests

 [Fact]
public async Task GetNewsByUrl_WithValidUrl_ShouldReturnNews()
{
// Arrange
  var url = $"test-article-{Guid.NewGuid()}";
      var createDto = CreateNewsDtoBuilder.Create().WithUrl(url).Build();
     await CreateNewsWithAuth(createDto);

   // Act
        var response = await _client.GetAsync($"/api/news/by-url?url={url}");

     // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
   var news = await response.Content.ReadFromJsonAsync<News>();
  news.Should().NotBeNull();
        news!.Url.Should().Be(url);
    }

    [Fact]
    public async Task GetNewsByUrl_WithInvalidUrl_ShouldReturn404()
    {
     // Arrange
 var invalidUrl = "non-existent-url";

   // Act
      var response = await _client.GetAsync($"/api/news/by-url?url={invalidUrl}");

   // Assert
response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetNewsByUrl_WithoutUrl_ShouldReturn400()
    {
      // Act
 var response = await _client.GetAsync("/api/news/by-url");

// Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

    #endregion

    #region POST /api/news Tests

    [Fact]
    public async Task CreateNews_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();

   // Act
      var response = await CreateNewsWithAuth(createDto);

// Assert
        response.Should().Be(HttpStatusCode.Created);
 }

  [Fact]
    public async Task CreateNews_WithInvalidData_ShouldReturn400()
    {
 // Arrange
        var createDto = CreateNewsDtoBuilder.Create().AsInvalidDto().Build();

   // Act
        var response = await CreateNewsWithAuth(createDto);

     // Assert
  response.Should().Be(HttpStatusCode.BadRequest);
    }

  [Fact]
    public async Task CreateNews_WithoutAuthentication_ShouldReturn401()
    {
     // Arrange
        var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();

  // Act
var response = await _client.PostAsJsonAsync("/api/news", createDto);

   // Assert
     response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region PUT /api/news/{id} Tests

    [Fact]
    public async Task UpdateNews_WithValidData_ShouldReturnNoContent()
    {
      // Arrange
        var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
var createResponse = await _client.PostAsJsonAsync("/api/news", createDto);
        var createdNews = await createResponse.Content.ReadFromJsonAsync<News>();

   var updateDto = UpdateNewsDtoBuilder.Create()
   .WithCaption("Updated Caption")
    .Build();

   // Act
        var response = await UpdateNewsWithAuth(createdNews!.Id, updateDto);

  // Assert
response.Should().Be(HttpStatusCode.NoContent);
  }

    [Fact]
    public async Task UpdateNews_WithInvalidId_ShouldReturn404()
    {
     // Arrange
   var invalidId = Guid.NewGuid().ToString();
     var updateDto = UpdateNewsDtoBuilder.Create().Build();

   // Act
     var response = await UpdateNewsWithAuth(invalidId, updateDto);

        // Assert
   response.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
 public async Task UpdateNews_WithoutAuthentication_ShouldReturn401()
    {
    // Arrange
   var updateDto = UpdateNewsDtoBuilder.Create().Build();

   // Act
var response = await _client.PutAsJsonAsync($"/api/news/{Guid.NewGuid()}", updateDto);

        // Assert
 response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region DELETE /api/news/{id} Tests

    [Fact]
    public async Task DeleteNews_WithValidId_ShouldReturnNoContent()
    {
     // Arrange
     var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
   var createResponse = await _client.PostAsJsonAsync("/api/news", createDto);
        var createdNews = await createResponse.Content.ReadFromJsonAsync<News>();

        // Act
   var response = await DeleteNewsWithAuth(createdNews!.Id);

 // Assert
        response.Should().Be(HttpStatusCode.NoContent);
  }

    [Fact]
    public async Task DeleteNews_WithInvalidId_ShouldReturn404()
    {
// Arrange
     var invalidId = Guid.NewGuid().ToString();

  // Act
     var response = await DeleteNewsWithAuth(invalidId);

     // Assert
      response.Should().Be(HttpStatusCode.NotFound);
    }

[Fact]
    public async Task DeleteNews_WithoutAuthentication_ShouldReturn401()
    {
    // Act
  var response = await _client.DeleteAsync($"/api/news/{Guid.NewGuid()}");

     // Assert
  response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

#region Complete CRUD Workflow Tests

    [Fact]
    public async Task CompleteCRUDWorkflow_ShouldWork()
   {
    // Create
  var createDto = CreateNewsDtoBuilder.Create()
    .WithCaption("Original Caption")
   .AsValidTechnologyNews()
   .Build();

var createResponse = await _client.PostAsJsonAsync("/api/news", createDto);
    createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
var createdNews = await createResponse.Content.ReadFromJsonAsync<News>();
    createdNews.Should().NotBeNull();

        // Read
   var getResponse = await _client.GetAsync($"/api/news/{createdNews!.Id}");
     getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
   var retrievedNews = await getResponse.Content.ReadFromJsonAsync<News>();
   retrievedNews!.Caption.Should().Be("Original Caption");

        // Update
        var updateDto = UpdateNewsDtoBuilder.Create()
.WithCaption("Updated Caption")
   .Build();
        var updateResponse = await _client.PutAsJsonAsync($"/api/news/{createdNews.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

     // Verify Update
  var getUpdatedResponse = await _client.GetAsync($"/api/news/{createdNews.Id}");
        var updatedNews = await getUpdatedResponse.Content.ReadFromJsonAsync<News>();
        updatedNews!.Caption.Should().Be("Updated Caption");

 // Delete
   var deleteResponse = await _client.DeleteAsync($"/api/news/{createdNews.Id}");
deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

  // Verify Delete
 var getDeletedResponse = await _client.GetAsync($"/api/news/{createdNews.Id}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helper Methods

    private async Task<HttpStatusCode> CreateNewsWithAuth(CreateNewsDto dto)
    {
   var response = await _client.PostAsJsonAsync("/api/news", dto);
   return response.StatusCode;
    }

 private async Task<HttpStatusCode> UpdateNewsWithAuth(string id, UpdateNewsDto dto)
    {
   var response = await _client.PutAsJsonAsync($"/api/news/{id}", dto);
   return response.StatusCode;
    }

   private async Task<HttpStatusCode> DeleteNewsWithAuth(string id)
   {
     var response = await _client.DeleteAsync($"/api/news/{id}");
    return response.StatusCode;
    }

    #endregion
}
