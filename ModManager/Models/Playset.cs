using ModManager.Abstractions.Models;

namespace ModManager.Models;

public class Playset : IPlayset
{
    public Playset(string fileName, IModStatus modStatus)
    {
        ModStatus = modStatus;
        FileName = fileName;
    }

    /// <inheritdoc />
    public IModStatus ModStatus { get; set; }

    /// <inheritdoc />
    public string FileName { get; set; }
}
