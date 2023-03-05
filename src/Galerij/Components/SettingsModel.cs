namespace Galerij.Blazorized.Components;

public record SettingsModel
{
    public int PlaybackSpeedSeconds { get; set; } = 10;

    public string? SelectedCollection { get; set; } = null;

    public string BackgroundColor { get; set; } = "#808080";
}
