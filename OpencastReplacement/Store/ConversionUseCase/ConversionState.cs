using Fluxor;
using OpencastReplacement.Helpers;
using OpencastReplacement.Models;

namespace OpencastReplacement.Store.ConversionUseCase
{
    [FeatureState]
    public record ConversionState
    {
        public ComparableList<Conversion> ConversionsInQueue { get; init; } = default!;
        public Conversion ConversionProgress { get; init; } = default!;
    }
}
