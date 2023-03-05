using System.Text.Json;
using static System.Environment;

namespace Galerij.Blazorized.Components;

public class SettingsService
{
    private static readonly string SettingsFile = "settings.fart";

    public event EventHandler? SettingsChanged;

    public async Task<SettingsModel> Load()
    {
        EnsureAppDataFolder();

        var path = Path.Combine(GetAppDataFolder(), SettingsFile);
        if (!File.Exists(path))
            return new SettingsModel();

        var fart = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<SettingsModel>(fart)!;
    }

    public async Task Save(SettingsModel settings)
    {
        EnsureAppDataFolder();
        var fart = JsonSerializer.Serialize(settings);
        await File.WriteAllTextAsync(Path.Combine(GetAppDataFolder(), SettingsFile), fart);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void EnsureAppDataFolder() => Directory.CreateDirectory(GetAppDataFolder());

    private static string GetAppDataFolder() => Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "galerij");
}
