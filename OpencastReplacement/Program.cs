using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OpencastReplacement.Services;
using Fluxor;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
var Environment = builder.Environment;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddFluxor(opt =>
{
    opt.ScanAssemblies(typeof(Program).Assembly);
    opt.UseRouting();
    opt.UseReduxDevTools();
});

builder.Services.AddSingleton<IFfmpegWrapper>(sp => new FfmpegWrapper(pathToExecutable: Configuration["ffmpeg:exepath"], pathToStorageFolder: Configuration["ffmpeg:storagepath"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
