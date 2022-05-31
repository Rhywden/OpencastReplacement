using MongoDB.Driver;
using OpencastReplacement.Data;
using OpencastReplacement.Models;
using RudderSingleton;
using System.Collections.Immutable;

namespace OpencastReplacement.Store
{
    public class SeriesLogicFlow : ILogicFlow
    {
        private readonly Store<AppState> _store;
        private readonly IMongoConnection _connection;
        private readonly IWebHostEnvironment _hostingEnv;

        public SeriesLogicFlow(Store<AppState> store, IMongoConnection connection, IWebHostEnvironment hostingEnv)
        {
            _store = store;
            _connection = connection;
            _hostingEnv = hostingEnv;
        }

        public async Task OnNext(object action)
        {
            switch(action)
            {
                case Actions.LoadSeries:
                    await LoadSeries();
                    break;
                case Actions.DeleteSeries.Request:
                    await DeleteSeries(((Actions.DeleteSeries.Request)action).seriesToBeDeleted);
                    break;
                case Actions.UpdateSeries.Request:
                    await UpdateSeries(((Actions.UpdateSeries.Request)action).seriesToBeUpdated);
                    break;
                case Actions.AddSeries.Request:
                    await AddSeries(((Actions.AddSeries.Request)action).seriesToBeAdded);
                    break;
            }
        }

        private async Task LoadSeries()
        {
            try
            {
                _store.Put(new Actions.LoadSeries.Request());
                var seriesCollection = _connection.GetSeriesCollection();
                var seriesFilter = Builders<Series>.Filter.Empty;
                var Series = await (await seriesCollection.FindAsync(seriesFilter)).ToListAsync();
                var series = ImmutableList<Series>.Empty.AddRange(Series);
                _store.Put(new Actions.SeriesSuccess(series));
            } catch(Exception ex)
            {
                _store.Put(new Actions.LoadVideos.Error(ex.Message));
            }
        }
        private async Task DeleteSeries(Series series)
        {
            try
            {
                var coll = _connection.GetSeriesCollection();
                var filter = Builders<Series>.Filter.Eq("_id", series.Id);
                await coll.DeleteOneAsync(filter);
                var newSeries = _store.State.Series.Remove(series);
                _store.Put(new Actions.SeriesSuccess(newSeries));
            }
            catch (Exception ex)
            {
                _store.Put(new Actions.DeleteSeries.Error(message: ex.Message));
            }
        }
        private async Task UpdateSeries(Series series)
        {
            try
            {
                var coll = _connection.GetSeriesCollection();
                var filter = Builders<Series>.Filter.Eq("_id", series.Id);
                await coll.ReplaceOneAsync(filter, series);
                
                int index = _store.State.Series.FindIndex(se => se.Id.Equals(series.Id));
                if (index != -1)
                {
                    var newSeries = _store.State.Series.SetItem(index, series);
                    _store.Put(new Actions.SeriesSuccess(newSeries));
                }
            } catch(Exception ex)
            {
                _store.Put(new Actions.DeleteSeries.Error(ex.Message));
            }
        }
        private async Task AddSeries(Series series)
        {
            try
            {
                var coll = _connection.GetSeriesCollection();
                await coll.InsertOneAsync(series);
                var newSeries = _store.State.Series.Add(series);
                _store.Put(new Actions.SeriesSuccess(newSeries));
            } catch(Exception ex)
            {
                _store.Put(new Actions.AddSeries.Error(ex.Message));
            }
        }
    }
}
