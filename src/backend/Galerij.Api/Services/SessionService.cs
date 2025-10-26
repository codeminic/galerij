using Galerij.Api.Models;
using Galerij.Api.Options;
using Microsoft.Extensions.Options;

namespace Galerij.Api.Services;

public interface ISessionService
{
    GallerySettings GetOrCreateSessionSettings(string sessionId);
    void SaveSessionSettings(string sessionId, GallerySettings settings);
}

public class SessionService : ISessionService
{
    private readonly GalleryOptions _options;
    private readonly ILogger<SessionService> _logger;
    private readonly Dictionary<string, GallerySettings> _sessionStore;
    private readonly string _sessionStorePath;
    private readonly object _lockObject = new();

    public SessionService(IOptions<GalleryOptions> options, ILogger<SessionService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _sessionStore = new Dictionary<string, GallerySettings>();
        _sessionStorePath = Path.Combine(AppContext.BaseDirectory, "sessions.json");

        LoadSessionsFromDisk();
    }

    public GallerySettings GetOrCreateSessionSettings(string sessionId)
    {
        lock (_lockObject)
        {
            if (_sessionStore.TryGetValue(sessionId, out var settings))
            {
                return settings;
            }

            var newSettings = new GallerySettings
            {
                Interval = _options.Interval,
                AutoPlay = _options.AutoPlay
            };

            _sessionStore[sessionId] = newSettings;
            SaveSessionsToDisk();

            return newSettings;
        }
    }

    public void SaveSessionSettings(string sessionId, GallerySettings settings)
    {
        lock (_lockObject)
        {
            _sessionStore[sessionId] = settings;
            SaveSessionsToDisk();
        }
    }

    private void LoadSessionsFromDisk()
    {
        try
        {
            if (File.Exists(_sessionStorePath))
            {
                var json = File.ReadAllText(_sessionStorePath);
                var sessions = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, GallerySettings>>(json);
                if (sessions != null)
                {
                    foreach (var kvp in sessions)
                    {
                        _sessionStore[kvp.Key] = kvp.Value;
                    }
                    _logger.LogInformation("Loaded {Count} sessions from disk", sessions.Count);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading sessions from disk");
        }
    }

    private void SaveSessionsToDisk()
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_sessionStore);
            File.WriteAllText(_sessionStorePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving sessions to disk");
        }
    }
}
