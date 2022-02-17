namespace OpencastReplacement.Models
{
    public record Conversion
    {
        public Guid ConversionId { get; init; } = new Guid();
        public int Progress { get; init; }
        public string FileName { get; init; } = default!;
        public string UserName { get; init; } = default!;
        public bool HasStarted { get; init; }
    }
}
