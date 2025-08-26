using ModManager.Abstractions.Models;
using Newtonsoft.Json;

// They are valid, as code would throw exception without them.
#pragma warning disable CS0657 // Not a valid attribute location for this declaration

namespace ModManager.Models;

public partial class Mod : ObservableObject, IMod
{
    [ObservableProperty]
    [property: JsonProperty(IMod.JSON_PROPERTY_ENABLED_NAME)]
    [NotifyPropertyChangedFor(nameof(RowBrush))]
    private bool isEnabled;

    [ObservableProperty] private bool isHidden;
    [ObservableProperty] [property: JsonIgnore] private bool isHiddenSibling;
    [ObservableProperty] private int priority;

    [ObservableProperty] [property: JsonIgnore] [NotifyPropertyChangedFor(nameof(RowBrush))]
    private bool isMissing;

    public event EventHandler<bool> IsHiddenChanged;
    public event EventHandler<bool> IsHiddenSiblingChanged;
    public event EventHandler<bool> IsEnabledChanged;
    public event EventHandler<int> PriorityChanged;

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
    [JsonIgnore]
    public bool IsLocalMod => Path.Contains($"local", StringComparison.InvariantCultureIgnoreCase);

    [JsonIgnore] public Brush RowBrush => GetRowBrush();

    private SolidColorBrush GetRowBrush()
    {
        if (IsMissing && IsLocalMod)
        {
            return new SolidColorBrush(Constants.UiColors.MissingLocalRowColor);
        }

        if (IsMissing)
        {
            return new SolidColorBrush(Constants.UiColors.MissingRowColor);
        }

        return IsEnabled
            ? new SolidColorBrush(Constants.UiColors.OnRowColor)
            : new SolidColorBrush(Constants.UiColors.OffRowColor);
    }

    partial void OnIsHiddenChanged(bool oldValue, bool newValue)
    {
        IsHiddenChanged?.Invoke(this, newValue);
    }

    partial void OnIsHiddenSiblingChanged(bool oldValue, bool newValue)
    {
        IsHiddenSiblingChanged?.Invoke(this, newValue);
    }

    partial void OnIsEnabledChanged(bool oldValue, bool newValue)
    {
        IsEnabledChanged?.Invoke(this, newValue);
    }

    partial void OnPriorityChanged(int oldValue, int newValue)
    {
        PriorityChanged?.Invoke(this, newValue);
    }
}
