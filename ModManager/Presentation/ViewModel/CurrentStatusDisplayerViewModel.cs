using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;

namespace ModManager.Presentation.ViewModel;

public class CurrentStatusDisplayerViewModel : IViewModel
{
    public IStateService StateService { get; }

    public CurrentStatusDisplayerViewModel(IStateService stateService)
    {
        StateService = stateService;

        StateService.InitializationCompleted += StateService_InitializationCompleted;
        StateService.EditingPlaysetChanged += StateService_EditingPlaysetChanged;
    }

    private void StateService_EditingPlaysetChanged(object? sender, IPlayset? e)
    {
        UpdateButtonEnabledSibling();
    }

    private void UpdateButtonEnabledSibling()
    {
        StateService.EditingPlayset?.ModStatus.Mods.ForEach(mod =>
        {
            mod.IsHiddenChanged -= Mod_HiddenChanged;
            mod.IsHiddenChanged += Mod_HiddenChanged;
        });
    }

    private void StateService_InitializationCompleted(object? sender, bool e)
    {
        UpdateButtonEnabledSibling();
    }

    private void Mod_HiddenChanged(object? sender, bool e)
    {
        if (sender is not IMod eventMod)
        {
            return;
        }

        IMod? currentMod = StateService.CurrentModStatus?.Mods.FirstOrDefault(x => x.IsMatchingMod(eventMod));

        if (currentMod == null)
        {
            return;
        }

        currentMod.IsHiddenSibling = eventMod.IsHidden;
    }
}
