using System;
using System.Collections.Generic;
using newsApi.Models;

namespace newsApi.Data
{
    public class MockNewsService : INewsService
    {
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
                    ImgPath = "https://i4.hurimg.com/i/hurriyet/75/750x422/5eaec3097af5072a587581f7.jpg",
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
                    ImgPath = "https://cdnuploads.aa.com.tr/uploads/Contents/2020/05/03/thumbs_b_c_e1a109ca046e8f74f310eaa8e012e09d.jpg",
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
                    ImgPath = "https://i12.haber7.net//haber/haber7/photos/2019/39/karamandan_nwakaemeye_ozel_gorev_1569743374_9174.jpg",
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
                    ImgPath = "https://icdn.ensonhaber.com/resimler/diger//kok/2020/05/08/koronavirus_3102.jpg",
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
                    ImgPath = "https://icdn.ensonhaber.com/resimler/diger/kok/2020/05/08/koronavirus_6087.jpg",
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
                ImgPath = "https://via.placeholder.com/600x300?text=KORONA",
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

        public News Get(string url)
        {
            throw new NotImplementedException();
        }

        public News Create(News news)
        {
            throw new NotImplementedException();
        }

        public void Update(Guid id, News newsIn)
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
