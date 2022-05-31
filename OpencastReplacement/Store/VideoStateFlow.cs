using RudderSingleton;

namespace OpencastReplacement.Store
{
    public class VideoStateFlow : IStateFlow<AppState>
    {
        public AppState Handle(AppState state, object actionValue) => actionValue switch
        {
            Actions.VideoSuccess action => state with { Videos = action.videos, VideosAreLoading = false },
            Actions.LoadVideos.Request => state with { VideosAreLoading = true },
            Actions.LoadVideos.Error action => state with { ErrorMessage = action.message },
            Actions.AddVideo.Error action => state with { ErrorMessage = action.message },
            Actions.DeleteVideo.Error action => state with { ErrorMessage = action.message },
            Actions.UpdateVideo.Error action => state with { ErrorMessage = action.message },

            Actions.SeriesSuccess action => state with { Series = action.series, SeriesAreLoading = false },
            Actions.LoadSeries.Request => state with { SeriesAreLoading = true },
            Actions.LoadSeries.Error action => state with { ErrorMessage = action.message },
            Actions.AddSeries.Error action => state with { ErrorMessage = action.message },
            Actions.UpdateSeries.Error action => state with { ErrorMessage = action.message },
            Actions.DeleteSeries.Error action => state with { ErrorMessage = action.message },

            Actions.TagSuccess action => state with { Tags = action.tags, TagsAreLoading = false },
            Actions.LoadTags.Request => state with { TagsAreLoading = true },
            Actions.LoadTags.Error action => state with { ErrorMessage = action.message },
            Actions.AddTag.Error action => state with { ErrorMessage = action.message },
            Actions.UpdateTag.Error action => state with { ErrorMessage = action.message },
            Actions.DeleteTag.Error action => state with { ErrorMessage = action.message },

            Actions.ConversionSuccess action => state with { Conversions = action.conversions },
            Actions.AddConversion.Error action => state with { ErrorMessage = action.message },
            Actions.UpdateConversion.Error action => state with { ErrorMessage = action.message },
            Actions.DeleteConversion.Error action => state with { ErrorMessage = action.message },
            _ => state
        };
    }
}
