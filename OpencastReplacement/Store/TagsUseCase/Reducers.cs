using Fluxor;
using OpencastReplacement.Helpers;

namespace OpencastReplacement.Store.TagsUseCase
{
    public static class Reducers
    {
        [ReducerMethod]
        public static TagState ReduceAddTagAction(TagState state, AddTagAction action)
        {
            if(!state.Tags.Contains(action.tag))
            {
                var list = state.Tags;
                list.Add(action.tag);
                return state with { Tags = list };
            } else
            {
                return state;
            }
        }
        [ReducerMethod]
        public static TagState ReduceDeleteTagAction(TagState state, DeleteTagAction action)
        {
            var list = state.Tags;
            list.Remove(action.tag);
            return state with { Tags = list };
        }
    }
}
