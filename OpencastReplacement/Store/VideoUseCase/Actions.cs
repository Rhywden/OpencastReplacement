using OpencastReplacement.Models;

namespace OpencastReplacement.Store.VideoUseCase
{
    public record AddVideoAction(Video video);
    public record DeleteVideoAction(Video video);
    public record UpdateVideoAction(Video video);
}
