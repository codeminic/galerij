using Galerij.Api.Endpoints;
using Galerij.Api.Options;
using Galerij.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.Configure<GalleryOptions>(builder.Configuration.GetSection(GalleryOptions.SectionName));
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddSingleton<ISessionService, SessionService>();

// Add CORS for local network access
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Map endpoints
app.MapGalleryEndpoints();

app.Run();