using MongoDB.Driver;
using OpencastReplacement.Events;
using OpencastReplacement.Models;

namespace OpencastReplacement.Data
{
    public class DataRepository : IDataRepository
    {
        private IMongoConnection _connection;
        private VideoAddedEvent videoAddedEvent;
        private IWebHostEnvironment hostingEnv;

        public List<Video> Videos { get; set; } = new();
        public List<Conversion> Conversions { get; set; } = new();
        public List<Models.Tag> Tags { get; set; } = new();
        public List<Series> Series { get; set; } = new();

        public async Task Init()
        {
            var videocollection = _connection.GetVideoCollection();
            var videofilter = Builders<Video>.Filter.Empty;
            Videos = await (await videocollection.FindAsync(videofilter)).ToListAsync();

            var tagcollection = _connection.GetTagCollection();
            var tagfilter = Builders<Models.Tag>.Filter.Empty;
            Tags = await (await tagcollection.FindAsync(tagfilter)).ToListAsync();
        }

        public DataRepository(IWebHostEnvironment env, IMongoConnection connection, VideoAddedEvent _videoAddedEvent)
        {
            _connection = connection;
            videoAddedEvent = _videoAddedEvent;
            hostingEnv = env;
        }

        public async void AddVideo(Video vid)
        {
            var coll = _connection.GetVideoCollection();
            await coll.InsertOneAsync(vid);
            Videos.Add(vid);
            await videoAddedEvent.Update(true);
        }

        public async void UpdateVideo(Video vid)
        {
            var coll = _connection.GetVideoCollection();
            var filter = Builders<Video>.Filter.Eq("_id", vid.Id);
            await coll.ReplaceOneAsync(filter, vid);
            int index = Videos.IndexOf(vid);
            if(index > -1)
            {
                Videos[index] = vid;
            }
            await videoAddedEvent.Update(false);
        }

        public void AddTag(Models.Tag tag)
        {
            throw new NotImplementedException();
        }

        public void UpdateTag(Models.Tag tag)
        {
            throw new NotImplementedException();
        }

        public void DeleteTag(Models.Tag tag)
        {
            throw new NotImplementedException();
        }

        public async void AddSeries(Series series)
        {
            var coll = _connection.GetSeriesCollection();
            await coll.InsertOneAsync(series);
            Series.Add(series);
            await videoAddedEvent.Update(true);
        }

        public async void UpdateSeries(Series series)
        {
            var coll = _connection.GetSeriesCollection();
            var filter = Builders<Series>.Filter.Eq("_id", series.Id);
            await coll.DeleteOneAsync(filter);
            Series.Remove(series);
            await videoAddedEvent.Update(false);
        }

        public async void DeleteSeries(Series series)
        {
            var coll = _connection.GetSeriesCollection();
            var filter = Builders<Series>.Filter.Eq("_id", series.Id);
            await coll.ReplaceOneAsync(filter, series);
            int index = Series.IndexOf(series);
            if (index > -1)
            {
                Series[index] = series;
            }
            await videoAddedEvent.Update(false);
        }
    }
}
