using System;
using MongoDB.Driver;
using newsApi.Common;
using newsApi.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace newsApi.Data
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMongoCollection<User> _userList;

        public UserService(INewsDatabaseSettings settings, IHttpClientFactory clientFactory)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _userList = database.GetCollection<User>(settings.UserCollectionName);
            _clientFactory = clientFactory;
        }

        public async void CreateUserAsync(string expoNotificationToken)
        {
            try
            {
                await _userList.InsertOneAsync(new User { ExpoNotificationRequest = expoNotificationToken });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async void SendNotification(ExpoNotificationRequest expoNotificationRequest)
        {
            //TODO get token list and send notifications to all devices from mongoDB.
            // ExpoNotificationRequest.to = All tokens as array
            var httpClient = _clientFactory.CreateClient();

            string jsonString = JsonSerializer.Serialize(expoNotificationRequest);
            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            using var httpResponse = await httpClient.PostAsync("https://exp.host/--/api/v2/push/send", stringContent);
            httpResponse.EnsureSuccessStatusCode();
        }

    }
}