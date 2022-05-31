namespace OpencastReplacement.Models
{
    public record Tag : IMongoEntry
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; init; } = default!;
        public bool IsPrivate { get; init; } = default!;
        public string UserId { get; init; } = default!;
    }
}
