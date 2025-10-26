using Galerij.Api.Models;
using Galerij.Api.Services;

namespace Galerij.Api.Endpoints;

public static class GalleryEndpoints
{
    public static void MapGalleryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/gallery")
            .WithName("Gallery");

        group.MapGet("/images", GetImages)
            .WithName("GetImages");

        group.MapGet("/random-image", GetRandomImage)
            .WithName("GetRandomImage");

        group.MapGet("/image/{imageId}", GetImage)
            .WithName("GetImage");

        group.MapGet("/settings", GetSettings)
            .WithName("GetSettings");

        group.MapPost("/settings", UpdateSettings)
            .WithName("UpdateSettings");

        group.MapGet("/config", GetConfig)
            .WithName("GetConfig");
    }

    private static async Task<IResult> GetImages(IImageService imageService)
    {
        var images = await imageService.GetImagesAsync();
        return Results.Ok(images);
    }

    private static async Task<IResult> GetRandomImage(IImageService imageService)
    {
        var image = await imageService.GetRandomImageAsync();
        if (image == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(image);
    }

    private static async Task<IResult> GetImage(string imageId, IImageService imageService)
    {
        var stream = await imageService.GetImageStreamAsync(imageId);
        if (stream == null)
        {
            return Results.NotFound();
        }

        // Determine content type based on file extension
        var filename = System.Net.WebUtility.UrlDecode(imageId);
        var ext = Path.GetExtension(filename).ToLowerInvariant();
        var contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };

        return Results.Stream(stream, contentType);
    }

    private static IResult GetSettings(HttpContext context, ISessionService sessionService)
    {
        var sessionId = GetSessionId(context);
        var settings = sessionService.GetOrCreateSessionSettings(sessionId);
        return Results.Ok(settings);
    }

    private static IResult UpdateSettings(HttpContext context, GallerySettings settings, ISessionService sessionService)
    {
        var sessionId = GetSessionId(context);
        sessionService.SaveSessionSettings(sessionId, settings);
        return Results.Ok(settings);
    }

    private static IResult GetConfig(HttpContext context)
    {
        var config = new
        {
            SessionId = GetSessionId(context)
        };
        return Results.Ok(config);
    }

    private static string GetSessionId(HttpContext context)
    {
        const string headerName = "X-Session-Id";

        if (context.Request.Headers.TryGetValue(headerName, out var value))
        {
            return value.ToString();
        }

        // If no session ID provided, create one based on request (this shouldn't happen in normal usage)
        return Guid.NewGuid().ToString();
    }
}
