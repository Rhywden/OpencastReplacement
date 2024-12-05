using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using OpencastReplacement.Data;
using OpencastReplacement.Services;

namespace OpencastReplacement.Health
{
    public class DBHealthCheck(ConfigurationWrapper configurationWrapper, IWebHostEnvironment webHostEnvironment) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var connString = configurationWrapper.ConfigurationManager["mongodb:connection"];
            try
            {
                var client = new MongoConnection(connString!, webHostEnvironment);
                var collection = client.GetVideoCollection();
                var filter = new BsonDocument();
                var total = await collection.CountDocumentsAsync(filter);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }
    }
}
