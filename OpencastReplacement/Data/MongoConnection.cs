using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace OpencastReplacement.Data
{
    public class MongoConnection : IMongoConnection
    {
        public MongoClient Client { get; private set; }

        public MongoConnection(string connectionString)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonClassMap.RegisterClassMap<Models.Tag>();
            BsonClassMap.RegisterClassMap<Models.Video>();
            Client = new MongoClient(connectionString);
        }
    }
}
