using OpencastReplacement.Helpers;

namespace OpencastReplacement.Models
{
    public record Video
    {
        public Guid Id { get; init; } = new Guid();
        public string UserId { get; init; } = default!;
        public string FileName { get; init; } = default!;
        public long FileSize { get; init; } = default!;
        public ComparableList<string> Tags { get; init; } = default!;
        public bool Public { get; init; } = default!;
        public bool Is480p { get; init; } = default!;
        public bool Is720p { get; init; } = default!;
        public bool Is1080p { get; init; } = default!;
        public TimeSpan Duration { get; init; } = default!;
    }
}
