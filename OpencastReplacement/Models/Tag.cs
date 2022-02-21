namespace OpencastReplacement.Models
{
    public record Tag
    {
        public Guid Id { get; init; } = new();
        public string Name { get; init; } = default!;
        public bool IsPrivate { get; init; } = default!;
        public string UserId { get; init; } = default!;
    }
}
