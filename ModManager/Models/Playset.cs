using ModManager.Abstractions.Models;
using Newtonsoft.Json;

namespace ModManager.Models;

public partial class Playset : ObservableObject, IPlayset
{
    [ObservableProperty] private string fileName;

    [JsonConstructor]
    public Playset(string fileName, IModStatus modStatus)
    {
        ModStatus = modStatus;
        FileName = fileName;
    }

    public Playset()
    {
    }

    /// <inheritdoc />
    public IModStatus ModStatus { get; set; }
}
