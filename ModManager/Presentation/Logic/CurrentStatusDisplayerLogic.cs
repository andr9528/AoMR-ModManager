using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;

namespace ModManager.Presentation.Logic;

public class CurrentStatusDisplayerLogic : BaseDisplayerLogic
{
    public CurrentStatusDisplayerLogic(IStateService stateService) : base(stateService)
    {
    }


    public void AddModClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        if (button?.Tag is not long workshopId)
        {
            return;
        }

        IMod? mod = StateService.EditingPlayset?.ModStatus.Mods.FirstOrDefault(x => x.WorkshopId == workshopId);

        if (mod == null)
        {
            return;
        }

        mod.IsHidden = false;
    }
}
