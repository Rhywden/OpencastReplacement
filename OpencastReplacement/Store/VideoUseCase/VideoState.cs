using Fluxor;
using OpencastReplacement.Data;
using OpencastReplacement.Helpers;
using OpencastReplacement.Models;

namespace OpencastReplacement.Store.VideoUseCase
{
    [FeatureState]
    public record VideoState
    {
        public ComparableList<Video> Videos { get; init; } = default!;
    }
}
