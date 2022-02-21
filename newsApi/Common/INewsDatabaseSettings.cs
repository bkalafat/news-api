namespace newsApi.Common
{
    public interface INewsDatabaseSettings
    {
        string NewsCollectionName { get; set; }
        string UserCollectionName { get; set; }
        string CommentCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
