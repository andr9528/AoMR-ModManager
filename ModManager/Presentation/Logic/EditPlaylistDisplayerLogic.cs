using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;

namespace ModManager.Presentation;

public class EditPlaylistDisplayerLogic : BaseDisplayerLogic, IDisplayerLogic
{
    private readonly EditPlaylistDisplayerViewModel viewModel;

    public EditPlaylistDisplayerLogic(IStateService stateService, EditPlaylistDisplayerViewModel viewModel) :
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
        if (mod != null)
        {
            mod.IsHidden = true;
        }
    }
}
