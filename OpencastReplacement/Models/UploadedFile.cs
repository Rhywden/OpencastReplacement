using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Immutable;

namespace OpencastReplacement.Models
{
    public record UploadedFile
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public IBrowserFile File { get; init; } = default!;
        public string SanitizedFilename { get; init; } = default!;
        public bool IsPublic { get; init; } = true;
        public ImmutableList<string> Tags { get; init; } = ImmutableList<string>.Empty;
        public string? title { get; init; }
        public string? description { get; init; }
    }
}
