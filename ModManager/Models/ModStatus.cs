using System.Collections.ObjectModel;
using ModManager.Abstractions.Models;
using Newtonsoft.Json;

namespace ModManager.Models;

public class ModStatus : IModStatus
{
    /// <inheritdoc />
    public List<object> Actions { get; set; }

    /// <inheritdoc />
    public ObservableCollection<IMod> Mods { get; set; }

    [JsonConstructor]
    public ModStatus(List<object> actions, ObservableCollection<Mod> mods)
    {
        Actions = actions;
        Mods = new ObservableCollection<IMod>(mods);
    }

    public ModStatus()
    {
    }
}
