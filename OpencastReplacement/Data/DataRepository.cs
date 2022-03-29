using MongoDB.Driver;
using OpencastReplacement.Models;

namespace OpencastReplacement.Data
{
    public class DataRepository : IDataRepository
    {
        private IMongoConnection _connection;

        public List<Video> Videos { get; set; } = new();
        public List<Conversion> Conversions { get; set; } = new();
        public List<Models.Tag> Tags { get; set; } = new();

        public async Task Init()
        {
            var db = _connection.Client.GetDatabase("videoserver");
            var videocollection = db.GetCollection<Video>("videos");
            var videofilter = Builders<Video>.Filter.Empty;
            Videos = await (await videocollection.FindAsync(videofilter)).ToListAsync();

            var tagcollection = db.GetCollection<Models.Tag>("tags");
            var tagfilter = Builders<Models.Tag>.Filter.Empty;
            Tags = await (await tagcollection.FindAsync(tagfilter)).ToListAsync();
        }

        public DataRepository(IMongoConnection connection)
        {
            _connection = connection;
        }
    }
}
