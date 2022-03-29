using Microsoft.AspNetCore.Components.Forms;
using OpencastReplacement.Helpers;

namespace OpencastReplacement.Models
{
    public record UploadedFile
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public IBrowserFile File { get; init; } = default!;
        public bool IsPublic { get; init; } = true;
        public ComparableList<string> Tags { get; init; } = new();
    }
}
