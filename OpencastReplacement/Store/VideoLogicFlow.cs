using MongoDB.Driver;
using OpencastReplacement.Data;
using OpencastReplacement.Models;
using Rudder;
using System.Collections.Immutable;

namespace OpencastReplacement.Store
{
    public class VideoLogicFlow : ILogicFlow
    {
        private readonly Store<AppState> _store;
        private readonly IMongoConnection _connection;
        private readonly IWebHostEnvironment _hostingEnv;
        public VideoLogicFlow(Store<AppState> store, IMongoConnection connection, IWebHostEnvironment env)
        {
            _store = store;
            _connection = connection;
            _hostingEnv = env;
        }

        public async Task OnNext(object action)
        {
            switch(action)
            {
                case Actions.LoadVideos:
                    await LoadVideos();
                    break;
                case Actions.DeleteVideo.Request:
                    await DeleteVideo(((Actions.DeleteVideo.Request)action).videoToBeDeleted);
                    break;
            }
        }

        private async Task LoadVideos()
        {
            _store.Put(new Actions.LoadVideos.Request());
            var videocollection = _connection.GetVideoCollection();
            var videofilter = Builders<Video>.Filter.Empty;
            var Videos = await (await videocollection.FindAsync(videofilter)).ToListAsync();
            try
            {
                var videos = ImmutableList<Video>.Empty.AddRange(Videos);
                _store.Put(new Actions.LoadVideos.Success(videos: videos));
            } catch(Exception ex)
            {
                _store.Put(new Actions.LoadVideos.Error(message: ex.Message));
            }
        }
        private async Task DeleteVideo(Video video)
        {
            try
            {
                var coll = _connection.GetVideoCollection();
                var filter = Builders<Video>.Filter.Eq("_id", video.Id);
                await coll.DeleteOneAsync(filter);
                string output = Path.Combine(_hostingEnv.ContentRootPath,
                            "wwwroot", "uploads", video.FileName);
                File.Delete(output);
                var videos = _store.State.Videos.Remove(video);
                _store.Put(new Actions.DeleteVideo.Success(videos));
            } catch(Exception ex)
            {
                _store.Put(new Actions.DeleteVideo.Error(message: ex.Message));
            }
        }
    }
}
