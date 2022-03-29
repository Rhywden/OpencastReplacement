using MongoDB.Bson.Serialization.Attributes;
using OpencastReplacement.Helpers;

namespace OpencastReplacement.Models
{
    public record Video
    {
        [BsonId]
        public Guid Id { get; init; } = Guid.NewGuid();
        public string UserId { get; init; } = default!;
        public string FileName { get; init; } = default!;
        public Guid? SerienId { get; init; }
        public long FileSize { get; init; } = default!;
        public ComparableList<string> Tags { get; init; } = new();
        public bool Public { get; init; } = true;
        public bool Is480p { get; init; } = default!;
        public bool Is720p { get; init; } = default!;
        public bool Is1080p { get; init; } = default!;
        public TimeSpan Duration { get; init; } = default!;
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
