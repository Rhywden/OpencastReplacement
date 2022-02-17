using Fluxor;

namespace OpencastReplacement.Store.ConversionUseCase
{
    public static class Reducers
    {
        [ReducerMethod]
        public static ConversionState ReduceSetConversionProgressAction(ConversionState state, SetConversionProgressAction action) {
            int index = state.ConversionsInProgress.FindIndex(c => c.ConversionId == action.ConversionId);
            var list = state.ConversionsInProgress;
            if(index > -1)
            {
                list[index] = list[index] with { Progress = action.Progress};
                return state with { ConversionsInProgress = list };
            } else
            {
                return state;
            }
        }
    }
}
