using Fluxor;
using OpencastReplacement.Data;
using OpencastReplacement.Helpers;
using OpencastReplacement.Models;

namespace OpencastReplacement.Store.VideoUseCase
{
    public class Effects
    {
        private readonly IMongoConnection mongoConnection;

        public Effects(IMongoConnection mongoConnection)
        {
            this.mongoConnection = mongoConnection;
        }

        [EffectMethod]
        public async Task HandleFetchVideoAction(FetchVideosAction action, IDispatcher dispatcher)
        {
            var listOfVideos = new ComparableList<Video>();
            dispatcher.Dispatch(new FetchVideosResultAction(listOfVideos));
        }
    }
}
