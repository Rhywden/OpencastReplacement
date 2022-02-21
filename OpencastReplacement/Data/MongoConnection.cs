using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace OpencastReplacement.Data
{
    public class MongoConnection : IMongoConnection
    {
        public MongoClient Client { get; private set; }

        public MongoConnection(string connectionString)
        {
            //BsonClassMap.RegisterClassMap<RocketchatMessage>();
            BsonClassMap.RegisterClassMap<Models.Tag>();
            BsonClassMap.RegisterClassMap<Models.Video>();
            Client = new MongoClient(connectionString);
        }
    }
}
