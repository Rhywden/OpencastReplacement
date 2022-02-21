using MongoDB.Driver;

namespace OpencastReplacement.Data
{
    public interface IMongoConnection
    {
        MongoClient Client { get; }
    }
}
