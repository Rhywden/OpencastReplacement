namespace OpencastReplacement.Models
{
    public class TagResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Tag { get; set; } = default!;
    }
}
