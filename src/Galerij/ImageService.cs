using System.Collections.Concurrent;

namespace Galerij;

public class ImageService
{
    private SemaphoreSlim _semaphore = new(1);
    private bool _isInitialized = false;
    private readonly string baseDirectory;

    private readonly ConcurrentBag<ImageCollection> imageCollections = new ();

    public ImageService(string baseDirectory)
    {
        this.baseDirectory = baseDirectory;
    }

    public async Task<IEnumerable<string>> GetCollections()
    {
        await EnsureInitialized();
        return imageCollections.Select(collection => collection.CollectionName);
    }

    public async Task<IEnumerable<ImageItem>> GetImages(string collectionName)
    {
        await EnsureInitialized();
        var collection = imageCollections.Single(c => c.CollectionName == collectionName);
        return collection.Images;
    }

    public async Task<IEnumerable<ImageItem>> GetAllImages()
    {
        await EnsureInitialized();
        var images = imageCollections.SelectMany(c => c.Images);
        return images;
    }

    public async Task<Stream> GetImageData(ImageItem image)
    {
        var memoryStream = new MemoryStream();
        var fileStream = File.OpenRead(image.Path);
        await fileStream.CopyToAsync(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    private async Task Reload()
    {
        _isInitialized = false;
        await EnsureInitialized();
    }

    private async Task EnsureInitialized()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_isInitialized)
                return;

            var directories = Directory.GetDirectories(baseDirectory);
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory, "*.jpg");

                if (!files.Any())
                    continue;

                var imageItems = files.Select(file => new ImageItem(file, null));
                var directoryInfo = new DirectoryInfo(directory);
                var imageCollection = new ImageCollection(directoryInfo.Name, imageItems);
                imageCollections.Add(imageCollection);
            }
        }
        finally
        {
            _semaphore.Release();
            _isInitialized = true;
        }
    }
}

public record ImageCollection(string CollectionName, IEnumerable<ImageItem> Images);

public record ImageItem(string Path, ImageMetadata? metadata);

public record ImageMetadata(string Path, int? ISO, int? Aperture, int? ShutterSpeed);