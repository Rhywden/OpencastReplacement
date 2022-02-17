using OpencastReplacement.Helpers;

namespace OpencastReplacement.Models
{
    public record Video
    {
        public Guid Id { get; init; } = new Guid();
        public string UserId { get; init; } = default!;
        public string FileName { get; init; } = default!;
        public string FileSize { get; init; } = default!;
        public ComparableList<string> Tags { get; init; } = default!;
        public bool Public { get; init; } = default!;
    }
}
