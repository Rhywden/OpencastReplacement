using OpencastReplacement.Models;
using Rudder;
using System.Collections.Immutable;

namespace OpencastReplacement.Store
{
    public class VideoLogicFlow : ILogicFlow
    {
        private readonly Store<AppState> _store;
        public VideoLogicFlow(Store<AppState> store)
        {
            _store = store;
        }

        public async Task OnNext(object action)
        {
            switch(action)
            {
                case Actions.LoadVideos:
                    await LoadVideos();
                    break;
            }
        }

        private async Task LoadVideos()
        {
            _store.Put(new Actions.LoadVideos.Request());
            try
            {
                var videos = ImmutableList<Video>.Empty;
                _store.Put(new Actions.LoadVideos.Success(videos: videos));
            } catch(Exception ex)
            {
                _store.Put(new Actions.LoadVideos.Error(message: ex.Message));
            }
        }
    }
}
