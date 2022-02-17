using Fluxor;
using OpencastReplacement.Helpers;
using OpencastReplacement.Models;

namespace OpencastReplacement.Store.ConversionUseCase
{
    public record ConversionState
    {
        public ComparableList<Conversion> ConversionsInProgress { get; init; } = default!;
    }
}
