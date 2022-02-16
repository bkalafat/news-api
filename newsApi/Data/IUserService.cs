using newsApi.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace newsApi.Data;

public interface IUserService
{
    public void SendNotification(ExpoNotificationRequest expoNotificationRequest);

    public void CreateUserAsync(string expoNotificationToken);
}