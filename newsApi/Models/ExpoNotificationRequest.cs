﻿namespace newsApi.Models;

public class ExpoNotificationRequest
{
    public string to { get; set; } // all users notification tokens as array
    public Data data { get; set; }
    public string title { get; set; } // TS Kulis
    public string body { get; set; } // news.caption

}
public class Data
{
    public string extradata { get; set; } // news.slug
}