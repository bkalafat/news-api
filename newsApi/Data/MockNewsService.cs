using newsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace newsApi.Data
{
    public class MockNewsService : INewsService
    {
        private readonly IUserService _userService;

        public MockNewsService(IUserService userService)
        {
            _userService = userService;
        }

        public List<News> Get()
        {
            return new List<News>
            {
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "Covid",
                    Type = "news",
                    Caption = "Korona vaka sayısı",
                    Summary = "Türkiyede vaka sayısı azalıyor.",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 1,
                    IsActive = true,
                },
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "Libya",
                    Type = "news",
                    Caption = "Libya'da Hafter milislerine ikmal yapan yakıt tankeri ile iki askeri araç vuruldu",
                    Summary = "Libya karışık aga",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 1,
                    IsActive = true,
                },
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "Covid",
                    Type = "subNews",
                    Caption = "Trabzonspor'da Nwakaeme teklifi.",
                    Summary = "Her ligde olduğu gibi trabzonspor'un da liderliği tescillendi.",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 2,
                    IsActive = true,
                },
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "Covid",
                    Type = "subNews",
                    Caption = "Sokağa Çıkma Yasağı",
                    Summary = "Çankırı'da 335 kişiye ceza kesildi",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 1,
                    IsActive = true,
                }
                ,
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "KYK",
                    Type = "subNews",
                    Caption = "Kyk'dan Çıktılar",
                    Summary = "Kyk karantinası sona erdi mutlular",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 1,
                    IsActive = true,
                }
            };
        }

        public News Get(Guid id)
        {
            return new News
            {
                Id = Guid.NewGuid(),
                Category = "Covid",
                Type = "news",
                Caption = "Korona vaka sayısı",
                Summary = "Türkiyede vaka sayısı azalıyor.",
                ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                ImgAlt = "test haber img",
                Content =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                Subjects = new[] { "Covid", "Türkiye" },
                Authors = new[] { "Mustafa Çolakoğlu", "Burak Kalafat" },
                CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                Priority = 1,
                IsActive = true,
            };
        }

        public News Get(string slug)
        {
            throw new NotImplementedException();
        }


        public List<News> GetLastNews()
        {
            return new List<News>
            {
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "Covid",
                    Type = "news",
                    Caption = "Korona vaka sayısı",
                    Summary = "Türkiyede vaka sayısı azalıyor.",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 1,
                    IsActive = true,
                },
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "Libya",
                    Type = "news",
                    Caption = "Libya'da Hafter milislerine ikmal yapan yakıt tankeri ile iki askeri araç vuruldu",
                    Summary = "Libya karışık aga",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 1,
                    IsActive = true,
                },
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "Covid",
                    Type = "subNews",
                    Caption = "Trabzonspor'da Nwakaeme teklifi.",
                    Summary = "Her ligde olduğu gibi trabzonspor'un da liderliği tescillendi.",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 2,
                    IsActive = true,
                },
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "Covid",
                    Type = "subNews",
                    Caption = "Sokağa Çıkma Yasağı",
                    Summary = "Çankırı'da 335 kişiye ceza kesildi",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 1,
                    IsActive = true,
                }
                ,
                new News
                {
                    Id = Guid.NewGuid(),
                    Category = "KYK",
                    Type = "subNews",
                    Caption = "Kyk'dan Çıktılar",
                    Summary = "Kyk karantinası sona erdi mutlular",
                    ImgPath = "https://firebasestorage.googleapis.com/v0/b/news-26417.appspot.com/o/tskulis-1109230.jpg.webp?alt=media&token=d40e71b5-5180-4e9d-89b3-0297d8f11d87",
                    ImgAlt = "test haber img",
                    Content =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at nisi ex. Nullam et elit elementum, consequat risus ut, aliquet metus. Phasellus pharetra, urna non mollis auctor, dui erat fermentum lorem, sed egestas sem nisl ut augue. Duis vitae turpis non dui luctus congue. Donec iaculis, diam in consequat dapibus, tortor mauris rhoncus ex, sit amet rutrum augue arcu a lacus. Vestibulum porta, orci vitae ultrices blandit, dolor metus tristique lorem,eget placerat nisl nulla in turpis. Maecenas vel aliquam leo. Vivamus eleifend sapien vel mauris mollis imperdiet.",
                    Subjects = new[] {"Covid", "Türkiye"},
                    Authors = new[] {"Mustafa Çolakoğlu", "Burak Kalafat"},
                    CreateDate = DateTime.Parse("2020-04-23T18:25:43.511Z"),
                    UpdateDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    ExpressDate = DateTime.Parse("2020-05-01T14:35:43.511Z"),
                    Priority = 1,
                    IsActive = true,
                }
            };
        }

        public News Create(News news)
        {

            if (news.ShowNotification)
                SendNotificationAsync(news);
            return news;
        }
        private async void SendNotificationAsync(News news)
        {
            var userList = await _userService.GetUserList();
            var expoNotificationRequest = new ExpoNotificationRequest
            {
                to = userList.Select(u => u.ExpoNotificationRequest).ToArray(),
                data = news,
                title = "TS Kulis",
                body = news.Caption
            };
            _userService.SendNotification(expoNotificationRequest);
        }

        public void Update(Guid id, News newsIn)
        {
            throw new NotImplementedException();
        }

        public void ShowNotifications()
        {
            throw new NotImplementedException();
        }
        public void Remove(News newsIn)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
