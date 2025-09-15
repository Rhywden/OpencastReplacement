using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using OpencastReplacement.Data;
using OpencastReplacement.Services;
using OpencastReplacement.Store;
using RudderSingleton;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;


var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
var Environment = builder.Environment;

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();
builder.Services.AddHttpClient();

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

if (!Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        // Ensure redirect_uri is built from the public URL when behind a reverse proxy
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        // In container/reverse-proxy setups, if KnownNetworks/Proxies aren't set, headers can be ignored.
        // Clear to allow all (or set your specific proxies/networks here for stricter security).
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();

var mongodb = System.Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? Configuration["mongodb:connection"];

builder.Services.AddSingleton<IFfmpegWrapper, FfmpegWrapper>();
builder.Services.AddSingleton<IMongoConnection>(mc => new MongoConnection(mongodb, Environment));
builder.Services.AddSingleton(cm => new ConfigurationWrapper(Configuration));
builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
{
    return new BackgroundTaskQueue(100);
});
builder.Services.AddSingleton<FileQueueMonitor>();

builder.Services.AddHealthChecks();

builder.Services.AddMudServices();
builder.Services.AddRudder<AppState>(options =>
{
    options.AddStateInitializer<AppStateInitializer>();
    options.AddStateFlows();
    options.AddLogicFlows();
/*#if DEBUG
    options.AddJsLogging(); // Logging middleware
#endif*/
});

var test = System.Environment.GetEnvironmentVariable("OIDC_AUTHORITY");

builder.Services.AddAuthentication(options =>
{
    // Use cookies for interactive users; OIDC challenges will redirect to the identity provider
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.Authority = System.Environment.GetEnvironmentVariable("OIDC_AUTHORITY");
    options.ClientId = System.Environment.GetEnvironmentVariable("OIDC_CLIENT_ID");
    options.ClientSecret = System.Environment.GetEnvironmentVariable("OIDC_CLIENT_SECRET");
    options.ResponseType = "code";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("gruppen");
    options.Scope.Add("offline_access");
    options.ClaimActions.Add(new JsonKeyClaimAction("role", string.Empty, "role"));
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //map claim to name for display on the upper right corner after login.  Can be name, email, etc.
        NameClaimType = "name",
        RoleClaimType = System.Environment.GetEnvironmentVariable("ROLE_CLAIM_TYPE") ?? "groups"
    };
    options.Events = new OpenIdConnectEvents
    {
        OnAccessDenied = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/");
            return Task.CompletedTask;
        },
        OnSignedOutCallbackRedirect = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/");
            return Task.CompletedTask;
        },
        OnUserInformationReceived = ctx =>
        {
            try
            {
                var root = ctx.User.RootElement;
                if (root.TryGetProperty("groups", out var groups) && groups.ValueKind == JsonValueKind.Array)
                {
                    var id = (ClaimsIdentity)ctx.Principal!.Identity!;
                    foreach (var g in groups.EnumerateArray())
                    {
                        var value = g.GetString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            id.AddClaim(new Claim("groups", value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("OIDC").LogWarning("Failed to process OIDC userinfo groups: {ex}", ex);
            }
            return Task.CompletedTask;
        },
        OnTicketReceived = ctx =>
        {
            var id = (ClaimsIdentity)ctx.Principal!.Identity!;

            // Gather groups from claims
            var groupsValues = ctx.Principal.Claims
                .Where(c => string.Equals(c.Type, "groups", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToArray();

            // Determine roles by explicit criteria
            var rolesToAdd = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var preferredUsername = ctx.Principal.FindFirst("preferred_username")?.Value
                ?? ctx.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? ctx.Principal.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(preferredUsername) && new[] { "has", "eve", "pe" }.Contains(preferredUsername, StringComparer.OrdinalIgnoreCase))
            {
                rolesToAdd.Add("Admin");
            }

            if (groupsValues.Any(g => string.Equals(g, "lehrer", StringComparison.OrdinalIgnoreCase)))
            {
                rolesToAdd.Add("Teacher");
            }

            if (groupsValues.Any(g => string.Equals(g, "schueler", StringComparison.OrdinalIgnoreCase)))
            {
                rolesToAdd.Add("Pupil");
            }

            // Add role claims (avoid duplicates)
            foreach (var role in rolesToAdd)
            {
                if (!id.HasClaim(ClaimTypes.Role, role))
                    id.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            // Derive classroom claim now that roles may be present
            return Task.CompletedTask;
        },
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Process X-Forwarded-* headers from the reverse proxy BEFORE anything that relies on request scheme/host
    app.UseForwardedHeaders();

    app.UseExceptionHandler("/Error");
    // When behind a reverse proxy, scheme will be set by the forwarded headers above.
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
