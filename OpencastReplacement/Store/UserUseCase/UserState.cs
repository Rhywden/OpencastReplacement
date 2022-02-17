using Fluxor;

namespace OpencastReplacement.Store.UserUseCase
{
    [FeatureState]
    public record UserState
    {
        public string UserName { get; init; } = default!;
        //TODO Add User Principal when Auth plugged in
        public bool IsLoggedIn { get; set; } = false;
    }
}
