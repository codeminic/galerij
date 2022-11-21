var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

var baseDirectory = app.Configuration["basedirectory"];
var files = Directory.GetFiles(baseDirectory);
var nextFileIndex = 0;

app.MapGet("/image/next", (HttpResponse response) =>
{
    response.Headers.CacheControl = "no-store,no-cache";
    response.Headers.Pragma = "no-cache";

    if (!files.Any())
        return Results.Problem(detail: "No images found", statusCode: 500);

    var index = nextFileIndex % files.Count();
    var filePath = files[index];
    nextFileIndex++;
    return Results.File(filePath, contentType: "image/jpeg");
});

app.Run();
