using Fluxor;
using OpencastReplacement.Helpers;
using OpencastReplacement.Models;

namespace OpencastReplacement.Store.TagsUseCase
{
    [FeatureState]
    public record TagState
    {
        public ComparableList<Tag> Tags { get; init; } = default!;
    }
}
