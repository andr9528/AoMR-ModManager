using System.Collections.ObjectModel;

namespace ModManager.Abstractions.Models;

public interface IModStatus
{
    public List<object> Actions { get; set; }
    public ObservableCollection<IMod> Mods { get; set; }
}
