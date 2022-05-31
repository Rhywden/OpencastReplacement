using MongoDB.Bson.Serialization.Attributes;
using OpencastReplacement.Helpers;
using System.Collections.Immutable;

namespace OpencastReplacement.Models
{
    public record Video : IMongoEntry
    {
        [BsonId]
        public Guid Id { get; init; } = Guid.NewGuid();
        public string UserId { get; init; } = default!;
        public string FileName { get; init; } = default!;
        public string? Titel { get; init; }
        public string? Beschreibung { get; init; }
        public string? Poster { get; init; }
        public Guid? SerienId { get; init; }
        public long FileSize { get; init; } = default!;
        [BsonSerializer(typeof(ImmutableListSerializer<string>))]
        public System.Collections.Immutable.ImmutableList<string> Tags { get; init; } = System.Collections.Immutable.ImmutableList<string>.Empty;
        public bool Public { get; init; } = true;
        public bool Is480p { get; init; } = default!;
        public bool Is720p { get; init; } = default!;
        public bool Is1080p { get; init; } = default!;
        public TimeSpan Duration { get; init; } = default!;
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
