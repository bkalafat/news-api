using newsApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace newsApi.Data;

public interface IUserService
{
    public void SendNotification(ExpoNotificationRequest expoNotificationRequest);

    public void CreateUserAsync(User user);

    public Task<List<User>> GetUserList();
}