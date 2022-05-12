using Rudder;

namespace OpencastReplacement.Store
{
    public class VideoStateFlow : IStateFlow<AppState>
    {
        public AppState Handle(AppState state, object actionValue) => actionValue switch
        {
            Actions.LoadVideos.Request => state with { VideosAreLoading = true },
            Actions.LoadVideos.Success action => state with { Videos = action.videos },
            Actions.LoadVideos.Error action => state with { ErrorMessage = action.message },
            Actions.DeleteVideo.Success action => state with { Videos = action.videos },
            Actions.DeleteVideo.Error action => state with { ErrorMessage = action.message },
            _ => state
        };
    }
}
