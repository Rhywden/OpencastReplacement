using OpencastReplacement.Models;
using System.Collections.Immutable;

namespace OpencastReplacement.Store
{
    public record AppState
    {
        public bool VideosAreLoading { get; init; } = false;
        public string ErrorMessage { get; init; } = string.Empty;
        public ImmutableList<Video> Videos { get; init; } = ImmutableList<Video>.Empty;
        public ImmutableList<Conversion> Conversions { get; init; } = ImmutableList<Conversion>.Empty;
        public ImmutableList<Tag> Tags { get; init; } = ImmutableList<Tag>.Empty;
        public ImmutableList<Series> Series { get; init; } = ImmutableList<Series>.Empty;
    }
}
