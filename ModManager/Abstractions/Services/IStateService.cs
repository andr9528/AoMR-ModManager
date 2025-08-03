using System.Collections.ObjectModel;
using ModManager.Abstractions.Models;

namespace ModManager.Abstractions.Services;

public interface IStateService
{
    IModStatus CurrentModStatus { get; set; }
    IPlayset EditingPlayset { get; set; }
    ObservableCollection<IPlayset> Playsets { get; set; }
    bool IsPlaysetActive { get; set; }
}
