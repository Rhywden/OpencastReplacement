using Fluxor;

namespace OpencastReplacement.Store.UserUseCase
{
    public static class Reducers
    {
        [ReducerMethod]
        public static UserState ReduceSetUserLoginStatusAction(UserState state, SetUserLoginStatusAction action) =>
            state with { IsLoggedIn = action.LoggedIn };
    }
}
