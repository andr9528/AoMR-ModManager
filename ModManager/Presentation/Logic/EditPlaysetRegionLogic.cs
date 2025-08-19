using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;

namespace ModManager.Presentation.Logic;

public class EditPlaylistRegionLogic : BaseDisplayerLogic, IDisplayerLogic
{
    private readonly ViewModel.EditPlaylistRegionViewModel viewModel;

    public EditPlaylistRegionLogic(IStateService stateService, ViewModel.EditPlaylistRegionViewModel viewModel) :
        base(stateService)
    {
        this.viewModel = viewModel;
    }

    /// <inheritdoc />
    public void EnabledIndicatorButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not IMod taggedMod)
        {
            return;
        }

        IMod? mod = StateService.EditingPlayset?.ModStatus?.Mods.FirstOrDefault(x => x.IsMatchingMod(taggedMod));

        if (mod != null)
        {
            mod.IsEnabled = !mod.IsEnabled;
        }
    }

    public void RemoveModClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not IMod taggedMod)
        {
            return;
        }

        IMod? mod = StateService.EditingPlayset?.ModStatus.Mods.FirstOrDefault(x => x.IsMatchingMod(taggedMod));
        if (mod == null)
        {
            return;
        }

        mod.IsEnabled = false;
        mod.IsHidden = true;
    }
}
