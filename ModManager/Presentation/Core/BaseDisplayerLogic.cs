using System.Collections;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation.Core;

public abstract class BaseDisplayerLogic
{
    protected readonly IStateService StateService;

    protected BaseDisplayerLogic(IStateService stateService)
    {
        StateService = stateService;
    }

    protected bool IsClickedMod(IMod mod, IMod taggedMod)
    {
        if (mod.WorkshopId != 0 && taggedMod.WorkshopId != 0 && mod.WorkshopId == taggedMod.WorkshopId)
        {
            return true;
        }

        return mod.Title.Equals(taggedMod.Title, StringComparison.InvariantCultureIgnoreCase) &&
               mod.IsLocalMod == taggedMod.IsLocalMod;
    }
}
