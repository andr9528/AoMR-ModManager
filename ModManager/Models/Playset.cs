using ModManager.Abstractions.Models;

namespace ModManager.Models;

public class Playset : IPlayset
{
    /// <inheritdoc />
    public IModStatus ModStatus { get; set; }

    /// <inheritdoc />
    public string FileName { get; set; }
}
