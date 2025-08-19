using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;

namespace ModManager.Presentation.Logic;

public class CurrentStatusRegionLogic : BaseLogic, IDisplayerLogic
{
    public CurrentStatusRegionLogic(IStateService stateService) : base(stateService)
    {
    }


    public void AddModClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        if (button?.Tag is not IMod taggedMod)
        {
            return;
        }

        IMod? mod = StateService.EditingPlayset?.ModStatus.Mods.FirstOrDefault(x => x.IsMatchingMod(taggedMod));

        if (mod == null)
        {
            return;
        }

        mod.IsHidden = false;
    }

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

        IMod? mod = StateService.CurrentModStatus?.Mods.FirstOrDefault(x => x.IsMatchingMod(taggedMod));

        if (mod != null)
        {
            mod.IsEnabled = !mod.IsEnabled;
        }
    }
}
