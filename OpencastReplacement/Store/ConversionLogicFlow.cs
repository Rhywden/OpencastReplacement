using OpencastReplacement.Models;
using RudderSingleton;

namespace OpencastReplacement.Store
{
    public class ConversionLogicFlow : ILogicFlow
    {
        private readonly Store<AppState> _store;
        public ConversionLogicFlow(Store<AppState> store)
        {
            _store = store;
        }

        public Task OnNext(object action)
        {
            switch(action)
            {
                case Actions.AddConversion.Request:
                    AddConversion(((Actions.AddConversion.Request)action).conversionToBeAdded);
                    break;
                case Actions.UpdateConversion.Request:
                    UpdateConversion(((Actions.UpdateConversion.Request)action).conversionToBeUpdated);
                    break;
                case Actions.DeleteConversion.Request:
                    DeleteConversion(((Actions.DeleteConversion.Request)action).conversionToBeDeleted);
                    break;
            }
            return Task.CompletedTask;
        }

        private void AddConversion(Conversion conversion)
        {
            try
            {
                _store.Put(new Actions.ConversionSuccess(_store.State.Conversions.Add(conversion)));
                
            } catch(Exception ex)
            {
                _store.Put(new Actions.AddConversion.Error(ex.Message));
            }
        }
        private void UpdateConversion(Conversion conversion)
        {
            try
            {
                int index = _store.State.Conversions.FindIndex(co => co.VideoId.Equals(conversion.VideoId));
                {
                    _store.Put(new Actions.ConversionSuccess(_store.State.Conversions.SetItem(index, conversion)));
                }
            } catch(Exception ex)
            {
                _store.Put(new Actions.UpdateConversion.Error(ex.Message));
            }
        }
        private void DeleteConversion(Conversion conversion)
        {
            try
            {
                int index = _store.State.Conversions.FindIndex(co => co.VideoId.Equals(conversion.VideoId));
                if (index != -1)
                {
                    _store.Put(new Actions.ConversionSuccess(_store.State.Conversions.RemoveAt(index)));
                }
            } catch(Exception ex)
            {
                _store.Put(new Actions.DeleteConversion.Error(ex.Message));
            }
        }
    }
}
