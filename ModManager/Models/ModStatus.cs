using ModManager.Abstractions.Models;
using Newtonsoft.Json;

namespace ModManager.Models;

public class ModStatus : IModStatus
{
    /// <inheritdoc />
    public List<object> Actions { get; set; }

    /// <inheritdoc />
    public List<IMod> Mods { get; set; }

    [JsonConstructor]
    public ModStatus(List<object> actions, List<Mod> mods)
    {
        Actions = actions;
        Mods = mods.Cast<IMod>().ToList();
    }

    public ModStatus()
    {
    }
}
