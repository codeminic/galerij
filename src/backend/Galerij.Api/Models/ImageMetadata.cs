namespace Galerij.Api.Models;

public class ImageMetadata
{
    public string Id { get; set; } = null!;
    public string Filename { get; set; } = null!;
    public long SizeBytes { get; set; }
    public DateTime LastModified { get; set; }
}
