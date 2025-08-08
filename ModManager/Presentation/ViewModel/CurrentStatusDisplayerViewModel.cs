using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation.ViewModel;

public class CurrentStatusDisplayerViewModel
{
    public IStateService StateService { get; }

    public CurrentStatusDisplayerViewModel(IStateService stateService)
    {
        StateService = stateService;

        StateService.CurrentModStatusChanged += BindVisibilityResolver;
        StateService.InitializationCompleted += StateService_InitializationCompleted;
    }

    private void StateService_InitializationCompleted(object? sender, bool e)
    {
        StateService.CurrentModStatus?.Mods.ForEach(x => x.VisibilityResolver = IsAddModButtonVisible);
    }

    private void BindVisibilityResolver(object? sender, IModStatus? newCurrentModStatus)
    {
        newCurrentModStatus?.Mods.ForEach(x => x.VisibilityResolver = IsAddModButtonVisible);
    }


    private bool IsAddModButtonVisible(long workshopId)
    {
        return StateService.EditingPlayset?.ModStatus.Mods.FirstOrDefault(x => x.WorkshopId == workshopId)?.Hidden ??
               false;
    }
}
