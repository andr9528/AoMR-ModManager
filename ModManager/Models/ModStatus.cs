using ModManager.Abstractions.Models;

namespace ModManager.Models;

public class ModStatus : IModStatus
{
    /// <inheritdoc />
    public List<object> Actions { get; set; } = [];

    /// <inheritdoc />
    public List<IMod> Mods { get; set; } = [];
}
