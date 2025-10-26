using Galerij.Api.Models;
using Galerij.Api.Options;
using Microsoft.Extensions.Options;

namespace Galerij.Api.Services;

public interface IImageService
{
    Task<List<ImageMetadata>> GetImagesAsync();
    Task<ImageMetadata?> GetRandomImageAsync();
    Task<Stream?> GetImageStreamAsync(string imageId);
}

public class ImageService(IOptions<GalleryOptions> options, ILogger<ImageService> logger)
    : IImageService
{
    private readonly GalleryOptions _options = options.Value;
    private DirectoryInfo? _imageDirectory;


    public Task<List<ImageMetadata>> GetImagesAsync()
    {
        try
        {
            var directory = GetImageDirectory();
            if (!directory.Exists)
            {
                logger.LogWarning("Image folder does not exist: {Path}", directory.FullName);
                return Task.FromResult(new List<ImageMetadata>());
            }

            var images = new List<ImageMetadata>();

            foreach (var file in directory.GetFiles())
            {
                if (_options.AllowedExtensions.Contains(file.Extension.ToLowerInvariant()))
                {
                    images.Add(new ImageMetadata
                    {
                        Id = GetImageIdFromFilename(file.Name),
                        Filename = file.Name,
                        SizeBytes = file.Length,
                        LastModified = file.LastWriteTimeUtc
                    });
                }
            }

            return Task.FromResult(images.OrderBy(x => x.Filename).ToList());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading images from folder");
            return Task.FromResult(new List<ImageMetadata>());
        }
    }

    public async Task<ImageMetadata?> GetRandomImageAsync()
    {
        try
        {
            var images = await GetImagesAsync();
            if (images.Count == 0)
            {
                return null;
            }

            var random = new Random();
            var randomIndex = random.Next(images.Count);
            return images[randomIndex];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting random image");
            return null;
        }
    }

    public Task<Stream?> GetImageStreamAsync(string imageId)
    {
        try
        {
            var directory = GetImageDirectory();
            if (!directory.Exists)
            {
                return Task.FromResult<Stream?>(null);
            }

            // Decode the image ID back to filename
            var filename = System.Net.WebUtility.UrlDecode(imageId);

            // Prevent directory traversal attacks
            if (filename.Contains("..") || Path.IsPathRooted(filename))
            {
                logger.LogWarning("Invalid image path attempted: {Path}", filename);
                return Task.FromResult<Stream?>(null);
            }

            var filePath = Path.Combine(directory.FullName, filename);
            var fileInfo = new FileInfo(filePath);

            // Verify the file is within the image directory
            if (!fileInfo.FullName.StartsWith(directory.FullName))
            {
                logger.LogWarning("Attempted to access file outside image directory: {Path}", filePath);
                return Task.FromResult<Stream?>(null);
            }

            // Check if the file has an allowed extension
            if (!_options.AllowedExtensions.Contains(fileInfo.Extension.ToLowerInvariant()))
            {
                logger.LogWarning("File has disallowed extension: {Path}", filePath);
                return Task.FromResult<Stream?>(null);
            }

            if (fileInfo.Exists)
            {
                Stream? stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                return Task.FromResult<Stream?>(stream);
            }

            return Task.FromResult<Stream?>(null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error opening image stream for ID: {ImageId}", imageId);
            return Task.FromResult<Stream?>(null);
        }
    }

    public string GetImageIdFromFilename(string filename)
    {
        return System.Net.WebUtility.UrlEncode(filename);
    }

    private DirectoryInfo GetImageDirectory()
    {
        if (_imageDirectory == null)
        {
            var path = _options.ImageFolderPath;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(AppContext.BaseDirectory, path);
            }
            _imageDirectory = new DirectoryInfo(path);
        }

        return _imageDirectory;
    }
}
