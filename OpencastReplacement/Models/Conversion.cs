namespace OpencastReplacement.Models
{
    public record Conversion
    {
        public Guid ConversionId { get; init; } = Guid.NewGuid();
        public double Progress { get; init; }
        public bool HasStarted { get; init; }
        public Guid VideoId { get; init; } = Guid.NewGuid();
        public string Filename { get; init; } = default!;
    }
}
