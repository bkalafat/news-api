using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using newsApi.Common;
using newsApi.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace newsApi.Data
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMongoCollection<User> _userList;
        private readonly IMemoryCache _cache;

        public UserService(INewsDatabaseSettings settings, IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _userList = database.GetCollection<User>(settings.UserCollectionName);
            _clientFactory = clientFactory;
            _cache = cache;
        }

        public async void CreateUserAsync(User user)
        {
            //TODO: Cache'te ExpoNotificationRequest varsa return
            //Cache'te yoksa _cache.Remove(CacheKeys.UserList);
            var cacheUserList = await GetUserList();
            if (cacheUserList.Any(cacheUser => cacheUser.ExpoNotificationRequest == user.ExpoNotificationRequest))
                return;
            _cache.Remove(CacheKeys.UserList);
            await _userList.InsertOneAsync(user);
        }

        public async Task<List<User>> GetUserList()
        {
            if (_cache.TryGetValue(CacheKeys.UserList, out List<User> userList)) return userList;
            return await _userList.Find(_ => true).ToListAsync();
        }

        public async void SendNotification(ExpoNotificationRequest expoNotificationRequest)
        {
            var httpClient = _clientFactory.CreateClient();

            string jsonString = JsonSerializer.Serialize(expoNotificationRequest);
            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            using var httpResponse = await httpClient.PostAsync("https://exp.host/--/api/v2/push/send", stringContent);
            httpResponse.EnsureSuccessStatusCode();
        }

    }
}