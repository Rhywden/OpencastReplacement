using Fluxor;

namespace OpencastReplacement.Store.VideoUseCase
{
    public static class Reducers
    {
        [ReducerMethod(typeof(FetchVideosAction))]
        public static VideoState ReduceFetchVideoAction(VideoState state) =>
            state;

        [ReducerMethod]
        public static VideoState ReduceFetchVideoResultAction(VideoState state, FetchVideosResultAction action) =>
            state with { Videos = action.videos };
    }
}
