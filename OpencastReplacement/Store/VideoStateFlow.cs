using RudderSingleton;

namespace OpencastReplacement.Store
{
    public class VideoStateFlow : IStateFlow<AppState>
    {
        public AppState Handle(AppState state, object actionValue) => actionValue switch
        {
            Actions.VideoSuccess action => state with { Videos = action.videos },
            Actions.LoadVideos.Request => state with { VideosAreLoading = true },
            Actions.LoadVideos.Error action => state with { ErrorMessage = action.message },
            Actions.AddVideo.Error action => state with { ErrorMessage = action.message },
            Actions.DeleteVideo.Error action => state with { ErrorMessage = action.message },
            Actions.UpdateVideo.Error action => state with { ErrorMessage = action.message },
            Actions.LoadSeries.Success action => state with { Series = action.series },
            Actions.LoadSeries.Error action => state with { ErrorMessage = action.message },
            _ => state
        };
    }
}
