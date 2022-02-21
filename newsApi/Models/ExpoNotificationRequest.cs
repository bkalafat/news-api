
namespace newsApi.Models;

public class ExpoNotificationRequest
{
    public string[] to { get; set; }
    public News data { get; set; }
    public string title { get; set; }
    public string body { get; set; }

}
