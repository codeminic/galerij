namespace Galerij.Api.Options;

public class GalleryOptions
{
    public const string SectionName = "Gallery";
    public required string ImageFolderPath { get; set; }
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    public int Interval { get; set; } = 60000;
    public bool AutoPlay { get; set; } = true;
}
