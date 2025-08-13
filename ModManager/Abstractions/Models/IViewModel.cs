using ModManager.Abstractions.Services;

namespace ModManager.Abstractions.Models;

public interface IViewModel
{
    public IStateService StateService { get; }
}
