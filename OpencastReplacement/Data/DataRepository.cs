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
            var videocollection = _connection.GetVideoCollection();
            var videofilter = Builders<Video>.Filter.Empty;
            Videos = await (await videocollection.FindAsync(videofilter)).ToListAsync();

            var tagcollection = _connection.GetTagCollection();
            var tagfilter = Builders<Models.Tag>.Filter.Empty;
            Tags = await (await tagcollection.FindAsync(tagfilter)).ToListAsync();
        }

        public DataRepository(IMongoConnection connection)
        {
            _connection = connection;
        }
    }
}
