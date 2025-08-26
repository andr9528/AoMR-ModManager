using ModManager.Abstractions.Models;

namespace ModManager.Models;

public partial class Playset : ObservableObject, IPlayset
{
    [ObservableProperty] private string fileName;

    public Playset(string fileName, IModStatus modStatus)
    {
        ModStatus = modStatus;
        FileName = fileName;
    }

    /// <inheritdoc />
    public IModStatus ModStatus { get; set; }
}
