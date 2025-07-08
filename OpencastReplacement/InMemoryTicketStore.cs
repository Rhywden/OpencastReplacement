using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace OpencastReplacement
{
    public sealed class InMemoryTicketStore : ITicketStore
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<InMemoryTicketStore> _logger;
        public InMemoryTicketStore(IMemoryCache cache, ILogger<InMemoryTicketStore> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _cache.Set(key, ticket);
            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            var ticket = _cache.Get<AuthenticationTicket>(key);
            return Task.FromResult(ticket);
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            string claimsList = string.Join(", ", ticket.Principal.Claims.Select(c => $"{c.Type}: {c.Value}"));
            _logger.LogInformation($"Storing authentication ticket with claims: {claimsList}");
            var key = ticket.Principal.Claims.First(c => c.Type == "preferred_username").Value;
            _cache.Set(key, ticket);
            return Task.FromResult(key);
        }
    }
}
