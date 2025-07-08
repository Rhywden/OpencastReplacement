using MongoDB.Driver;
using OpencastReplacement.Data;
using OpencastReplacement.Models;
using RudderSingleton;
using System.Collections.Immutable;

namespace OpencastReplacement.Store
{
    public class AppStateInitializer : IStateInitializer<AppState>
    {
        private IMongoConnection _connection;
        private ILogger<AppStateInitializer> _logger;

        public AppStateInitializer(IMongoConnection connection, ILogger<AppStateInitializer> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<AppState> GetInitialStateAsync()
        {
            var videocollection = _connection.GetVideoCollection();
            var videofilter = Builders<Video>.Filter.Empty;
            var Videos = await(await videocollection.FindAsync(videofilter)).ToListAsync();
            var tagcollection = _connection.GetTagCollection();
            var tagfilter = Builders<Models.Tag>.Filter.Empty;
            var Tags = await (await tagcollection.FindAsync(tagfilter)).ToListAsync();
            var seriescollection = _connection.GetSeriesCollection();
            var seriesfilter = Builders<Models.Series>.Filter.Empty;
            var Series = await (await seriescollection.FindAsync(seriesfilter)).ToListAsync();
            var store = new AppState
            {
                Videos = ImmutableList<Video>.Empty.AddRange(Videos),
                Tags = ImmutableList<Models.Tag>.Empty.AddRange(Tags),
                Series = ImmutableList<Series>.Empty.AddRange(Series),
            };
            _logger.LogInformation($"Initialized AppState with {store.Videos.Count} videos, {store.Tags.Count} tags, and {store.Series.Count} series.");
            return store;
        }
    }
}
