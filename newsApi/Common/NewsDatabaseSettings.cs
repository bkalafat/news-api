namespace newsApi.Common
{
    public class NewsDatabaseSettings : INewsDatabaseSettings
    {
        public string NewsCollectionName { get; set; }
        public string CommentCollectionName { get; set; }
        public string UserCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
