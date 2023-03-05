using Blazorise;
using Blazorise.Bulma;
using Galerij;
using Galerij.Blazorized.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorise(options => options.Immediate = true)
	.AddBulmaProviders();

builder.Services.AddSingleton<ImageService>(new ImageService("C:\\Users\\Dominic\\OneDrive\\22_Unsere Hochzeit\\Fotos"));
builder.Services.AddSingleton<SettingsService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
