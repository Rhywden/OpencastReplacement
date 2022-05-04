using MongoDB.Driver;

namespace OpencastReplacement.Data
{
    public interface IMongoConnection
    {
        MongoClient Client { get; }

        public IMongoCollection<Models.Tag> GetTagCollection();
        public IMongoCollection<Models.Video> GetVideoCollection();
        public IMongoCollection<Models.Series> GetSeriesCollection();
    }
}
