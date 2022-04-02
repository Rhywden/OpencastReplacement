using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OpencastReplacement.Models;

namespace OpencastReplacement.Data
{
    public class MongoConnection : IMongoConnection
    {
        public MongoClient Client { get; private set; }
        private string database;

        public MongoConnection(string connectionString, IWebHostEnvironment env)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonClassMap.RegisterClassMap<Models.Tag>();
            BsonClassMap.RegisterClassMap<Models.Video>();
            Client = new MongoClient(connectionString);
            database = env.IsDevelopment() ? "videoserver_dev" : "videoserver";
        }

        public IMongoCollection<Models.Tag> GetTagCollection()
        {
            return Client.GetDatabase(database).GetCollection<Models.Tag>("tags");
        }

        public IMongoCollection<Video> GetVideoCollection()
        {
            return Client.GetDatabase(database).GetCollection<Models.Video>("videos");
        }
    }
}
