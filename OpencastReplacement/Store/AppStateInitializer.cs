using Rudder;

namespace OpencastReplacement.Store
{
    public class AppStateInitializer : IStateInitializer<AppState>
    {
        public Task<AppState> GetInitialStateAsync()
        {
            //TODO: Get stuff from DB and plonk it down here
            var store = new AppState
            {

            };
            return Task.FromResult(store);
        }
    }
}
