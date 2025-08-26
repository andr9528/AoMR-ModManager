using Newtonsoft.Json;

namespace ModManager.Abstractions.Models;

public interface IMod
{
    public const string JSON_PROPERTY_WORKSHOP_ID_NAME = "WorkshopID";
    public const string JSON_PROPERTY_INSTALL_CRC_NAME = "InstallCRC";
    public const string JSON_PROPERTY_ENABLED_NAME = "Enabled";

    public event EventHandler<bool> IsHiddenChanged;
    public event EventHandler<bool> IsHiddenSiblingChanged;
    public event EventHandler<bool> IsEnabledChanged;
    public event EventHandler<int> PriorityChanged;

    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string Path { get; set; }

    [JsonProperty(JSON_PROPERTY_WORKSHOP_ID_NAME)] public long WorkshopId { get; set; }

    public string LastUpdate { get; set; }
    public string InstallTime { get; set; }
    public int Priority { get; set; }

    [JsonProperty(JSON_PROPERTY_ENABLED_NAME)] public bool IsEnabled { get; set; }

    [JsonProperty(JSON_PROPERTY_INSTALL_CRC_NAME)] public uint? InstallCrc { get; set; }

    public bool IsHidden { get; set; }

    [JsonIgnore] public bool IsLocalMod { get; }
    [JsonIgnore] public bool IsHiddenSibling { get; set; }
    [JsonIgnore] public bool IsMissing { get; set; }
    [JsonIgnore] public Brush RowBrush { get; }
}
