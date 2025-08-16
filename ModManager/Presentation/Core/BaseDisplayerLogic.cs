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
}
