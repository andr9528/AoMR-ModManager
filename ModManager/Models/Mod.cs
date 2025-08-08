using System.Text.Json.Serialization;
using ModManager.Abstractions.Models;

namespace ModManager.Models;

public partial class Mod : ObservableObject, IMod
{
    [ObservableProperty] private bool enabled;
    [ObservableProperty] private bool hidden;
    [ObservableProperty] private int priority;

    /// <inheritdoc />
    public string Title { get; set; }

    /// <inheritdoc />
    public string Author { get; set; }

    /// <inheritdoc />
    public string Description { get; set; }

    /// <inheritdoc />
    public string Path { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(IMod.JSON_PROPERTY_WORKSHOP_ID_NAME)]
    public long WorkshopId { get; set; }

    /// <inheritdoc />
    public string LastUpdate { get; set; }

    /// <inheritdoc />
    public string InstallTime { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(IMod.JSON_PROPERTY_INSTALL_CRC_NAME)]
    public uint? InstallCrc { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public Func<long, bool>? VisibilityResolver { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsAddModButtonVisible => VisibilityResolver?.Invoke(WorkshopId) ?? false;

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsLocalMod => Path.Contains($"local", StringComparison.InvariantCultureIgnoreCase);
}
