using System.Text.Json.Serialization;
using ModManager.Abstractions.Models;
using Newtonsoft.Json;

namespace ModManager.Models;

public partial class Mod : ObservableObject, IMod
{
    [ObservableProperty] [JsonProperty(IMod.JSON_PROPERTY_ENABLED_NAME)]
    private bool isEnabled;

    [ObservableProperty] private bool isHidden;
    [ObservableProperty] private bool isHiddenSibling;
    [ObservableProperty] private int priority;

    public event EventHandler<bool> IsHiddenChanged;
    public event EventHandler<bool> IsHiddenSiblingChanged;

    /// <inheritdoc />
    public string Title { get; set; }

    /// <inheritdoc />
    public string Author { get; set; }

    /// <inheritdoc />
    public string Description { get; set; }

    /// <inheritdoc />
    public string Path { get; set; }

    /// <inheritdoc />
    [JsonProperty(IMod.JSON_PROPERTY_WORKSHOP_ID_NAME)]
    public long WorkshopId { get; set; }

    /// <inheritdoc />
    public string LastUpdate { get; set; }

    /// <inheritdoc />
    public string InstallTime { get; set; }

    /// <inheritdoc />
    [JsonProperty(IMod.JSON_PROPERTY_INSTALL_CRC_NAME)]
    public uint? InstallCrc { get; set; }

    /// <inheritdoc />
    [Newtonsoft.Json.JsonIgnore]
    public bool IsLocalMod => Path.Contains($"local", StringComparison.InvariantCultureIgnoreCase);

    partial void OnIsHiddenChanged(bool oldValue, bool newValue)
    {
        IsHiddenChanged?.Invoke(this, newValue);
    }

    partial void OnIsHiddenSiblingChanged(bool oldValue, bool newValue)
    {
        IsHiddenSiblingChanged?.Invoke(this, newValue);
    }
}
