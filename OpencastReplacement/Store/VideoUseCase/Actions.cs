using OpencastReplacement.Helpers;
using OpencastReplacement.Models;

namespace OpencastReplacement.Store.VideoUseCase
{
    public record FetchVideosAction();
    public record FetchVideosResultAction(ComparableList<Video> videos);
    public record AddVideoAction(Video video);
    public record DeleteVideoAction(Video video);
    public record UpdateVideoAction(Video video);
}
