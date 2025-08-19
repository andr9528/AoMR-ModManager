using System.Collections;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation.Core;

public abstract class BaseLogic
{
    protected readonly IStateService StateService;

    protected BaseLogic(IStateService stateService)
    {
        StateService = stateService;
    }
}
