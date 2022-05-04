using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Immutable;

namespace OpencastReplacement.Models
{
    public record Series
    {
        [BsonId]
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; init; } = default!;
        public ImmutableList<Guid> Videos { get; init; } = ImmutableList.Create<Guid>();
    }
}
