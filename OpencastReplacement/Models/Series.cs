using MongoDB.Bson.Serialization.Attributes;
using OpencastReplacement.Helpers;
using System.Collections.Immutable;

namespace OpencastReplacement.Models
{
    public record Series : IMongoEntry
    {
        [BsonId]
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; init; } = default!;

        public string UserId { get; init; } = default!;

        [BsonSerializer(typeof(ImmutableListSerializer<Video>))]
        public ImmutableList<Video> Videos { get; init; } = ImmutableList<Video>.Empty;
    }
}
