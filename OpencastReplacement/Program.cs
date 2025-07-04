using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using OpencastReplacement;
using OpencastReplacement.Data;
using OpencastReplacement.Models;
using OpencastReplacement.Services;
using OpencastReplacement.Store;
using RudderSingleton;
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
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
{
    
})
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
        }
    };
});

builder.Services.AddTransient<ITicketStore, InMemoryTicketStore>();
builder.Services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, ConfigureCookieAuthenticationOptions>();

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
