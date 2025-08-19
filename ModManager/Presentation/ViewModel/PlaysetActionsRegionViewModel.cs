using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation.ViewModel;

public class PlaysetActionsRegionViewModel : IViewModel
{
    /// <inheritdoc />
    public IStateService StateService { get; }

    public PlaysetActionsRegionViewModel(IStateService stateService)
    {
        StateService = stateService;
    }
}
