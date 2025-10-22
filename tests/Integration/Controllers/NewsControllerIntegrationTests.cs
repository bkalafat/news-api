using System.Net;
using FluentAssertions;
using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;
using NewsApi.Infrastructure.Security;
using NewsApi.Tests.Helpers;
using NewsApi.Tests.Integration.Fixtures;

namespace NewsApi.Tests.Integration.Controllers
{
    public class NewsControllerIntegrationTests : IClassFixture<NewsApiWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly NewsApiWebApplicationFactory _factory;
        private readonly string _authToken;

        public NewsControllerIntegrationTests(NewsApiWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var jwtService = scope.ServiceProvider.GetRequiredService<JwtTokenService>();
            _authToken = jwtService.GenerateToken("test-user", "test-user");
        }

        #region GET /api/news Tests

        [Fact]
        public async Task GetAllNews_WhenNoNewsExists_ShouldReturnEmptyList()
        {
            var response = await _client.GetAsync("/api/news");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = await response.Content.ReadFromJsonAsync<List<News>>();
            news.Should().NotBeNull();
            news.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllNews_AfterCreatingNews_ShouldReturnNews()
        {
            var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
            var createResponse = await CreateNewsWithAuth(createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var response = await _client.GetAsync("/api/news");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = await response.Content.ReadFromJsonAsync<List<News>>();
            news.Should().NotBeNull();
            news.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetAllNews_WithCategoryFilter_ShouldReturnFilteredNews()
        {
            await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithCategory("Technology").Build());
            await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithCategory("Sports").Build());
            await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithCategory("Technology").Build());

            var response = await _client.GetAsync("/api/news?category=Technology");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = await response.Content.ReadFromJsonAsync<List<News>>();
            news.Should().HaveCount(2);
            news.Should().OnlyContain(n => n.Category == "Technology");
        }

        [Fact]
        public async Task GetAllNews_WithTypeFilter_ShouldReturnFilteredNews()
        {
            await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithType("Breaking").Build());
            await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithType("Article").Build());
            await CreateNewsWithAuth(CreateNewsDtoBuilder.Create().WithType("Breaking").Build());

            var response = await _client.GetAsync("/api/news?type=Breaking");
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
            var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
            var createResponse = await CreateNewsWithAuth(createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdNews = await createResponse.Content.ReadFromJsonAsync<News>();

            var response = await _client.GetAsync($"/api/news/{createdNews!.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = await response.Content.ReadFromJsonAsync<News>();
            news.Should().NotBeNull();
            news!.Id.Should().Be(createdNews.Id);
        }

        [Fact]
        public async Task GetNewsById_WithInvalidId_ShouldReturn404()
        {
            var invalidId = Guid.NewGuid().ToString();
            var response = await _client.GetAsync($"/api/news/{invalidId}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region GET /api/news/by-url Tests

        [Fact]
        public async Task GetNewsByUrl_WithValidUrl_ShouldReturnNews()
        {
            var url = $"test-article-{Guid.NewGuid()}";
            var createDto = CreateNewsDtoBuilder.Create().WithUrl(url).Build();
            var createResponse = await CreateNewsWithAuth(createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var response = await _client.GetAsync($"/api/news/by-url?url={url}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var news = await response.Content.ReadFromJsonAsync<News>();
            news.Should().NotBeNull();
            news!.Url.Should().Be(url);
        }

        [Fact]
        public async Task GetNewsByUrl_WithoutUrl_ShouldReturn400()
        {
            var response = await _client.GetAsync("/api/news/by-url");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetNewsByUrl_WithInvalidUrl_ShouldReturn404()
        {
            var invalidUrl = $"nonexistent-{Guid.NewGuid()}";
            var response = await _client.GetAsync($"/api/news/by-url?url={invalidUrl}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region POST /api/news Tests

        [Fact]
        public async Task CreateNews_WithValidData_ShouldReturnCreated()
        {
            var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
            var response = await CreateNewsWithAuth(createDto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdNews = await response.Content.ReadFromJsonAsync<News>();
            createdNews.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateNews_WithInvalidData_ShouldReturn400()
        {
            var createDto = CreateNewsDtoBuilder.Create().AsInvalidDto().Build();
            var response = await CreateNewsWithAuth(createDto);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateNews_WithoutAuthentication_ShouldReturn401()
        {
            var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
            var response = await _client.PostAsJsonAsync("/api/news", createDto);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region PUT /api/news/{id} Tests

        [Fact]
        public async Task UpdateNews_WithValidData_ShouldReturnNoContent()
        {
            var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
            var createResponse = await CreateNewsWithAuth(createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdNews = await createResponse.Content.ReadFromJsonAsync<News>();

            var updateDto = UpdateNewsDtoBuilder.Create().WithCaption("Updated Caption").Build();
            var response = await UpdateNewsWithAuth(createdNews!.Id, updateDto);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateNews_WithInvalidId_ShouldReturn404()
        {
            var invalidId = Guid.NewGuid().ToString();
            var updateDto = UpdateNewsDtoBuilder.Create().Build();
            var response = await UpdateNewsWithAuth(invalidId, updateDto);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateNews_WithoutAuthentication_ShouldReturn401()
        {
            var updateDto = UpdateNewsDtoBuilder.Create().Build();
            var response = await _client.PutAsJsonAsync($"/api/news/{Guid.NewGuid()}", updateDto);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region DELETE /api/news/{id} Tests

        [Fact]
        public async Task DeleteNews_WithValidId_ShouldReturnNoContent()
        {
            var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
            var createResponse = await CreateNewsWithAuth(createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdNews = await createResponse.Content.ReadFromJsonAsync<News>();

            var response = await DeleteNewsWithAuth(createdNews!.Id);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteNews_WithInvalidId_ShouldReturn404()
        {
            var invalidId = Guid.NewGuid().ToString();
            var response = await DeleteNewsWithAuth(invalidId);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteNews_WithoutAuthentication_ShouldReturn401()
        {
            var response = await _client.DeleteAsync($"/api/news/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region Complete CRUD Workflow Tests

        [Fact]
        public async Task CompleteCRUDWorkflow_ShouldWork()
        {
            var createDto = CreateNewsDtoBuilder.Create()
                .AsValidTechnologyNews()
                .WithCaption("Original Caption")
                .Build();

            var createResponse = await CreateNewsWithAuth(createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdNews = await createResponse.Content.ReadFromJsonAsync<News>();
            createdNews.Should().NotBeNull();

            var getResponse = await _client.GetAsync($"/api/news/{createdNews!.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedNews = await getResponse.Content.ReadFromJsonAsync<News>();
            retrievedNews!.Caption.Should().Be("Original Caption");

            var updateDto = UpdateNewsDtoBuilder.Create()
                .WithCaption("Updated Caption")
                .Build();

            var updateResponse = await UpdateNewsWithAuth(createdNews.Id, updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getUpdatedResponse = await _client.GetAsync($"/api/news/{createdNews.Id}");
            var updatedNews = await getUpdatedResponse.Content.ReadFromJsonAsync<News>();
            updatedNews!.Caption.Should().Be("Updated Caption");

            var deleteResponse = await DeleteNewsWithAuth(createdNews.Id);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getDeletedResponse = await _client.GetAsync($"/api/news/{createdNews.Id}");
            getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region Helper Methods

        private async Task<HttpResponseMessage> CreateNewsWithAuth(CreateNewsDto dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/news")
            {
                Content = JsonContent.Create(dto)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
            return await _client.SendAsync(request);
        }

        private async Task<HttpResponseMessage> UpdateNewsWithAuth(string id, UpdateNewsDto dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/news/{id}")
            {
                Content = JsonContent.Create(dto)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
            return await _client.SendAsync(request);
        }

        private async Task<HttpResponseMessage> DeleteNewsWithAuth(string id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/news/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
            return await _client.SendAsync(request);
        }

        #endregion
    }
}
