using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OpencastReplacement.Services;
using OpencastReplacement.Data;
using MudBlazor.Services;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.IdentityModel.Tokens;
using RudderSingleton;
using OpencastReplacement.Store;
using System.Security.Claims;

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
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();

builder.Services.AddSingleton<IFfmpegWrapper, FfmpegWrapper>();
builder.Services.AddSingleton<IMongoConnection>(mc => new MongoConnection(Configuration["mongodb:connection"], Environment));
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

builder.Services.AddAuthentication(options =>
{
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
        OnUserInformationReceived = async context =>
        {
            var identity = context.Principal?.Identity as ClaimsIdentity;
            if(identity is not null && context.User is not null && context.User.RootElement.TryGetProperty("groups", out var groups))
            {
                foreach (var group in groups.EnumerateArray())
                {
                    string? value = group.GetString();
                    if(string.IsNullOrEmpty(value)) continue;
                    identity.AddClaim(new Claim("groups", value));
                }
            }
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedProto
    });
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
